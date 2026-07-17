using System.Globalization;
using System.Text;

using Cursor_Installer_Creator.Data;
using Cursor_Installer_Creator.Repository.CursorAssignmentRepo;

using Microsoft.Extensions.Logging;

namespace Cursor_Installer_Creator.Service.InfServ;

public sealed class InfParserService : IInfParserService
{
    private readonly ICursorAssignmentRepository _cursorAssignmentRepository;
    private readonly ILogger<InfParserService> _logger;

    public InfParserService(ICursorAssignmentRepository cursorAssignmentRepository, ILogger<InfParserService> logger)
    {
        _cursorAssignmentRepository = cursorAssignmentRepository;
        _logger = logger;
    }

    public InfFile Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("INF content cannot be null or empty.", nameof(content));

        _logger.LogDebug("Parsing INF content, length={Length} characters", content.Length);
        var parsed = ParseInf(content);
        _logger.LogDebug("Parsed {SectionCount} INF sections: {Sections}", parsed.Sections.Count, string.Join(", ", parsed.Sections.Keys));
        return BuildInfFile(parsed);
    }

    public async Task<InfFile> ParseAsync(Stream stream, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
            throw new ArgumentException("The provided stream must be readable.", nameof(stream));

        encoding ??= Encoding.UTF8;

        using var reader = new StreamReader(
            stream,
            encoding,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 4096,
            leaveOpen: true);

        var content = await reader.ReadToEndAsync(cancellationToken);
        _logger.LogDebug("Read {Length} characters from INF stream", content.Length);
        var infFile = Parse(content);
        var sorted = infFile.Cursors.OrderBy(c => c.Assignment?.Order).ToList();
        infFile.Cursors.Clear();
        foreach (var entry in sorted)
            infFile.Cursors.Add(entry);
        return infFile;
    }

    public Task<InfFile> ParseAsync(string content, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Parse(content));
    }

    private InfFile BuildInfFile(ParsedInf parsed)
    {
        var assignments = _cursorAssignmentRepository
            .GetAllAssignments()
            .OrderBy(x => x.Order)
            .ToArray();

        var inf = new InfFile();

        ReadVersion(parsed, inf);
        _logger.LogDebug("INF version: Provider={Provider}, Signature={Signature}, DriverVer={DriverVer}", inf.Provider, inf.Signature, inf.DriverVer);

        if (parsed.Strings.TryGetValue("CUR_DIR", out var curDir) && !string.IsNullOrWhiteSpace(curDir))
        {
            inf.CursorDirectory = curDir;
            _logger.LogDebug("CUR_DIR={CurDir}", curDir);
        }
        else
        {
            _logger.LogDebug("CUR_DIR not found in [Strings]");
        }

        var diskSubdirs = ParseSourceDisksNames(parsed);
        var fileDisks = ParseSourceDisksFiles(parsed);

        if (TryReadSchemeEntry(parsed, assignments, inf, diskSubdirs, fileDisks))
        {
            _logger.LogInformation("INF loaded via Scheme.Reg entry: SchemeName={SchemeName}, cursors={Count}", inf.SchemeName, inf.Cursors.Count);
            return inf;
        }

        _logger.LogDebug("No Scheme.Reg entry found, falling back to per-cursor registry entries");
        TryReadCursorEntries(parsed, inf, diskSubdirs, fileDisks);
        _logger.LogInformation("INF loaded via per-cursor entries: SchemeName={SchemeName}, cursors={Count}", inf.SchemeName, inf.Cursors.Count);

        return inf;
    }

    private static Dictionary<string, string> ParseSourceDisksNames(ParsedInf parsed)
    {
        var sectionPriority = new[]
        {
            "SourceDisksNames",
            "SourceDisksNames.x86",
            "SourceDisksNames.arm",
            "SourceDisksNames.arm64",
            "SourceDisksNames.amd64",
        };

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var sectionName in sectionPriority)
        {
            if (!parsed.Sections.TryGetValue(sectionName, out var section))
                continue;

            foreach (var directive in section.Directives)
            {
                var diskId = directive.Key.Trim();
                var fields = SplitCsvLike(directive.Value);
                var diskPath = fields.Count > 3
                    ? Expand(Unquote(fields[3]), parsed.Strings).Trim().Trim('\\', '/')
                    : string.Empty;
                result[diskId] = diskPath;
            }
        }

        return result;
    }

    private static Dictionary<string, (string DiskId, string Subdir)> ParseSourceDisksFiles(ParsedInf parsed)
    {
        var sectionPriority = new[]
        {
            "SourceDisksFiles",
            "SourceDisksFiles.x86",
            "SourceDisksFiles.arm",
            "SourceDisksFiles.arm64",
            "SourceDisksFiles.amd64",
        };

        var result = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase);

        foreach (var sectionName in sectionPriority)
        {
            if (!parsed.Sections.TryGetValue(sectionName, out var section))
                continue;

            foreach (var directive in section.Directives)
            {
                var filename = directive.Key.Trim();
                var fields = SplitCsvLike(directive.Value);
                var diskId = fields.Count > 0
                    ? Expand(Unquote(fields[0]), parsed.Strings).Trim()
                    : "1";
                var subdir = fields.Count > 1
                    ? Expand(Unquote(fields[1]), parsed.Strings).Trim().Trim('\\', '/')
                    : string.Empty;
                result[filename] = (diskId, subdir);
            }
        }

        return result;
    }

    private static string ResolveRelativePath(string filename, Dictionary<string, string> diskSubdirs, Dictionary<string, (string DiskId, string Subdir)> fileDisks)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return string.Empty;

        var (diskId, perFileSubdir) = fileDisks.TryGetValue(filename, out var entry)
            ? entry
            : ("1", string.Empty);

        var diskPath = diskSubdirs.TryGetValue(diskId, out var dp) ? dp : string.Empty;
        var segments = new[] { diskPath, perFileSubdir, filename }
            .Where(s => !string.IsNullOrWhiteSpace(s));

        return string.Join('\\', segments);
    }

    private static void ReadVersion(ParsedInf parsed, InfFile inf)
    {
        if (!parsed.Sections.TryGetValue("Version", out var versionSection))
            return;

        foreach (var directive in versionSection.Directives)
        {
            var value = Unquote(Expand(directive.Value, parsed.Strings));

            if (directive.Key.Equals("Provider", StringComparison.OrdinalIgnoreCase))
            {
                inf.Provider = value;
            }
            else if (directive.Key.Equals("Signature", StringComparison.OrdinalIgnoreCase))
            {
                inf.Signature = value;
            }
            else if (directive.Key.Equals("DriverVer", StringComparison.OrdinalIgnoreCase))
            {
                var parts = value.Split(',', 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    if (DateOnly.TryParseExact(parts[0], "MM/dd/yyyy", out var date))
                        inf.DriverVerDate = date;

                    try
                    {
                        inf.DriverVer = new Version(parts[1]);
                    }
                    catch { /* malformed version string — leave null */ }
                }
            }
        }
    }

    private static bool TryReadSchemeEntry(ParsedInf parsed, CursorAssignment[] assignments, InfFile inf, Dictionary<string, string> diskSubdirs, Dictionary<string, (string DiskId, string Subdir)> fileDisks)
    {
        foreach (var section in parsed.Sections.Values)
        {
            foreach (var rawLine in section.RawLines)
            {
                var fields = SplitCsvLike(rawLine)
                    .Select(x => Expand(Unquote(x), parsed.Strings))
                    .ToArray();

                if (fields.Length < 5)
                    continue;

                var root = fields[0];
                var path = fields[1];
                var valueName = fields[2];
                var data = fields[4];

                if (!root.Equals("HKCU", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!path.Equals(@"Control Panel\Cursors\Schemes", StringComparison.OrdinalIgnoreCase))
                    continue;

                inf.SchemeName = valueName;

                var cursorPaths = SplitCsvLike(data)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();

                for (var i = 0; i < cursorPaths.Length; i++)
                {
                    var assignment = i < assignments.Length ? assignments[i] : null;

                    inf.Cursors.Add(new InfCursorEntry
                    {
                        Assignment = assignment,
                        SourcePath = ResolveRelativePath(
                            ExtractFilenameFromAbsolutePath(cursorPaths[i]),
                            diskSubdirs, fileDisks)
                    });
                }

                return true;
            }
        }

        return false;
    }

    private bool TryReadCursorEntries(ParsedInf parsed, InfFile inf, Dictionary<string, string> diskSubdirs, Dictionary<string, (string DiskId, string Subdir)> fileDisks)
    {
        var foundAny = false;

        foreach (var section in parsed.Sections.Values)
        {
            foreach (var rawLine in section.RawLines)
            {
                var fields = SplitCsvLike(rawLine)
                    .Select(x => Expand(Unquote(x), parsed.Strings))
                    .ToArray();

                if (fields.Length < 5)
                    continue;

                var root = fields[0];
                var path = fields[1];
                var valueName = fields[2];
                var data = fields[4];

                if (!root.Equals("HKCU", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!path.Equals(@"Control Panel\Cursors", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (valueName.Equals("Scheme Source", StringComparison.OrdinalIgnoreCase))
                    continue;

                var assignment = _cursorAssignmentRepository.GetAssignmentFromName(
                    name: valueName,
                    order: [CursorAssignmentType.WindowsReg, CursorAssignmentType.WindowsInstall, CursorAssignmentType.Name]
                );

                var sourcePath = ResolveRelativePath(
                    ExtractFilenameFromAbsolutePath(data),
                    diskSubdirs, fileDisks);
                _logger.LogDebug("Cursor entry: regName={RegName}, sourcePath={SourcePath}", valueName, sourcePath);

                inf.Cursors.Add(new InfCursorEntry
                {
                    Assignment = assignment,
                    SourcePath = sourcePath,
                });

                foundAny = true;
            }
        }

        if (string.IsNullOrWhiteSpace(inf.SchemeName))
        {
            foreach (var kvp in parsed.Strings)
            {
                if (kvp.Key.Contains("scheme", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(kvp.Value))
                {
                    inf.SchemeName = kvp.Value;
                    break;
                }
            }
        }

        return foundAny;
    }

    private static ParsedInf ParseInf(string content)
    {
        var result = new ParsedInf();
        ParsedInfSection? currentSection = null;

        using var reader = new StringReader(content);
        string? rawLine;

        while ((rawLine = reader.ReadLine()) is not null)
        {
            var line = RemoveComment(rawLine).Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                var sectionName = line[1..^1].Trim();

                currentSection = new ParsedInfSection
                {
                    Name = sectionName
                };

                result.Sections[sectionName] = currentSection;
                continue;
            }

            if (currentSection is null)
                continue;

            currentSection.RawLines.Add(line);

            var equalsIndex = line.IndexOf('=');
            if (equalsIndex > 0)
            {
                var key = line[..equalsIndex].Trim();
                var value = line[(equalsIndex + 1)..].Trim();
                currentSection.Directives.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        if (result.Sections.TryGetValue("Strings", out var stringsSection))
        {
            foreach (var directive in stringsSection.Directives)
            {
                result.Strings[directive.Key] = Unquote(directive.Value);
            }
        }

        return result;
    }

    private static string Expand(string value, IReadOnlyDictionary<string, string> strings)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var result = value;

        for (var i = 0; i < 10; i++)
        {
            var changed = false;

            foreach (var kvp in strings)
            {
                var token = $"%{kvp.Key}%";

                if (result.Contains(token, StringComparison.OrdinalIgnoreCase))
                {
                    result = result.Replace(token, kvp.Value, StringComparison.OrdinalIgnoreCase);
                    changed = true;
                }
            }

            if (!changed)
                break;
        }

        return result;
    }

    private static string RemoveComment(string line)
    {
        if (string.IsNullOrEmpty(line))
            return line;

        var sb = new StringBuilder();
        var inQuotes = false;

        foreach (var c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                sb.Append(c);
                continue;
            }

            if (c == ';' && !inQuotes)
                break;

            sb.Append(c);
        }

        return sb.ToString();
    }

    private static List<string> SplitCsvLike(string input)
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(input))
            return result;

        var sb = new StringBuilder();
        var inQuotes = false;

        foreach (var c in input)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                sb.Append(c);
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString().Trim());
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }

        if (sb.Length > 0)
            result.Add(sb.ToString().Trim());

        return result;
    }

    private static string ExtractFilenameFromAbsolutePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        var value = Unquote(path).Trim().Replace('/', '\\');

        var lastSlash = value.LastIndexOf('\\');
        return lastSlash >= 0 ? value[(lastSlash + 1)..] : value;
    }

    private static string Unquote(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        value = value.Trim();

        return value.Length >= 2 && value[0] == '"' && value[^1] == '"'
            ? value[1..^1]
            : value;
    }

    private sealed class ParsedInf
    {
        public Dictionary<string, ParsedInfSection> Sections { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> Strings { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed class ParsedInfSection
    {
        public string Name { get; init; } = string.Empty;
        public List<KeyValuePair<string, string>> Directives { get; } = [];
        public List<string> RawLines { get; } = [];
    }

    // ── Serialization ──────────────────────────────────────────────────────────

    private static readonly HashSet<char> InvalidPathChars = [.. Path.GetInvalidFileNameChars()];

    public string Serialize(InfFile infFile)
    {
        ArgumentNullException.ThrowIfNull(infFile);

        var schemeName = infFile.SchemeName ?? Constants.DefaultInstallerSchemeName;
        var safeFolderName = string.Concat(schemeName.Select(c => InvalidPathChars.Contains(c) ? '_' : c));
        var cursorDir = infFile.CursorDirectory ?? @$"Cursors\{safeFolderName}";

        // Sort entries by assignment order; unassigned entries go last.
        var entries = infFile.Cursors
            .OrderBy(e => e.Assignment?.Order ?? int.MaxValue).ToArray();

        // Canonical ordered list of all 17 Windows cursor slots for positional scheme CSV.
        var allAssignments = _cursorAssignmentRepository.GetAllAssignments()
            .OrderBy(a => a.Order).ToArray();
        var entriesByAssignmentId = entries
            .Where(e => e.Assignment is not null).ToDictionary(e => e.Assignment!.Id);

        // Derive filename and registry name from the actual ICursor, falling back to the slot assignment.
        static string? GetFileName(InfCursorEntry e)
        {
            var assignment = e.Cursor?.Assignment ?? e.Assignment;
            var install = assignment?.WindowsInstall;
            if (string.IsNullOrWhiteSpace(install))
                return null;
            var ext = e.Cursor?.Type == CursorType.ani ? ".ani" : ".cur";
            return install + ext;
        }

        static string? GetRegName(InfCursorEntry e)
            => (e.Cursor?.Assignment ?? e.Assignment)?.WindowsReg;

        var computed = entries.ToDictionary(e => e, e => (FileName: GetFileName(e), RegName: GetRegName(e)));

        var sb = new StringBuilder();

        var signature = string.IsNullOrWhiteSpace(infFile.Signature)
            ? Constants.DefaultInstallerSignature
            : infFile.Signature;
        var provider = string.IsNullOrWhiteSpace(infFile.Provider)
            ? Constants.DefaultInstallerProvider
            : infFile.Provider;
        var driverVerDate = (infFile.DriverVerDate?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
            ?? DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        var driverVerVersion = infFile.DriverVer is { } dv ? dv.ToString() : "1.0.0.0";

        // [Version]
        sb.AppendLine($"; Generated by {Constants.AppName}");
        sb.AppendLine();
        sb.AppendLine("[Version]");
        sb.AppendLine($"signature=\"{InfEscapeStringValue(signature)}\"");
        sb.AppendLine($"DriverVer={driverVerDate},{driverVerVersion}");
        sb.AppendLine("Provider=%PROVIDER%");
        sb.AppendLine();

        // [DefaultInstall]
        sb.AppendLine("[DefaultInstall]");
        sb.AppendLine("CopyFiles = Scheme.Cur");
        sb.AppendLine("AddReg    = Scheme.Reg");
        sb.AppendLine();

        // [DestinationDirs]
        sb.AppendLine("[DestinationDirs]");
        sb.AppendLine("Scheme.Cur = 10,\"%CUR_DIR%\"");
        sb.AppendLine();

        // [SourceDisksNames]
        sb.AppendLine("[SourceDisksNames]");
        sb.AppendLine("1 = %DISK_NAME%,,,");
        sb.AppendLine();

        // [SourceDisksFiles]
        sb.AppendLine("[SourceDisksFiles]");
        foreach (var fn in entries.Select(e => computed[e].FileName).Where(fn => !string.IsNullOrEmpty(fn)).Distinct())
        {
            sb.AppendLine($"{fn} = 1");
        }
        sb.AppendLine();

        // [Scheme.Cur]
        sb.AppendLine("[Scheme.Cur]");
        foreach (var e in entries)
        {
            var fn = computed[e].FileName;
            if (!string.IsNullOrEmpty(fn))
                sb.AppendLine(fn);
        }
        sb.AppendLine();

        // [Scheme.Reg]
        sb.AppendLine("[Scheme.Reg]");
        foreach (var e in entries)
        {
            var regName = computed[e].RegName;
            if (string.IsNullOrWhiteSpace(regName))
                continue;
            var fn = computed[e].FileName;
            var value = string.IsNullOrEmpty(fn) ? string.Empty : $"%10%\\%CUR_DIR%\\{fn}";
            sb.AppendLine($"HKCU,\"Control Panel\\Cursors\",\"{regName}\",,\"{value}\"");
        }

        // Scheme CSV entry — always 17 positional slots in canonical Windows order.
        // Absent slots emit an empty string so that subsequent slots land in the correct position.
        var schemeValues = allAssignments.Select(a =>
        {
            if (!entriesByAssignmentId.TryGetValue(a.Id, out var e))
                return string.Empty;
            var fn = computed[e].FileName;
            return string.IsNullOrEmpty(fn) ? string.Empty : $"%10%\\%CUR_DIR%\\{fn}";
        });
        sb.AppendLine($"HKCU,\"Control Panel\\Cursors\\Schemes\",\"%SCHEME_NAME%\",,\"{string.Join(',', schemeValues)}\"");
        sb.AppendLine();

        // [Strings]
        sb.AppendLine("[Strings]");
        sb.AppendLine($"DISK_NAME   = \"{InfEscapeStringValue(schemeName)}\"");
        sb.AppendLine($"SCHEME_NAME = \"{InfEscapeStringValue(schemeName)}\"");
        sb.AppendLine($"CUR_DIR     = \"{InfEscapeStringValue(cursorDir)}\"");
        sb.AppendLine($"PROVIDER    = \"{InfEscapeStringValue(provider)}\"");

        return sb.ToString();
    }

    private static string InfEscapeStringValue(string value)
        => value.Replace("\"", "\"\"");
}

using System.IO.Compression;
using System.Text;

using Avalonia.Platform;
using Avalonia.Platform.Storage;

using Cursor_Installer_Creator.Data;
using Cursor_Installer_Creator.Extensions;
using Cursor_Installer_Creator.Repository.CursorAssignmentRepo;
using Cursor_Installer_Creator.Service.InfServ;

using Microsoft.Extensions.Logging;

namespace Cursor_Installer_Creator.Repository.InfFileRepo;

public sealed class InfFileRepository : IInfFileRepository
{
    private static readonly UTF8Encoding _utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    private readonly IInfParserService _infParserService;
    private readonly ICursorAssignmentRepository _cursorAssignmentRepository;
    private readonly ILogger<InfFileRepository> _logger;

    public InfFileRepository(IInfParserService infParserService, ICursorAssignmentRepository cursorAssignmentRepository, ILogger<InfFileRepository> logger)
    {
        _infParserService = infParserService;
        _cursorAssignmentRepository = cursorAssignmentRepository;
        _logger = logger;
    }

    public async Task<InfFile?> GetInfFileFromFileAsync(IStorageFile file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        _logger.LogDebug("Loading INF file: {FileName}", file.Name);
        await using var stream = await file.OpenReadAsync();
        var infFile = await _infParserService.ParseAsync(stream, cancellationToken: cancellationToken);
        await AddMissingCursorEntriesAsync(infFile);
        _logger.LogDebug("Loaded INF file: {FileName}, SchemeName={SchemeName}, cursors={Count}", file.Name, infFile.SchemeName, infFile.Cursors.Count);
        return infFile;
    }

    public async Task<InfFile?> GetInfFileFromBytesAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        _logger.LogDebug("Loading INF from byte array, length={Length}", data.Length);
        await using var stream = new MemoryStream(data, writable: false);
        var infFile = await _infParserService.ParseAsync(stream, cancellationToken: cancellationToken);
        await AddMissingCursorEntriesAsync(infFile);
        _logger.LogDebug("Loaded INF from bytes: SchemeName={SchemeName}, cursors={Count}", infFile.SchemeName, infFile.Cursors.Count);
        return infFile;
    }

    public async Task<InfFile?> GetInfFileFromTextAsync(string data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(data);

        var infFile = await _infParserService.ParseAsync(data, cancellationToken: cancellationToken);
        await AddMissingCursorEntriesAsync(infFile);
        return infFile;
    }

    public async Task AddMissingCursorEntriesAsync(InfFile infFile)
    {
        var assignments = _cursorAssignmentRepository.GetAllAssignments().ToList();
        if (infFile.Cursors.Count >= assignments.Count)
            return;

        assignments.RemoveAll(x => infFile.Cursors.Any(c => c.Assignment == x));
        foreach (var assignment in assignments)
        {
            infFile.Cursors.Add(new InfCursorEntry
            {
                Assignment = assignment,
            });
        }
        _logger.LogDebug("Added {Count} missing cursor entries", assignments.Count);
    }

    public async Task WriteInfFileAsync(InfFile infFile, IStorageFile destination)
    {
        ArgumentNullException.ThrowIfNull(infFile);
        ArgumentNullException.ThrowIfNull(destination);

        var content = _infParserService.Serialize(infFile);
        await using var stream = await destination.OpenWriteAsync();
        await using var writer = new StreamWriter(stream, _utf8NoBom);
        await writer.WriteAsync(content);
    }

    public async Task WriteInfPackageAsync(InfFile infFile, IStorageFolder destination, bool archive = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(infFile);
        ArgumentNullException.ThrowIfNull(destination);

        if (string.IsNullOrWhiteSpace(infFile.SchemeName) || string.IsNullOrWhiteSpace(infFile.Provider))
            throw new InvalidOperationException("Package Name and Provider are required for export.");

        _logger.LogInformation("Exporting INF package: SchemeName={SchemeName}, archive={Archive}", infFile.SchemeName, archive);

        if (archive)
        {
            await WriteInfPackageArchiveAsync(infFile, destination, cancellationToken);
            return;
        }

        var cleanedInfFile = CreateCleanedInfFile(infFile);

        foreach (var cursorEntry in cleanedInfFile.Cursors)
        {
            var fileName = $"{cursorEntry.Cursor!.Assignment!.WindowsInstall}.{cursorEntry.Cursor!.FileExtension}";
            var cursorStorageFile = await destination.CreateFileAsync(fileName)
                ?? throw new InvalidOperationException($"Could not create cursor file '{fileName}'.");
            await using var stream = await cursorStorageFile.OpenWriteAsync();
            await stream.WriteAsync(cursorEntry.Cursor.CursorBytes, cancellationToken);
        }

        var infStorageFile = await destination.CreateFileAsync(Constants.DefaultInstallerFileName)
            ?? throw new InvalidOperationException($"Could not create INF file '{Constants.DefaultInstallerFileName}'.");
        await WriteInfFileAsync(cleanedInfFile, infStorageFile);
    }

    public async Task WriteInfPackageArchiveAsync(InfFile infFile, IStorageFolder destination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(infFile);
        ArgumentNullException.ThrowIfNull(destination);

        var cleanedInfFile = CreateCleanedInfFile(infFile);

        var schemeName = string.IsNullOrWhiteSpace(cleanedInfFile.SchemeName) ? Constants.DefaultInstallerSchemeName : cleanedInfFile.SchemeName;
        var zipStorageFile = await destination.CreateFileAsync($"{schemeName}.zip")
            ?? throw new InvalidOperationException("Could not create archive file.");

        await using var output = await zipStorageFile.OpenWriteAsync();
        using var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true);

        foreach (var cursorEntry in cleanedInfFile.Cursors)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = cursorEntry.Cursor!;
            var entryName = $"{cursor.Assignment!.WindowsInstall}.{cursor.FileExtension}";

            var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);

            await using var entryStream = entry.Open();
            await entryStream.WriteAsync(cursor.CursorBytes, cancellationToken);
        }

        var infEntry = archive.CreateEntry(Constants.DefaultInstallerFileName, CompressionLevel.Optimal);
        await using var infEntryStream = infEntry.Open();
        var content = _infParserService.Serialize(cleanedInfFile);
        await using var writer = new StreamWriter(infEntryStream, _utf8NoBom);
        await writer.WriteAsync(content);
    }

    public async Task WriteInfPackageInstallerAsync(InfFile infFile, IStorageFolder destination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(infFile);
        ArgumentNullException.ThrowIfNull(destination);

        if (string.IsNullOrWhiteSpace(infFile.SchemeName) || string.IsNullOrWhiteSpace(infFile.Provider))
            throw new InvalidOperationException("Package Name and Provider are required for export.");

        _logger.LogInformation("Exporting installer script: SchemeName={SchemeName}", infFile.SchemeName);

        var cleanedInfFile = CreateCleanedInfFile(infFile);

        var cursors = new Dictionary<string, string>(cleanedInfFile.Cursors.Count);
        foreach (var cursorEntry in cleanedInfFile.Cursors)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = cursorEntry.Cursor!;
            var fileName = $"{cursor.Assignment!.WindowsInstall}.{cursor.FileExtension}";
            cursors[fileName] = EncodeGzipBase64(cursor.CursorBytes);
        }

        var infContent = _infParserService.Serialize(cleanedInfFile);
        var scriptContent = await BuildInstallerScriptAsync(cleanedInfFile.SchemeName!, cleanedInfFile.Provider!, cursors, infContent, cancellationToken);

        var installerFileName = cleanedInfFile.SchemeName!.Replace(" ", "_");
        var installerFile = await destination.CreateFileAsync($"{installerFileName}-Setup.bat")
            ?? throw new InvalidOperationException("Could not create installer file.");

        await using var stream = await installerFile.OpenWriteAsync();
        await using var writer = new StreamWriter(stream, _utf8NoBom);
        await writer.WriteAsync(scriptContent);
    }

    private static async Task<string> BuildInstallerScriptAsync(string schemeName, string provider, Dictionary<string, string> cursors, string infContent, CancellationToken cancellationToken)
    {
        using var stream = AssetLoader.Open(new Uri($"avares://{nameof(Cursor_Installer_Creator)}/Resources/inf-installer.ps1"));
        using var reader = new StreamReader(stream, _utf8NoBom);
        var template = await reader.ReadToEndAsync(cancellationToken);

        var cursorsDict = string.Join(Environment.NewLine + "    ", cursors.Select(kv => $"'{kv.Key}' = '{kv.Value}'"));

        return template
            .Replace("{{SCHEME_NAME}}", schemeName)
            .Replace("{{PROVIDER}}", provider)
            // Some auto-formatters expand {{token}} to { { token } } inside PS1 files; the second Replace handles that.
            .Replace("{{CURSORS_DICT}}", cursorsDict).Replace("{ { CURSORS_DICT } }", cursorsDict)
            .Replace("{{INF_CONTENT}}", infContent.TrimEnd())
            .Replace("{{SCHEME_NAME_ESCAPED}}", schemeName.Replace("'", "''"));
    }

    private static string EncodeGzipBase64(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gz = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            gz.Write(data);
        }
        return Convert.ToBase64String(output.ToArray());
    }

    private static InfFile CreateCleanedInfFile(InfFile infFile)
    {
        var cleaned = new InfFile
        {
            SchemeName = infFile.SchemeName,
            Provider = infFile.Provider,
            CursorDirectory = infFile.CursorDirectory,
        };
        cleaned.Cursors.AddRange(infFile.Cursors.Where(x => x is not null && x.Cursor is not null));

        if (cleaned.Cursors.Any(x => x is null || x.Assignment is null))
            throw new InvalidOperationException("All cursor entries must have an assigned cursor and assignment.");

        return cleaned;
    }
}

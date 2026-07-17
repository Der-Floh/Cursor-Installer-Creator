using System.Runtime.Versioning;

using Avalonia.Platform;
using Avalonia.Platform.Storage;

using Cursor_Installer_Creator.Data;
using Cursor_Installer_Creator.Repository.CursorAssignmentRepo;
using Cursor_Installer_Creator.Service.CursorServ;

using Microsoft.Extensions.Logging;

using Microsoft.Win32;

namespace Cursor_Installer_Creator.Repository.CursorRepo;

public sealed class CursorRepository : ICursorRepository
{
    private const string WindowsCursorRegistryPath = @"Control Panel\Cursors";
    private static readonly string WindowsCursorFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Cursors");

    private readonly ICursorAssignmentRepository _cursorAssignmentRepository;
    private readonly ICursorService _cursorService;
    private readonly ILogger<CursorRepository> _logger;

    public CursorRepository(ICursorAssignmentRepository cursorAssignmentRepository, ICursorService cursorService, ILogger<CursorRepository> logger)
    {
        _cursorAssignmentRepository = cursorAssignmentRepository;
        _cursorService = cursorService;
        _logger = logger;
    }

    public async Task<ICursor?> GetCursorFromFileAsync(IStorageFile file, CursorAssignment? assignment = null)
    {
        ArgumentNullException.ThrowIfNull(file);

        var fileName = file.Name;
        _logger.LogDebug("Loading cursor from file: {FileName}", fileName);

        await using var stream = await file.OpenReadAsync();

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        var cursorType = _cursorService.GetCursorTypeFromFile(fileName);
        var cursor = GetCursorFromBytes(bytes, cursorType, assignment);
        _logger.LogDebug("Loaded cursor: {FileName}, type={Type}, bytes={Bytes}", fileName, cursorType, bytes.Length);

        return cursor;
    }

    public ICursor GetCursorFromBytes(byte[] data, CursorType type, CursorAssignment? assignment = null)
    {
        ArgumentNullException.ThrowIfNull(data);

        return type switch
        {
            CursorType.cur => new CCursor(data, type, assignment),
            CursorType.ani => new ACursor(data, type, assignment),
            _ => throw new NotSupportedException($"Unsupported cursor type for file: {data}"),
        };
    }

    public async Task<ICursor[]> GetDefaultCursorsAsync()
    {
        var assignments = _cursorAssignmentRepository.GetAllAssignments();
        var cursors = new List<ICursor>();
        foreach (var assignment in assignments)
        {
            var cursor = await GetDefaultCursorAsync(assignment);
            if (cursor is not null)
                cursors.Add(cursor);
        }
        return [.. cursors];
    }

    public async Task<ICursor?> GetDefaultCursorAsync(CursorAssignment assignment)
    {
        var cursorPath = @$"Assets/Cursors/{assignment.WindowsDefault}.cur";
        var cursorBytes = await AssetLoaderTryGetAsset(cursorPath);
        if (cursorBytes is null)
        {
            cursorPath = Path.ChangeExtension(cursorPath, ".ani");
            cursorBytes = await AssetLoaderTryGetAsset(cursorPath);
        }

        if (cursorBytes is null)
        {
            _logger.LogDebug("Default cursor asset not found for assignment: {Name}", assignment.Name);
            return null;
        }

        var cursorType = _cursorService.GetCursorTypeFromFile(cursorPath);
        var cursor = GetCursorFromBytes(cursorBytes, cursorType, assignment);
        _logger.LogDebug("Loaded default cursor: {Name}, type={Type}", assignment.Name, cursorType);
        return cursor;
    }

    [SupportedOSPlatform("windows")]
    public async Task<ICursor[]> GetCurrWindowsCursorsAsync(bool fallbackToDefault = false)
    {
        if (!OperatingSystem.IsWindows() && !fallbackToDefault)
            throw new PlatformNotSupportedException("This method is only supported on Windows.");

        var cursors = new List<ICursor>();

        if (OperatingSystem.IsWindows())
        {
            using var key = Registry.CurrentUser.OpenSubKey(WindowsCursorRegistryPath, writable: false);
            if (key is not null)
            {
                // Search Registry for cursors
                var valueNames = key.GetValueNames();
                foreach (var valueName in valueNames)
                {
                    var registryPath = key.GetValue(valueName)?.ToString();
                    var assignment = _cursorAssignmentRepository.GetAssignmentFromName(valueName, CursorAssignmentType.WindowsReg);
                    var cursorPath = ResolveWindowsCursorPath(registryPath, assignment?.WindowsDefault);

                    if (cursorPath is not null)
                    {
                        var cursorType = _cursorService.GetCursorTypeFromFile(cursorPath);
                        var cursorBytes = await File.ReadAllBytesAsync(cursorPath);
                        var cursor = GetCursorFromBytes(cursorBytes, cursorType, assignment);
                        cursors.Add(cursor);
                    }
                }
            }

            if (!fallbackToDefault)
                return [.. cursors.OrderBy(x => x.Assignment?.Order)];
        }

        // Add missing cursors from CursorAssignment
        cursors = [.. await AddMissingCursorsAsync(cursors)];

        return [.. cursors.OrderBy(x => x.Assignment?.Order)];
    }

    public async Task<ICursor[]> AddMissingCursorsAsync(IEnumerable<ICursor?> cursors)
    {
        var allCursors = new List<ICursor>();
        allCursors.AddRange(cursors.Where(x => x is not null).Select(x => x!));
        var assignments = _cursorAssignmentRepository.GetAllAssignments().ToList();
        assignments.RemoveAll(x => allCursors.Any(c => c.Assignment == x));

        foreach (var assignment in assignments)
        {
            var cursor = await GetDefaultCursorAsync(assignment);
            if (cursor is not null)
                allCursors.Add(cursor);
        }

        return [.. allCursors];
    }

    [SupportedOSPlatform("windows")]
    public async Task<ICursor?> GetCurrWindowsCursorAsync(CursorAssignment assignment, bool fallbackToDefault = false)
    {
        if (!OperatingSystem.IsWindows() && !fallbackToDefault)
            throw new PlatformNotSupportedException("This method is only supported on Windows.");

        if (OperatingSystem.IsWindows())
        {
            using var key = Registry.CurrentUser.OpenSubKey(WindowsCursorRegistryPath, writable: false);
            if (key is not null)
            {
                var registryPath = key.GetValue(assignment.WindowsReg)?.ToString();
                var cursorPath = ResolveWindowsCursorPath(registryPath, assignment.WindowsDefault);

                if (cursorPath is not null)
                {
                    var cursorType = _cursorService.GetCursorTypeFromFile(cursorPath);
                    var cursorBytes = await File.ReadAllBytesAsync(cursorPath);
                    var cursor = GetCursorFromBytes(cursorBytes, cursorType, assignment);
                    return cursor;
                }
            }
        }

        if (fallbackToDefault)
            return await GetDefaultCursorAsync(assignment);

        return null;
    }

    private static string? ResolveWindowsCursorPath(string? registryPath, string? windowsDefault)
    {
        var path = string.IsNullOrWhiteSpace(registryPath) ? null : registryPath;

        if (path is null || !File.Exists(path))
        {
            path = Path.Combine(WindowsCursorFolderPath, $"{windowsDefault}.cur");
            if (!File.Exists(path))
                path = Path.ChangeExtension(path, ".ani");
        }

        return File.Exists(path) ? path : null;
    }

    private static async Task<byte[]?> AssetLoaderTryGetAsset(string assetUri)
    {
        try
        {
            using var stream = AssetLoader.Open(new Uri($"avares://{nameof(Cursor_Installer_Creator)}/{assetUri.TrimStart("/")}"));
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
        catch (Exception ex) when (ex is FileNotFoundException or IOException)
        {
            return null;
        }
    }
}

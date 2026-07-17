using System.Diagnostics;
using System.Runtime.Versioning;

using Avalonia.Platform.Storage;

using Cursor_Installer_Creator.Data;
using Cursor_Installer_Creator.Extensions;
using Cursor_Installer_Creator.Repository.CursorRepo;
using Cursor_Installer_Creator.Repository.InfFileRepo;
using Cursor_Installer_Creator.Service.StorageServ;

using Microsoft.Extensions.Logging;

namespace Cursor_Installer_Creator.Service.InfServ;

public sealed class InfService : IInfService
{
    private readonly IInfFileRepository _infFileRepository;
    private readonly IStorageService _storageService;
    private readonly ICursorRepository _cursorRepository;
    private readonly ILogger<InfService> _logger;

    public InfService(IInfFileRepository infFileRepository, IStorageService storageService, ICursorRepository cursorRepository, ILogger<InfService> logger)
    {
        _infFileRepository = infFileRepository;
        _storageService = storageService;
        _cursorRepository = cursorRepository;
        _logger = logger;
    }

    public async Task LoadCursorsAsync(InfFile infFile, IStorageFile? infStorageFile = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(infFile);

        _logger.LogInformation("Loading cursors: isWindows={IsWindows}, hasStorageFile={HasStorageFile}", OperatingSystem.IsWindows(), infStorageFile is not null);

        if (OperatingSystem.IsWindows())
            await LoadCursorsFromFileSystemAsync(infFile, infStorageFile, cancellationToken);
        else
            await LoadCursorsViaFolderPickerAsync(infFile, infStorageFile, cancellationToken);
    }

    [SupportedOSPlatform("windows")]
    private async Task LoadCursorsFromFileSystemAsync(InfFile infFile, IStorageFile? infStorageFile, CancellationToken cancellationToken)
    {
        var localPath = infStorageFile?.TryGetLocalPath();
        if (string.IsNullOrWhiteSpace(localPath) || !File.Exists(localPath))
        {
            _logger.LogWarning("INF file local path not available or does not exist, falling back to folder picker");
            await LoadCursorsViaFolderPickerAsync(infFile, infStorageFile, cancellationToken);
            return;
        }

        var baseDir = Path.GetDirectoryName(localPath)!;
        _logger.LogDebug("Loading cursors from filesystem, baseDir={BaseDir}", baseDir);

        foreach (var cursor in infFile.Cursors)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(cursor.SourcePath))
                continue;

            var fullPath = Path.Combine(baseDir, cursor.SourcePath!);
            if (!File.Exists(fullPath))
            {
                _logger.LogDebug("Cursor file not found, skipping: {FullPath}", fullPath);
                continue;
            }

            var cursorFile = await _storageService.TryGetFileFromPathAsync(fullPath);
            if (cursorFile is null)
                continue;

            cursor.Cursor = await _cursorRepository.GetCursorFromFileAsync(cursorFile, cursor.Assignment);
            _logger.LogDebug("Loaded cursor: {SourcePath}", cursor.SourcePath);
        }
    }

    private async Task LoadCursorsViaFolderPickerAsync(InfFile infFile, IStorageFile? infStorageFile, CancellationToken cancellationToken)
    {
        if (!_storageService.CanOpenFolders)
            throw new InvalidOperationException("The storage provider cannot pick folders.");

        _logger.LogDebug("Prompting user to select cursor folder via folder picker");

        var options = new FolderPickerOpenOptions
        {
            Title = $"Select folder containing '{infStorageFile?.Name ?? "INF file"}'",
            AllowMultiple = false,
        };
        if (infStorageFile is not null)
        {
            options.SuggestedStartLocation = await infStorageFile.GetParentAsync();
            _logger.LogDebug($"Setting folder picker Location to '{options.SuggestedStartLocation}'");
        }

        var folders = await _storageService.OpenFolderPickerAsync(options);
        if (folders is null || folders.Count == 0)
        {
            _logger.LogWarning("Folder picker cancelled or returned no folders; cursors will not be loaded");
            return;
        }

        var folder = folders[0];
        _logger.LogDebug("Folder selected: {FolderName}", folder.Name);

        foreach (var cursor in infFile.Cursors)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(cursor.SourcePath))
                continue;

            var cursorFile = await NavigateToFileAsync(folder, cursor.SourcePath!);
            if (cursorFile is null)
            {
                _logger.LogDebug("Cursor file not found in selected folder: {SourcePath}", cursor.SourcePath);
                continue;
            }

            cursor.Cursor = await _cursorRepository.GetCursorFromFileAsync(cursorFile, cursor.Assignment);
            _logger.LogDebug("Loaded cursor: {SourcePath}", cursor.SourcePath);
        }
    }

    private static async Task<IStorageFile?> NavigateToFileAsync(IStorageFolder root, string relativePath)
    {
        var segments = relativePath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return null;

        var current = root;
        for (var i = 0; i < segments.Length - 1; i++)
        {
            var sub = await current.GetFolderAsync(segments[i], StringComparison.OrdinalIgnoreCase);
            if (sub is null)
                return null;
            current = sub;
        }

        return await current.GetFileAsync(segments[^1], StringComparison.OrdinalIgnoreCase);
    }

    public void UpdateCursors(InfFile infFile, IEnumerable<ICursor?> cursors)
    {
        ArgumentNullException.ThrowIfNull(infFile);
        ArgumentNullException.ThrowIfNull(cursors);

        var cursorDict = cursors.Where(x => x?.Assignment is not null).ToDictionary(c => c!.Assignment!);
        foreach (var entry in infFile.Cursors)
        {
            if (entry.Assignment is null)
                continue;

            if (cursorDict.TryGetValue(entry.Assignment, out var cursor))
            {
                entry.Cursor = cursor;
            }
        }
    }

    [SupportedOSPlatform("windows")]
    public async Task InstallCursorPackage(InfFile infFile, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(infFile);

        _logger.LogInformation("Installing cursor package: {SchemeName}", infFile.SchemeName);

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var tempStorageFolder = await _storageService.StorageProvider.TryGetFolderFromPathAsync(tempDir)
                ?? throw new InvalidOperationException($"Could not resolve temp directory as a storage folder: {tempDir}");

            await _infFileRepository.WriteInfPackageAsync(infFile, tempStorageFolder, cancellationToken: cancellationToken);

            var rundllPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "rundll32.exe");
            var arguments = $"setupapi,InstallHinfSection DefaultInstall 132 .\\{Constants.DefaultInstallerFileName}";

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{rundllPath} {arguments}\"",
                WorkingDirectory = tempDir,
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };

            try
            {
                process.Start();

                try
                {
                    await process.WaitForExitAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    process.Kill(entireProcessTree: true);
                    throw;
                }

                if (process.ExitCode != 0)
                {
                    _logger.LogWarning("rundll32 exited with non-zero code {ExitCode} for {SchemeName}", process.ExitCode, infFile.SchemeName);
                    throw new InvalidOperationException($"Cursor installation failed (rundll32 exit code {process.ExitCode}).");
                }

                _logger.LogInformation("Cursor package installed successfully: {SchemeName}", infFile.SchemeName);
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                _logger.LogWarning("Cursor package installation cancelled by user");
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                _logger.LogError(ex, "Cursor package installation failed: {SchemeName}", infFile.SchemeName);
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
        finally
        {
            try
            {
                Directory.Delete(tempDir, recursive: true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete temp directory after installation: {TempDir}", tempDir);
            }
        }
    }
}

using Avalonia.Platform.Storage;

using Cursor_Installer_Creator.Service.TopLevelServ;

using Microsoft.Extensions.Logging;

namespace Cursor_Installer_Creator.Service.StorageServ;

public sealed class StorageService : IStorageService
{
    public IStorageProvider StorageProvider => field ??= _topLevelProvider.GetTopLevel().StorageProvider;
    public bool CanOpenFiles => StorageProvider.CanOpen;
    public bool CanOpenFolders => StorageProvider.CanPickFolder;

    private readonly ITopLevelProvider _topLevelProvider;
    private readonly ILogger<StorageService> _logger;

    public StorageService(ITopLevelProvider topLevelProvider, ILogger<StorageService> logger)
    {
        _topLevelProvider = topLevelProvider;
        _logger = logger;
    }

    public async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
    {
        _logger.LogDebug("Opening file picker: title={Title}", options.Title);
        var result = await StorageProvider.OpenFilePickerAsync(options);
        _logger.LogDebug("File picker returned {Count} file(s)", result.Count);
        return result;
    }

    public async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
    {
        _logger.LogDebug("Opening folder picker: title={Title}", options.Title);
        var result = await StorageProvider.OpenFolderPickerAsync(options);
        _logger.LogDebug("Folder picker returned {Count} folder(s)", result.Count);
        return result;
    }

    public Task<IStorageFile?> TryGetFileFromPathAsync(string absolutePath)
        => StorageProvider.TryGetFileFromPathAsync(new Uri(absolutePath));
}

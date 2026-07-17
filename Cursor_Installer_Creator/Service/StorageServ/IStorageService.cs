using Avalonia.Platform.Storage;

namespace Cursor_Installer_Creator.Service.StorageServ;

public interface IStorageService
{
    IStorageProvider StorageProvider { get; }
    bool CanOpenFiles { get; }
    bool CanOpenFolders { get; }
    Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options);
    Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options);
    Task<IStorageFile?> TryGetFileFromPathAsync(string absolutePath);
}

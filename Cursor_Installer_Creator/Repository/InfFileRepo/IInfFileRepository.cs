using Avalonia.Platform.Storage;

using Cursor_Installer_Creator.Data;

namespace Cursor_Installer_Creator.Repository.InfFileRepo;

public interface IInfFileRepository
{
    Task<InfFile?> GetInfFileFromFileAsync(IStorageFile file, CancellationToken cancellationToken = default);
    Task<InfFile?> GetInfFileFromBytesAsync(byte[] data, CancellationToken cancellationToken = default);
    Task<InfFile?> GetInfFileFromTextAsync(string data, CancellationToken cancellationToken = default);
    Task AddMissingCursorEntriesAsync(InfFile infFile);
    Task WriteInfFileAsync(InfFile infFile, IStorageFile destination);
    Task WriteInfPackageAsync(InfFile infFile, IStorageFolder destination, bool archive = false, CancellationToken cancellationToken = default);
    Task WriteInfPackageArchiveAsync(InfFile infFile, IStorageFolder destination, CancellationToken cancellationToken = default);
    Task WriteInfPackageInstallerAsync(InfFile infFile, IStorageFolder destination, CancellationToken cancellationToken = default);
}

using Avalonia.Platform.Storage;

using Cursor_Installer_Creator.Data;

namespace Cursor_Installer_Creator.Service.InfServ;

public interface IInfService
{
    Task LoadCursorsAsync(InfFile infFile, IStorageFile? infStorageFile = null, CancellationToken cancellationToken = default);
    void UpdateCursors(InfFile infFile, IEnumerable<ICursor?> cursors);
    Task InstallCursorPackage(InfFile infFile, CancellationToken cancellationToken = default);
}

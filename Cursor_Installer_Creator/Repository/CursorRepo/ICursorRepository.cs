using System.Runtime.Versioning;

using Avalonia.Platform.Storage;

using Cursor_Installer_Creator.Data;

namespace Cursor_Installer_Creator.Repository.CursorRepo;

public interface ICursorRepository
{
    Task<ICursor?> GetCursorFromFileAsync(IStorageFile file, CursorAssignment? assignment = null);
    ICursor? GetCursorFromBytes(byte[] data, CursorType type, CursorAssignment? assignment = null);
    Task<ICursor[]> GetDefaultCursorsAsync();
    Task<ICursor?> GetDefaultCursorAsync(CursorAssignment assignment);
    Task<ICursor[]> AddMissingCursorsAsync(IEnumerable<ICursor?> cursors);

    [SupportedOSPlatform("windows")]
    Task<ICursor[]> GetCurrWindowsCursorsAsync(bool fallbackToDefault = false);

    [SupportedOSPlatform("windows")]
    Task<ICursor?> GetCurrWindowsCursorAsync(CursorAssignment assignment, bool fallbackToDefault = false);
}

using Cursor_Installer_Creator.Data;

namespace Cursor_Installer_Creator.Service.CursorServ;

public interface ICursorService
{
    CursorType GetCursorTypeFromFile(string filePath);
    bool IsValidCursorFile(string fileName);
}

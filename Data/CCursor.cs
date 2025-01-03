using Cursor_Installer_Creator.Utils;
using System.IO;

namespace Cursor_Installer_Creator.Data;

public enum CCursorType
{
    cur,
    ani,
    unknown,
}

public sealed class CCursor
{
    public required CursorAssignment Assignment { get; set; }
    public required string CursorPath { get; set; }

    public CCursorType GetCursorType()
    {
        if (string.IsNullOrWhiteSpace(CursorPath))
            return CCursorType.unknown;

        if (CursorPath.EndsWith(".ani"))
            return CCursorType.ani;
        else if (CursorPath.EndsWith(".cur"))
            return CCursorType.cur;

        return CCursorType.unknown;
    }

    public string? GetImagePath()
    {
        var imagePath = Path.Combine(Program.TempPath, $"{Assignment.WindowsReg}.png");
        if (File.Exists(imagePath))
            return imagePath;

        imagePath = CursorHelper.CreateCursorImage(this);
        return File.Exists(imagePath) ? imagePath : null;
    }
}

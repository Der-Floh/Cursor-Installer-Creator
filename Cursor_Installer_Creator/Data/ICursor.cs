namespace Cursor_Installer_Creator.Data;

public interface ICursor
{
    byte[] CursorBytes { get; }
    CursorType Type { get; }
    string FileExtension { get; }
    CursorAssignment? Assignment { get; set; }

    Task<CursorAnimationFrame[]> GetCursorFramesAsync(int targetSize = 0);
}

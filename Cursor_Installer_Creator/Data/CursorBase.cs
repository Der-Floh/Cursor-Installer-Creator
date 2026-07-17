using CommunityToolkit.Mvvm.ComponentModel;

namespace Cursor_Installer_Creator.Data;

public abstract partial class CursorBase : ObservableObject, ICursor
{
    [ObservableProperty]
    public partial byte[] CursorBytes { get; set; }

    [ObservableProperty]
    public partial CursorAssignment? Assignment { get; set; }

    [ObservableProperty]
    public partial CursorType Type { get; set; }

    public string FileExtension => Type.ToString().ToLowerInvariant();

    protected CursorBase(byte[] cursorBytes, CursorType type, CursorAssignment? assignment = null)
    {
        CursorBytes = cursorBytes;
        Type = type;
        Assignment = assignment;
    }

    public abstract Task<CursorAnimationFrame[]> GetCursorFramesAsync(int targetSize = 0);

    public override string ToString()
        => $"{Assignment?.ToString() ?? "Unknown"} ({Type})";
}

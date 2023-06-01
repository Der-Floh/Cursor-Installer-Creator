using System.Runtime.InteropServices;

namespace Cursor_Installer_Creator;
public class AdvancedCursors
{

    [DllImport("User32.dll")]
    private static extern IntPtr LoadCursorFromFile(String str);

    public static Cursor Create(string filename)
    {
        IntPtr hCursor = LoadCursorFromFile(filename);

        if (!IntPtr.Zero.Equals(hCursor))
        {
            return new Cursor(hCursor);
        }
        else
        {
            throw new ApplicationException("Could not create cursor from file " + filename);
        }
    }
}

using System.Text.Json;

namespace Cursor_Installer_Creator;

internal static class Program
{
    public static string TempPath { get; private set; } = Path.Combine(Path.GetTempPath(), "CursorTemp");
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        if (args.Length != 0)
        {
            string json = args[0];
            List<CCursor>? cCursors = JsonSerializer.Deserialize<List<CCursor>>(json);
            Application.Run(new MainForm(cCursors));
        }
        else
            Application.Run(new MainForm());
    }
}
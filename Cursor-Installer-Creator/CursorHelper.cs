using Microsoft.Win32;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Cursor_Installer_Creator;
public sealed class CursorHelper
{
    public static List<CCursor> GetSelectedCursors()
    {
        var cursors = new List<CCursor>();

        using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors"))
        {
            if (key is not null)
            {
                string[] valueNames = key.GetValueNames();
                foreach (string valueName in valueNames)
                {
                    string? cursorPath = key.GetValue(valueName)?.ToString();
                    if (string.IsNullOrEmpty(cursorPath) || !File.Exists(cursorPath))
                        cursorPath = "C:/Windows/Cursors/" + GetDefaultCursorName(valueName);

                    if (File.Exists(cursorPath) && ConvertCursorFile(cursorPath, valueName))
                    {
                        CCursor cursor = new CCursor()
                        {
                            Name = valueName,
                            CursorName = Path.GetFileName(cursorPath),
                            CursorPath = cursorPath,
                            ImagePath = Path.Combine(Program.TempPath, $"{valueName}.png")
                        };
                        cursors.Add(cursor);
                    }
                }
            }
        }

        return cursors;
    }

    public static string? GetSelectedCursorPath(string name)
    {
        using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors"))
        {
            if (key is not null)
            {
                string[] valueNames = key.GetValueNames();
                if (valueNames is not null && valueNames.Length != 0)
                {
                    string? valueName = valueNames.FirstOrDefault(x => x == name);
                    if (string.IsNullOrEmpty(valueName))
                        return null;

                    string? cursorPath = key.GetValue(valueName)?.ToString();
                    if (string.IsNullOrEmpty(cursorPath) || !File.Exists(cursorPath))
                        cursorPath = "C:/Windows/Cursors/" + GetDefaultCursorName(valueName);

                    return cursorPath;
                }
            }
        }

        return null;
    }

    private static string GetDefaultCursorName(string name)
    {
        switch (name)
        {
            case "AppStarting":
                return "wait_m.cur";
            case "Arrow":
                return "arrow_m.cur";
            case "Crosshair":
                return "cross_im.cur";
            case "Hand":
                return "aero_link_im.cur";
            case "Help":
                return "help_m.cur";
            case "IBeam":
                return "beam_im.cur";
            case "No":
                return "no_m.cur";
            case "NWPen":
                return "pen_m.cur";
            case "SizeAll":
                return "move_m.cur";
            case "SizeNESW":
                return "lnesw.cur";
            case "SizeNS":
                return "lns.cur";
            case "SizeNWSE":
                return "lnwse.cur";
            case "SizeWE":
                return "lwe.cur";
            case "UpArrow":
                return "up_m.cur";
            case "Wait":
                return "busy_m.cur";
            case "Person":
                return "person_m.cur";
            case "Pin":
                return "pin_m.cur";
        }
        return "";
    }

    public static bool ConvertCursorFile(string cursorPath, string cursorName, bool copyOrgiginal = false)
    {
        try
        {
            Cursor cursor = GetCursorFromFile(cursorPath);
            Icon icon = Icon.FromHandle(cursor.Handle);
            Bitmap bitmap = icon.ToBitmap();

            if (!Directory.Exists(Program.TempPath))
                Directory.CreateDirectory(Program.TempPath);

            if (copyOrgiginal)
            {
                string destinationDirectory = Program.TempPath;
                string destinationFileName = Path.ChangeExtension($"{cursorName}.png", Path.GetExtension(cursorPath));
                string destinationFullPath = Path.Combine(destinationDirectory, destinationFileName);
                File.Copy(cursorPath, destinationFullPath, true);
            }

            bitmap.Save(Path.Combine(Program.TempPath, $"{cursorName}.png"), System.Drawing.Imaging.ImageFormat.Png);

            return true;
        }
        catch
        {
            return false;
        }
    }

    [DllImport("User32.dll")]
    private static extern IntPtr LoadCursorFromFile(string str);
    private static Cursor GetCursorFromFile(string filename)
    {
        IntPtr hCursor = LoadCursorFromFile(filename);
        if (!IntPtr.Zero.Equals(hCursor))
            return new Cursor(hCursor);
        else
            throw new ApplicationException("Could not create cursor from file " + filename);
    }

    public static void CreateInstaller(string packageName, string folderPath, IEnumerable<CCursor> ccursors, bool createZip = true)
    {
        using (StreamWriter writer = new StreamWriter($"{Program.TempPath}/installer.inf"))
        {
            writer.WriteLine($" ; {packageName}");
            writer.WriteLine("[Version]");
            writer.WriteLine("Signature = \"$WINDOWS NT$\"");
            writer.WriteLine("Provider = Der_Floh");
            writer.WriteLine("Class = Cursor");
            writer.WriteLine("ClassGuid = {some-guid}");
            writer.WriteLine("CatalogFile = installer.cat");
            writer.WriteLine();
            writer.WriteLine("[DefaultInstall]");
            writer.WriteLine("CopyFiles = Scheme.Cur, Scheme.Txt");
            writer.WriteLine("AddReg = Scheme.Reg");
            writer.WriteLine();
            writer.WriteLine("[DestinationDirs]");
            writer.WriteLine("Scheme.Cur = 10,\"%CUR_DIR%\"");
            writer.WriteLine("Scheme.Txt = 10,\"%CUR_DIR%\"");
            writer.WriteLine();
            writer.WriteLine("[Scheme.Reg]");
            writer.WriteLine(@"HKCU,""Control Panel\Cursors\Schemes"",""%SCHEME_NAME%"",,""%10%\%CUR_DIR%\%pointer%,%10%\%CUR_DIR%\%help%,%10%\%CUR_DIR%\%work%,%10%\%CUR_DIR%\%busy%,%10%\%CUR_DIR%\%Cross%,%10%\%CUR_DIR%\%Text%,%10%\%CUR_DIR%\%Hand%,%10%\%CUR_DIR%\%Unavailiable%,%10%\%CUR_DIR%\%Vert%,%10%\%CUR_DIR%\%Horz%,%10%\%CUR_DIR%\%Dgn1%,%10%\%CUR_DIR%\%Dgn2%,%10%\%CUR_DIR%\%move%,%10%\%CUR_DIR%\%alternate%,%10%\%CUR_DIR%\%link%,%10%\%CUR_DIR%\%pin%,%10%\%CUR_DIR%\%person%""");
            writer.WriteLine();

            writer.WriteLine("[Scheme.Cur]");
            foreach (CCursor ccursor in ccursors)
            {
                writer.WriteLine("\"" + ccursor.CursorName + "\"");
            }
            writer.WriteLine();

            writer.WriteLine("[Strings]");
            writer.WriteLine("CUR_DIR = \"Cursors\\" + packageName + "\"");
            writer.WriteLine("SCHEME_NAME = \"" + packageName + "\"");
            foreach (string cursorAssignment in GetWindowsCursorAssignments(ccursors))
            {
                writer.WriteLine(cursorAssignment);
            }
        }

        List<string?> files = ccursors.Select(c => c.CursorPath).ToList();
        files.Add($"{Program.TempPath}/installer.inf");

        if (createZip)
        {
            string zipPath = Path.Combine(folderPath, $"{packageName}.zip");
            CreateZipFile(zipPath, packageName, files);
        }
        else
        {
            folderPath = Path.Combine(folderPath, packageName);
            Directory.CreateDirectory(folderPath);
            foreach (string file in files)
            {
                File.Copy(file, Path.Combine(folderPath, Path.GetFileName(file)), true);
            }
        }
    }

    public static void CreateZipFile(string zipPath, string folder, IEnumerable<string> files)
    {


        if (File.Exists(zipPath) && MessageBox.Show($"{Path.GetFileName(zipPath)} already exists. Do you want to overwrite it?", "File already Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            File.Delete(zipPath);
        using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            foreach (string file in files)
            {
                string filePath = Path.Combine(folder, Path.GetFileName(file));
                archive.CreateEntryFromFile(file, filePath);
            }
        }
    }

    public static IEnumerable<string> GetWindowsCursorAssignments(IEnumerable<CCursor> ccursors)
    {
        List<string> result = new List<string>();

        foreach (CCursor cursor in ccursors)
        {
            result.Add($"{TransformCursorName(cursor.Name)} = \"{cursor.CursorName}\"");
        }

        return result;
    }

    public static string? TransformCursorName(string name)
    {
        Dictionary<string, string> windowsStandardDict = new Dictionary<string, string>
        {
            { "pointer", "Arrow" },
            { "help", "Help" },
            { "work", "AppStarting" },
            { "busy", "Wait" },
            { "cross", "Crosshair" },
            { "text", "IBeam" },
            { "hand", "NWPen" },
            { "unavailiable", "No" },
            { "vert", "SizeNS" },
            { "horz", "SizeWE" },
            { "dgn1", "SizeNWSE" },
            { "dgn2", "SizeNESW" },
            { "move", "SizeAll" },
            { "alternate", "UpArrow" },
            { "link", "Hand" },
            { "pin", "Pin" },
            { "person", "Person" },
        };
        Dictionary<string, string> standardWindowsDict = windowsStandardDict.ToDictionary(x => x.Value, x => x.Key);

        windowsStandardDict.TryGetValue(name, out string? result);
        if (result is null)
            standardWindowsDict.TryGetValue(name, out result);

        return result;
    }

    public static void InstallCursor(string installerFilePath)
    {
        string command = @"C:\WINDOWS\System32\rundll32.exe";
        string arguments = "setupapi,InstallHinfSection DefaultInstall 132 " + installerFilePath;

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/k \"{command} {arguments}\"",
            Verb = "runas",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process process = new Process
        {
            StartInfo = startInfo
        };

        try
        {
            process.Start();
            process.WaitForExit();
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            if (ex.NativeErrorCode != 1223)
            {
                MessageBox.Show(ex.Message, "Error " + ex.ErrorCode, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

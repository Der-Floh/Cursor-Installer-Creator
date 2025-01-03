using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Cursor = System.Windows.Forms.Cursor;

namespace Cursor_Installer_Creator;

public static class CursorHelper
{
    public static List<CCursor> GetSelectedCursors()
    {
        var ccursors = new List<CCursor>();

        var assignmentsAll = CursorAssignment.CursorAssignments.Values.ToList();
        using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors");
        if (key is not null)
        {
            // Search Registry for cursors
            var valueNames = key.GetValueNames();
            foreach (var valueName in valueNames)
            {
                var cursorPath = key.GetValue(valueName)?.ToString();
                if (string.IsNullOrEmpty(cursorPath) || !File.Exists(cursorPath))
                {
                    var assignment = CursorAssignment.FromName(valueName, CursorAssignmentType.WindowsReg);
                    cursorPath = $"C:/Windows/Cursors/{assignment?.Windows}.cur";
                }

                if (File.Exists(cursorPath))
                {
                    var ccursor = ConvertCursorFile(cursorPath, valueName);
                    if (ccursor is not null)
                    {
                        ccursors.Add(ccursor);
                    }
                }
            }

            // Add missing cursors from CursorAssignment
            foreach (var assignment in assignmentsAll)
            {
                var cursorAssignment = CursorAssignment.FromName(assignment.WindowsReg, CursorAssignmentType.WindowsReg);
                var cursorPath = $"C:/Windows/Cursors/{cursorAssignment?.Windows}.cur";
                if (File.Exists(cursorPath))
                {
                    var ccursor = ConvertCursorFile(cursorPath, assignment.WindowsReg);
                    if (ccursor is not null)
                    {
                        ccursors.Add(ccursor);
                    }
                }
            }
        }

        foreach (var assignment in CursorAssignment.CursorAssignments)
        {
            var ccursor = ccursors.FirstOrDefault(x => x.Assignment.ID == assignment.Key);
            if (ccursor is null)
            {
                ccursor = new CCursor
                {
                    Assignment = assignment.Value,
                    CursorPath = @$"C:\Windows\Cursors\{assignment.Value.Windows}.cur",
                };
                ccursor.CursorPath = ConvertCursorFile(ccursor.CursorPath, ccursor.Assignment)?.CursorPath ?? ccursor.CursorPath;
                if (File.Exists(ccursor.CursorPath))
                {
                    ccursors.Add(ccursor);
                }
            }
        }

        return [.. ccursors.OrderBy(x => x.Assignment.ID)];
    }

    public static CCursor? GetSelectedCursor(CursorAssignment assignment)
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Cursors");
        if (key is not null)
        {
            var cursorPath = key.GetValue(assignment.WindowsReg)?.ToString();
            if (!File.Exists(cursorPath))
            {
                cursorPath = @$"C:\Windows\Cursors\{assignment.Windows}.cur";
            }

            return ConvertCursorFile(cursorPath, assignment.ID);
        }

        return null;
    }

    private static Dictionary<string, string> ParseInstallerInfStrings(string filePath)
    {
        var stringsDictionary = new Dictionary<string, string>();
        var lines = File.ReadAllLines(filePath);
        var isStringsSection = false;

        foreach (var line in lines)
        {
            if (line.Trim().Equals("[Strings]", StringComparison.OrdinalIgnoreCase))
            {
                isStringsSection = true;
                continue;
            }

            if (isStringsSection)
            {
                var parts = line.Split('=');

                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim().TrimStart('\"').TrimEnd('\"');
                    var assignment = CursorAssignment.FromName(key, CursorAssignmentType.WindowsInstall);
                    if (assignment is not null)
                    {
                        stringsDictionary[assignment.WindowsReg] = value;
                    }
                }
            }
        }

        return stringsDictionary;
    }

    public static IEnumerable<CCursor> CursorsFromInstallerInf(string filePath)
    {
        var ccursors = new List<CCursor>();
        var cursorDictionary = ParseInstallerInfStrings(filePath);
        foreach (var kvp in cursorDictionary)
        {
            var assignment = CursorAssignment.FromName(kvp.Key, CursorAssignmentType.WindowsReg);
            if (assignment is not null)
            {
                var ccursor = ConvertCursorFile(Path.Combine(Path.GetDirectoryName(filePath)!, kvp.Value), assignment);
                if (ccursor is not null)
                {
                    ccursors.Add(ccursor);
                }
            }
        }
        return ccursors;
    }

    public static string? CreateCursorImage(CCursor ccursor)
    {
        try
        {
            var cursor = GetCursorFromFile(ccursor.CursorPath);
            var icon = Icon.FromHandle(cursor.Handle);
            var bitmap = icon.ToBitmap();

            if (!Directory.Exists(Program.TempPath))
                Directory.CreateDirectory(Program.TempPath);

            var imagePath = Path.Combine(Program.TempPath, $"{ccursor.Assignment.WindowsReg}.png");
            bitmap.Save(imagePath, ImageFormat.Png);

            return imagePath;
        }
        catch
        {
            return null;
        }
    }

    public static void RemoveCursorDisplayImage(CCursor ccursor)
    {
        var prevCursorImagePath = Path.ChangeExtension(ccursor.CursorPath, ".png");
        if (File.Exists(prevCursorImagePath))
        {
            File.Delete(prevCursorImagePath);
        }
    }

    public static CCursor? ConvertCursorFile(string cursorPath, int cursorID) => ConvertCursorFile(cursorPath, CursorAssignment.CursorAssignments[cursorID]);
    public static CCursor? ConvertCursorFile(string cursorPath, string cursorName) => ConvertCursorFile(cursorPath, CursorAssignment.FromName(cursorName, CursorAssignmentType.WindowsReg));

    private static CCursor? ConvertCursorFile(string cursorPath, CursorAssignment? assignment)
    {
        try
        {
            if (assignment is null)
                return null;
            var cursor = GetCursorFromFile(cursorPath);

            if (!Directory.Exists(Program.TempPath))
                Directory.CreateDirectory(Program.TempPath);

            var destinationFileName = assignment.WindowsReg + Path.GetExtension(cursorPath);
            var destinationFullPath = Path.Combine(Program.TempPath, destinationFileName);
            File.Copy(cursorPath, destinationFullPath, true);

            var prevCursorImagePath = Path.ChangeExtension(destinationFullPath, ".png");
            if (File.Exists(prevCursorImagePath))
            {
                File.Delete(prevCursorImagePath);
            }

            var ccursor = new CCursor
            {
                CursorPath = destinationFullPath,
                Assignment = assignment,
            };

            return ccursor;
        }
        catch
        {
            return null;
        }
    }

    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadCursorFromFile(string str);
    private static Cursor GetCursorFromFile(string filename)
    {
        var hCursor = LoadCursorFromFile(filename);
        return !IntPtr.Zero.Equals(hCursor)
            ? new Cursor(hCursor)
            : throw new ApplicationException("Could not create cursor from file " + filename);
    }

    public static void CreateInstaller(string packageName, string folderPath, IEnumerable<CCursor> ccursors, bool createZip = true)
    {
        using (var writer = new StreamWriter($"{Program.TempPath}/installer.inf"))
        {
            writer.Write(CreateInstallerInfString(packageName, ccursors));
        }

        var files = ccursors.Select(x => x.CursorPath).ToList();
        files.Add($"{Program.TempPath}/installer.inf");

        if (createZip)
        {
            var zipPath = Path.Combine(folderPath, $"{packageName}.zip");
            CreateZipFile(zipPath, files);
        }
        else
        {
            folderPath = Path.Combine(folderPath, packageName);
            Directory.CreateDirectory(folderPath);
            foreach (var file in files)
            {
                File.Copy(file, Path.Combine(folderPath, Path.GetFileName(file)), true);
            }
        }
    }

    private static string CreateInstallerInfString(string packageName, IEnumerable<CCursor> ccursors)
    {
        var sb = new StringBuilder();
        ccursors = ccursors.OrderBy(x => x.Assignment.ID);

        sb.AppendLine($" ; {packageName}");
        sb.AppendLine("[Version]");
        sb.AppendLine("Signature = \"$WINDOWS NT$\"");
        sb.AppendLine("Provider = Der_Floh");
        sb.AppendLine("Class = Cursor");
        sb.AppendLine("ClassGuid = {some-guid}");
        sb.AppendLine("CatalogFile = installer.cat");
        sb.AppendLine();
        sb.AppendLine("[DefaultInstall]");
        sb.AppendLine("CopyFiles = Scheme.Cur, Scheme.Txt");
        sb.AppendLine("AddReg = Scheme.Reg");
        sb.AppendLine();
        sb.AppendLine("[DestinationDirs]");
        sb.AppendLine("Scheme.Cur = 10,\"%CUR_DIR%\"");
        sb.AppendLine("Scheme.Txt = 10,\"%CUR_DIR%\"");
        sb.AppendLine();
        sb.AppendLine("[Scheme.Reg]");
        sb.AppendLine(@$"HKCU,""Control Panel\Cursors\Schemes"",""%SCHEME_NAME%"",,""{string.Join(',', ccursors.OrderBy(x => x.Assignment.Order).Select(x => @$"%10%\%CUR_DIR%\%{x.Assignment.WindowsInstall}%"))}""");
        sb.AppendLine();

        sb.AppendLine("[Scheme.Cur]");
        foreach (var ccursor in ccursors.OrderBy(x => x.Assignment.ID))
        {
            var extension = Path.GetExtension(ccursor.CursorPath);
            sb.AppendLine($"\"{ccursor.Assignment.WindowsReg}{extension}\"");
        }
        sb.AppendLine();

        sb.AppendLine("[Strings]");
        sb.AppendLine($"CUR_DIR      = \"Cursors\\{packageName}\"");
        sb.AppendLine($"SCHEME_NAME  = \"{packageName}\"");
        foreach (var ccursor in ccursors.OrderBy(x => x.Assignment.ID))
        {
            var extension = Path.GetExtension(ccursor.CursorPath);
            var spaceCount = 12 - ccursor.Assignment.WindowsInstall.ToCharArray().Length;
            sb.AppendLine($"{ccursor.Assignment.WindowsInstall}{new string(' ', spaceCount + 1)}= \"{ccursor.Assignment.WindowsReg}{extension}\"");
        }

        return sb.ToString();
    }

    private static void CreateZipFile(string zipPath, IEnumerable<string> files)
    {
        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }
        using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
        var folder = Path.GetFileNameWithoutExtension(zipPath);
        foreach (var file in files)
        {
            var filePath = Path.Combine(folder, Path.GetFileName(file));
            archive.CreateEntryFromFile(file, filePath);
        }
    }

    public static void InstallCursor(string installerFilePath)
    {
        var command = @"C:\WINDOWS\System32\rundll32.exe";
        var arguments = "setupapi,InstallHinfSection DefaultInstall 132 " + installerFilePath;

        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"{command} {arguments}\"",
            Verb = "runas",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        var process = new Process
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
                throw new Exception(ex.Message + Environment.NewLine + ex.ErrorCode);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Cursor_Installer_Creator;

public sealed class InnoSetupCompiler
{
    public string AppName { get; set; } = "My Custom Cursor";
    public List<string> Files { get; set; } = [];
    public string FileToExecute { get; set; } = string.Empty;

    public bool CreateInstaller(string outputPath)
    {
        // Define the temporary installation folder (Windows Temp folder)
        var tempInstallDir = "{tmp}";

        // Define the Inno Setup script content
        var innoSetupScript = $@"
[Setup]
AppName={AppName}
AppVersion=1
DefaultDirName={tempInstallDir}\{AppName}
DefaultGroupName={AppName}
OutputDir={Path.GetDirectoryName(outputPath)}
OutputBaseFilename={Path.GetFileNameWithoutExtension(outputPath)}
DisableProgramGroupPage=yes
DisableDirPage=yes
SetupIconFile={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "SetupIcon.ico")}
Uninstallable=no
PrivilegesRequired=admin

[Files]
";

        // Add each file to the script
        foreach (var file in Files)
        {
            innoSetupScript += $@"Source: ""{file}""; DestDir: ""{tempInstallDir}\{AppName}""; Flags: ignoreversion
";
        }

        // Add the post-installation task
        if (!string.IsNullOrEmpty(FileToExecute))
        {
            innoSetupScript += $@"
[Run]
Filename: ""{tempInstallDir}\{AppName}\{Path.GetFileName(FileToExecute)}""; Flags: postinstall runascurrentuser
";
        }

        //        innoSetupScript += @"
        //[Code]
        //procedure CurPageChanged(CurPageID: Integer);
        //begin
        //  if CurPageID = wpFinished then
        //    WizardForm.RunList.Visible := False;
        //end;
        //";

        // Define the path where the script will be saved
        var scriptPath = Path.Combine(Program.TempPath, $"{AppName}-InstallerScript.iss");

        // Save the script to a file
        File.WriteAllText(scriptPath, innoSetupScript);

        // Path to the Inno Setup Compiler executable (ISCC.exe) in the Tools folder
        var innoSetupCompilerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "ISCC.exe");

        // Create a process to run the Inno Setup Compiler with the script
        var processStartInfo = new ProcessStartInfo
        {
            FileName = innoSetupCompilerPath,
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processStartInfo };
        process.Start();
        process.WaitForExit();

        // Check the exit code to determine if the process succeeded
        return process.ExitCode == 0;
    }
}

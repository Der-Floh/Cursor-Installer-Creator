using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cursor_Installer_Creator.Views;

public partial class CursorInstallerMainView : UserControl
{
    public CursorInstallerMainView()
    {
        InitializeComponent();

        CursorListViewElem.OnImportInfFile += CursorListViewElem_OnImportInfFile;
    }

    private void CreateCursorInstaller(string packageName, string location)
    {
        throw new NotImplementedException("Not yet implemented");
    }

    private async Task<Uri?> PickFolderLocation()
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Pick Cursor File",
            AllowMultiple = false,
        });

        return files is null || files.Count == 0 ? null : files[0].Path;
    }

    private void CursorListViewElem_OnImportInfFile(object? sender, string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        if (lines.Length != 0)
        {
            var parts = lines[0].Split(';');
            if (parts.Length >= 1)
            {
                var packageName = parts[parts.Length - 1].Trim();
                CursorPackagenameTextBox.Text = packageName;
            }
        }
    }

    private void CursorInstallButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var packageName = CursorPackagenameTextBox.Text;
        if (string.IsNullOrEmpty(packageName))
            packageName = CursorPackagenameTextBox.Watermark!;

        CursorHelper.CreateInstaller(packageName, Program.TempPath, CursorListViewElem.Cursors.Select(x => x.CCursor), false);
        var installerPath = Path.Combine(Program.TempPath, packageName, "installer.inf");
        CursorHelper.InstallCursor(installerPath);
    }

    private async void CreateCursorPackageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateCursorPackageButton.IsEnabled = false;
        var packageName = CursorPackagenameTextBox.Text;
        if (string.IsNullOrEmpty(packageName))
            packageName = CursorPackagenameTextBox.Watermark!;

        var location = await PickFolderLocation();
        if (location is null)
            return;

        var cursors = CursorListViewElem.Cursors.Select(x => x.CCursor);
        switch (CursorPackageTypeComboBox.SelectedIndex)
        {
            case 0:
                CursorHelper.CreateInstaller(packageName, Uri.UnescapeDataString(location.AbsolutePath), cursors, false);
                break;
            case 1:
                CursorHelper.CreateInstaller(packageName, Uri.UnescapeDataString(location.AbsolutePath), cursors, true);
                break;
            case 2:
                CreateCursorInstaller(packageName, Uri.UnescapeDataString(location.AbsolutePath));
                break;
        }
        CreateCursorPackageButton.IsEnabled = true;
        OperationSuccessTextBlock.IsVisible = true;
        await Task.Delay(3000);
        OperationSuccessTextBlock.IsVisible = false;
    }
}
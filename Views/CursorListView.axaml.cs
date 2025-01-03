using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Cursor_Installer_Creator.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserControl = Avalonia.Controls.UserControl;

namespace Cursor_Installer_Creator.Views;

public partial class CursorListView : UserControl
{
    public event EventHandler<string> OnImportInfFile;
    public List<CursorItemView> Cursors { get; set; } = [];

    public CursorListView()
    {
        InitializeComponent();

        var cursors = CursorHelper.GetSelectedCursors();
        for (var i = 0; i < cursors.Count; i++)
        {
            var (row, column) = CursorItemsGrid.GetRowColumnByIndex(i);
            if (CursorItemsGrid.GetByRowColumn<CursorItemView>(row, column) is not CursorItemView cursorItemView)
                continue;
            cursorItemView.CCursor = cursors[i];
            Cursors.Add(cursorItemView);
        }
    }

    private async Task<Uri?> GetCursorInstallerFile()
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Pick Cursor File",
            AllowMultiple = false,
            FileTypeFilter = [CursorInstaller],
        });

        return files is null || files.Count == 0 ? null : files[0].Path;
    }

    public static FilePickerFileType CursorInstaller { get; } = new("Cursor Installer Files")
    {
        Patterns = ["*.inf"]
    };

    private async void InstallerImportButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var filePath = await GetCursorInstallerFile();
        if (filePath is null)
            return;

        OnImportInfFile?.Invoke(this, Uri.UnescapeDataString(filePath.AbsolutePath));

        var ccursors = CursorHelper.CursorsFromInstallerInf(Uri.UnescapeDataString(filePath.AbsolutePath));
        foreach (var ccursor in ccursors)
        {
            var cursorItemView = Cursors.FirstOrDefault(x => x.CCursor.Assignment.ID == ccursor.Assignment.ID);
            if (cursorItemView is not null)
            {
                //CursorHelper.ConvertCursorFile(cursorItemView.CCursor.CursorPath);
                cursorItemView.CCursor = ccursor;
            }
        }
    }
}
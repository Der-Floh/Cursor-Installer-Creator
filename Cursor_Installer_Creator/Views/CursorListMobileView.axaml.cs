using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;

using Cursor_Installer_Creator.ViewModels;

using UserControl = Avalonia.Controls.UserControl;

namespace Cursor_Installer_Creator.Views;

public sealed partial class CursorListMobileView : UserControl
{
    public CursorListMobileView()
    {
        InitializeComponent();
    }

    private void ImportInfPanel_DragEnter(object? sender, DragEventArgs e)
    {
        if (sender is Border border && IsValidCursorDrop(e))
            border.BorderThickness = new Thickness(4);
    }

    private void ImportInfPanel_DragLeave(object? sender, DragEventArgs e)
    {
        if (sender is Border border)
            border.BorderThickness = new Thickness(0);
    }

    private void ImportInfPanel_DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = IsValidCursorDrop(e)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private async void ImportInfPanel_Drop(object? sender, DragEventArgs e)
    {
        if (sender is Border border)
            border.BorderThickness = new Thickness(0);

        var file = GetSingleAcceptedFile(e);
        if (file is null)
            return;

        if (DataContext is not CursorListViewModel viewModel)
            return;

        await viewModel.RunGuardedAsync(async () => await viewModel.ImportInfFileAsync(file));
    }

    private static bool IsValidCursorDrop(DragEventArgs e)
        => OperatingSystem.IsBrowser()
            ? e.DataTransfer.Formats.Contains(DataFormat.File)
            : GetSingleAcceptedFile(e) is not null;

    private static IStorageFile? GetSingleAcceptedFile(DragEventArgs e)
    {
        if (!e.DataTransfer.Formats.Contains(DataFormat.File))
            return null;

        var files = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().ToArray();
        if (files is null || files.Length == 0)
            return null;

        var file = files[0];
        var isInfFile = Path.GetExtension(file.Name).Equals(".inf", StringComparison.OrdinalIgnoreCase);

        return isInfFile
            ? file
            : null;
    }
}

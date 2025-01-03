using Avalonia.Controls;
using Avalonia.Threading;
using Cursor_Installer_Creator.Utils;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cursor_Installer_Creator.Views;

public partial class UpdateNotifyView : UserControl
{
    public UpdateNotifyView()
    {
        InitializeComponent();
    }

    public async Task<bool> HasUpdateAsync() => await GitHubUpdater.HasUpdateAsync();

    public async Task ShowUpdateNotify()
    {
        await Dispatcher.UIThread.InvokeAsync(() => IsVisible = true);
        await Dispatcher.UIThread.InvokeAsync(() => UpdateBorderElem.Classes.Add("update"));
    }

    public async Task HideUpdateNotify()
    {
        await Dispatcher.UIThread.InvokeAsync(() => UpdateBorderElem.Classes.Remove("update"));
        await Task.Delay(1000);
        await Dispatcher.UIThread.InvokeAsync(() => IsVisible = false);
    }

    private void ViewOnGitHubButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = GitHubUpdater.LatestReleaseUrl,
            UseShellExecute = true
        });
        Task.Run(HideUpdateNotify);
    }

    private void DismissButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Task.Run(HideUpdateNotify);
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Cursor_Installer_Creator.Views;
using System.IO;

namespace Cursor_Installer_Creator;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += (_, _) => Directory.Delete(Program.TempPath, true);
            desktop.MainWindow = new MainWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
using Avalonia.Controls;

namespace Cursor_Installer_Creator.Views;

public sealed partial class MainWindow : Window
{
    public MainWindow(CursorInstallerMainContainer container)
    {
        InitializeComponent();
        Content = container;
    }
}

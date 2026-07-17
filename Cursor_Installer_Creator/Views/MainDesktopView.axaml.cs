using Avalonia.Controls;

using Cursor_Installer_Creator.ViewModels;

namespace Cursor_Installer_Creator.Views;

public sealed partial class CursorInstallerMainView : UserControl
{
    public CursorInstallerMainView(CursorInstallerMainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

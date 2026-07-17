using Avalonia.Controls;

using Cursor_Installer_Creator.ViewModels;

namespace Cursor_Installer_Creator.Views;

public sealed partial class CursorInstallerMainMobileView : UserControl
{
    public CursorInstallerMainMobileView(CursorInstallerMainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
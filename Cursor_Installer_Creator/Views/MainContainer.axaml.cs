using Avalonia;
using Avalonia.Controls;

using Cursor_Installer_Creator.Service.NotificationServ;
using Cursor_Installer_Creator.Service.TopLevelServ;

namespace Cursor_Installer_Creator.Views;

public sealed partial class CursorInstallerMainContainer : UserControl
{
    private readonly CursorInstallerMainView _normalView;
    private readonly CursorInstallerMainMobileView _mobileView;
    private readonly INotificationService _notificationService;
    private readonly ITopLevelProvider _topLevelProvider;

    public CursorInstallerMainContainer(CursorInstallerMainView normalView, CursorInstallerMainMobileView mobileView, INotificationService notificationService, ITopLevelProvider topLevelProvider)
    {
        InitializeComponent();

        _normalView = normalView;
        _mobileView = mobileView;
        _notificationService = notificationService;
        _topLevelProvider = topLevelProvider;

        _mobileView.IsVisible = false;
        _mobileView.IsEnabled = false;

        ResponsiveHost.Children.Insert(0, _normalView);
        ResponsiveHost.Children.Insert(1, _mobileView);
    }

    private void UserControl_AttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is not null)
            _topLevelProvider.SetTopLevel(topLevel);
        _notificationService.Setup(MainWindowNotificationManager);

        UpdateLayoutMode(Bounds.Size);
    }

    private void UserControl_SizeChanged(object? sender, SizeChangedEventArgs e)
        => UpdateLayoutMode(e.NewSize);

    private void UpdateLayoutMode(Size size)
    {
        if (size.Width <= 0 || size.Height <= 0)
            return;

        var useMobileView = size.Height > size.Width;

        _normalView.IsVisible = !useMobileView;
        _normalView.IsEnabled = !useMobileView;

        _mobileView.IsVisible = useMobileView;
        _mobileView.IsEnabled = useMobileView;
    }
}
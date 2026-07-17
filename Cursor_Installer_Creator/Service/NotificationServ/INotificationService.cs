using Avalonia.Controls.Notifications;

namespace Cursor_Installer_Creator.Service.NotificationServ;

public interface INotificationService
{
    void Setup(WindowNotificationManager notificationManager);
    void ShowInfo(string message);
    void ShowInfo(string title, string message);
    void ShowSuccess(string message);
    void ShowSuccess(string title, string message);
    void ShowWarning(string message);
    void ShowWarning(string title, string message);
    void ShowError(string message);
    void ShowError(string title, string message);
    void ShowNotification(string title, string message, NotificationType type);
    void ShowNotification(Notification notification);
}

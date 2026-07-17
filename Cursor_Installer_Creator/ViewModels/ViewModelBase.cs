using CommunityToolkit.Mvvm.ComponentModel;

using Cursor_Installer_Creator.Service.NotificationServ;

namespace Cursor_Installer_Creator.ViewModels;

public interface IViewModelBase { }

public abstract class ViewModelBase : ObservableObject, IViewModelBase
{
    private readonly INotificationService? _notificationService;

    protected ViewModelBase() { }

    protected ViewModelBase(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task RunGuardedAsync(Func<Task> action, bool browserFireAndForget = false)
    {
        var task = ExecuteGuardedAsync(action);

        // Non-Chromium browsers never complete file/folder picker Tasks on cancel.
        // Return a completed task so AsyncRelayCommand doesn't get stuck.
        return browserFireAndForget && OperatingSystem.IsBrowser()
            ? Task.CompletedTask
            : task;
    }

    private async Task ExecuteGuardedAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            _notificationService?.ShowError(ex.Message);
        }
    }
}

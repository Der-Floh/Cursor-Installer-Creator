using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Cursor_Installer_Creator.Repository.CursorAssignmentRepo;
using Cursor_Installer_Creator.Repository.CursorRepo;
using Cursor_Installer_Creator.Repository.InfFileRepo;
using Cursor_Installer_Creator.Service.CursorServ;
using Cursor_Installer_Creator.Service.GithubServ;
using Cursor_Installer_Creator.Service.InfServ;
using Cursor_Installer_Creator.Service.NotificationServ;
using Cursor_Installer_Creator.Service.StorageServ;
using Cursor_Installer_Creator.Service.TopLevelServ;
using Cursor_Installer_Creator.ViewModels;
using Cursor_Installer_Creator.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cursor_Installer_Creator;

public sealed partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        SetupGlobalExceptionHandlers();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => Services.GetRequiredService<CursorInstallerMainContainer>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = Services.GetRequiredService<CursorInstallerMainContainer>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void SetupGlobalExceptionHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                Services.GetService<INotificationService>()?.ShowError(ex.Message);
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Services.GetService<INotificationService>()?.ShowError(e.Exception.InnerException?.Message ?? e.Exception.Message);
            e.SetObserved();
        };
    }

    private static void ConfigureServices(IServiceCollection services)
    {
#if DEBUG
        var minLevel = LogLevel.Debug;
#else
        var minLevel = LogLevel.Warning;
#endif
        services.AddLogging(builder =>
        {
            // AddConsole() uses a background thread queue which throws PlatformNotSupportedException
            // in single-threaded WASM. Use a simple synchronous console logger for Browser instead.
            if (OperatingSystem.IsBrowser())
                builder.AddProvider(new SyncConsoleLoggerProvider());
            else
                builder.AddConsole();

            builder.SetMinimumLevel(minLevel);
        });

        // repositories
        services.AddSingleton<ICursorAssignmentRepository, CursorAssignmentRepository>();
        services.AddSingleton<ICursorRepository, CursorRepository>();
        services.AddSingleton<IInfFileRepository, InfFileRepository>();

        // services
        services.AddSingleton<ICursorService, CursorService>();
        services.AddSingleton<IInfService, InfService>();
        services.AddSingleton<IInfParserService, InfParserService>();
        services.AddSingleton<IGithubService, GithubService>();
        services.AddSingleton<ITopLevelProvider, TopLevelProvider>();
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<INotificationService, NotificationService>();

        // views
        services.AddTransient<MainWindow>();
        services.AddTransient<CursorInstallerMainContainer>();
        services.AddTransient<CursorInstallerMainView>();
        services.AddTransient<CursorInstallerMainMobileView>();
        services.AddTransient<CursorListView>();
        services.AddTransient<CursorListMobileView>();
        services.AddTransient<CursorItemView>();
        services.AddTransient<BitmapAnimation>();

        // view models
        services.AddSingleton<CursorInstallerMainViewModel>();
        services.AddTransient<CursorListViewModel>();
    }

    private sealed class SyncConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new SyncConsoleLogger(categoryName);
        public void Dispose() { }
    }

    private sealed class SyncConsoleLogger(string categoryName) : ILogger
    {
        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Console.WriteLine($"[{logLevel}] {categoryName}: {formatter(state, exception)}");
            if (exception is not null)
                Console.WriteLine(exception.ToString());
        }
    }
}
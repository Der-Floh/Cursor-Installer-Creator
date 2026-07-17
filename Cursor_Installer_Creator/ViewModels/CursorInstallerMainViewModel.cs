using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Cursor_Installer_Creator.Data;
using Cursor_Installer_Creator.Repository.InfFileRepo;
using Cursor_Installer_Creator.Service.GithubServ;
using Cursor_Installer_Creator.Service.InfServ;
using Cursor_Installer_Creator.Service.NotificationServ;
using Cursor_Installer_Creator.Service.StorageServ;

namespace Cursor_Installer_Creator.ViewModels;

public sealed partial class CursorInstallerMainViewModel : ViewModelBase
{
    public string DragDropLabel { get; } = "Supports Drag and Drop";
    public string CursorPreviewSizeLabel { get; } = "Cursor Preview Size";
    public string PackageNameLabel { get; } = "Cursor Package Name";
    public string PackageNameDefaultValue { get; } = Constants.DefaultInstallerSchemeName;
    public string PackageProviderLabel { get; } = "Cursor Package Provider";
    public string PackageProviderDefaultValue { get; } = Constants.DefaultInstallerProvider;
    public string InstallButtonLabel => InstallingCursor ? "Cancel" : "Install Cursor";
    public string PackageExportFolderLabel { get; } = "Target Folder";
    public string PackageExportZipLabel { get; } = "Compressed Archive (.zip)";
    public string PackageExportInstallerLabel { get; } = "Installer (.bat)";
    public string PackageCreateLabel { get; } = "Create Package";
    public string ViewOnGithubLabel { get; } = "View on GitHub";
    public string UpdateAvailableLabel { get; } = "An update is Available";

    [ObservableProperty]
    public partial bool InstallingCursor { get; set; }

    [ObservableProperty]
    public partial bool ExportingPackage { get; set; }

    [ObservableProperty]
    public partial ComboChoice<CursorPackageExportTypes>? CursorPackageType { get; set; }

    public ObservableCollection<ComboChoice<CursorPackageExportTypes>> CursorPackageTypes { get; } =
    [
        new() { Display = "Target Folder", Value = CursorPackageExportTypes.Folder },
        new() { Display = "Compressed Archive (.zip)", Value = CursorPackageExportTypes.Zip },
        new() { Display = "Installer (.bat)", Value = CursorPackageExportTypes.Installer },
    ];

    public CursorListViewModel CursorListViewModel { get; }

    private readonly IInfFileRepository _infFileRepository;
    private readonly IInfService _infService;
    private readonly IStorageService _storageService;
    private readonly INotificationService _notificationService;
    private readonly IGithubService _githubService;

    private CancellationTokenSource? _installCts;

    public CursorInstallerMainViewModel(IInfFileRepository infFileRepository, IInfService infService, IStorageService storageService, INotificationService notificationService, IGithubService githubService, CursorListViewModel cursorListViewModel) : base(notificationService)
    {
        _infFileRepository = infFileRepository;
        _infService = infService;
        _storageService = storageService;
        _notificationService = notificationService;
        _githubService = githubService;
        CursorListViewModel = cursorListViewModel;

        CursorPackageType = CursorPackageTypes[0];

        if (OperatingSystem.IsWindows())
            _ = RunGuardedAsync(CheckForUpdate);
    }

    partial void OnInstallingCursorChanged(bool value) => OnPropertyChanged(nameof(InstallButtonLabel));

    [RelayCommand(AllowConcurrentExecutions = true)]
    public Task InstallCursor() => RunGuardedAsync(async () =>
    {
        if (InstallingCursor)
        {
            _installCts?.Cancel();
            return;
        }

        if (!OperatingSystem.IsWindows())
            throw new InvalidOperationException("Only Windows supports directly installing cursors.");

        using var cts = new CancellationTokenSource();
        _installCts = cts;
        InstallingCursor = true;
        try
        {
            await _infService.InstallCursorPackage(CursorListViewModel.CurrentInfFile, cts.Token);
        }
        catch (OperationCanceledException) { /* swallow */ }
        finally
        {
            _installCts = null;
            InstallingCursor = false;
        }
    });

    [RelayCommand]
    public Task ExportPackage() => RunGuardedAsync(async () =>
    {
        ExportingPackage = true;
        try
        {
            if (CursorPackageType is null)
                throw new InvalidOperationException("Export Type must be selected.");

            if (string.IsNullOrWhiteSpace(CursorListViewModel.CurrentInfFile.SchemeName))
                throw new InvalidOperationException("Package Name must be provided.");

            if (string.IsNullOrWhiteSpace(CursorListViewModel.CurrentInfFile.Provider))
                throw new InvalidOperationException("Package Provider must be provided.");

            var folders = await _storageService.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Folder for Package Export",
                AllowMultiple = false,
            });

            if (folders is null || folders.Count == 0)
                return; // user cancelled

            var folder = folders[0];

            switch (CursorPackageType.Value)
            {
                case CursorPackageExportTypes.Folder:
                    await _infFileRepository.WriteInfPackageAsync(CursorListViewModel.CurrentInfFile, folder);
                    break;
                case CursorPackageExportTypes.Zip:
                    await _infFileRepository.WriteInfPackageAsync(CursorListViewModel.CurrentInfFile, folder, archive: true);
                    break;
                case CursorPackageExportTypes.Installer:
                    await _infFileRepository.WriteInfPackageInstallerAsync(CursorListViewModel.CurrentInfFile, folder);
                    break;
            }

            _notificationService.ShowSuccess("Package exported successfully.");
        }
        finally
        {
            ExportingPackage = false;
        }
    }, browserFireAndForget: true);

    [SupportedOSPlatform("windows")]
    public async Task CheckForUpdate()
    {
        await Task.Delay(1000);
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
        var hasUpdate = await _githubService.HasUpdateAsync(Constants.GithubRepoOwner, Constants.GithubRepoName, currentVersion);
        if (!hasUpdate)
            return;

        _notificationService.ShowNotification(new Notification
        {
            Title = "Update available",
            Message = "A new version of Cursor Installer Creator is available.",
            Type = NotificationType.Information,
            OnClick = () =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/Der-Floh/Cursor-Installer-Creator/releases/latest",
                    UseShellExecute = true
                });
            }
        });
    }
}

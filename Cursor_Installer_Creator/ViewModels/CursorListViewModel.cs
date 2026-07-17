using System.Collections.ObjectModel;
using System.Runtime.Versioning;

using Avalonia.Platform.Storage;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Cursor_Installer_Creator.Data;
using Cursor_Installer_Creator.Extensions;
using Cursor_Installer_Creator.Repository.CursorRepo;
using Cursor_Installer_Creator.Repository.InfFileRepo;
using Cursor_Installer_Creator.Service.CursorServ;
using Cursor_Installer_Creator.Service.InfServ;
using Cursor_Installer_Creator.Service.NotificationServ;
using Cursor_Installer_Creator.Service.StorageServ;

namespace Cursor_Installer_Creator.ViewModels;

public sealed partial class CursorListViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial InfFile CurrentInfFile { get; set; } = new InfFile();

    [ObservableProperty]
    public partial bool IsLoading { get; set; } = true;

    [ObservableProperty]
    public partial ComboChoice<int>? CursorPreviewSize { get; set; }

    public ObservableCollection<ComboChoice<int>> CursorPreviewSizes { get; } =
    [
        new() { Display = "Very Small (x32)", Value = 32 },
        new() { Display = "Small (x48)", Value = 48 },
        new() { Display = "Medium (x64)", Value = 64 },
        new() { Display = "Large (x96)", Value = 96 },
        new() { Display = "Very Large (x128)", Value = 128 }
    ];

    private readonly IStorageService _storageService;
    private readonly ICursorRepository _cursorRepository;
    private readonly IInfFileRepository _infFileRepository;
    private readonly IInfService _infService;
    private readonly ICursorService _cursorService;

    private readonly HashSet<InfCursorEntry> _resettingCursors = [];
    private readonly HashSet<InfCursorEntry> _importingCursors = [];

    private static readonly FilePickerFileType _infPickerType = new("INF Files")
    {
        Patterns = ["*.inf"],
        MimeTypes = ["application/inf", "text/plain"]
    };

    private static readonly FilePickerFileType _cursorPickerType = new("Cursor Files")
    {
        Patterns = ["*.cur", "*.ani"],
        MimeTypes = ["image/x-icon", "image/vnd.microsoft.icon", "application/x-navi-animation", "application/octet-stream"]
    };

    public CursorListViewModel(IStorageService storageService, INotificationService notificationService, ICursorRepository cursorRepository, IInfFileRepository infFileRepository, IInfService infService, ICursorService cursorService) : base(notificationService)
    {
        _storageService = storageService;
        _cursorRepository = cursorRepository;
        _infFileRepository = infFileRepository;
        _infService = infService;
        _cursorService = cursorService;

        CursorPreviewSize = CursorPreviewSizes[0];

        var items = Enumerable.Range(0, 17).Select(_ => new InfCursorEntry()).ToArray();
        CurrentInfFile.Cursors.AddRange(items);

        if (OperatingSystem.IsWindows())
            _ = RunGuardedAsync(LoadWindowsCursorsAsync);
        else
            _ = RunGuardedAsync(LoadCursorsAsync);
    }

    [RelayCommand]
    public Task ImportInfFile() => RunGuardedAsync(async () =>
    {
        if (!_storageService.CanOpenFiles)
            throw new InvalidOperationException("Storage provider does not support opening files.");

        var files = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Pick Cursor Installer (.inf) File",
            AllowMultiple = false,
            FileTypeFilter = [_infPickerType],
            SuggestedFileType = _infPickerType
        });

        if (files is null || files.Count == 0)
            return;

        var file = files[0];
        await ImportInfFileAsync(file);

    }, browserFireAndForget: true);

    [RelayCommand(AllowConcurrentExecutions = true, CanExecute = nameof(CanResetCursor))]
    public Task CursorItemResetCursor(InfCursorEntry entry) => RunGuardedAsync(async () =>
    {
        _resettingCursors.Add(entry);
        CursorItemResetCursorCommand.NotifyCanExecuteChanged();
        try
        {
            var assignment = entry.Assignment
                ?? throw new InvalidOperationException("Cursor does not have an assignment.");

            var defaultCursor = await _cursorRepository.GetDefaultCursorAsync(assignment);
            if (defaultCursor is not null)
                entry.Cursor = defaultCursor;
        }
        finally
        {
            _resettingCursors.Remove(entry);
            CursorItemResetCursorCommand.NotifyCanExecuteChanged();
        }
    });

    private bool CanResetCursor(InfCursorEntry entry) => !_resettingCursors.Contains(entry);

    [RelayCommand(AllowConcurrentExecutions = true, CanExecute = nameof(CanImportCursorFile))]
    public Task CursorItemImportCursorFile(InfCursorEntry entry) => RunGuardedAsync(async () =>
    {
        _importingCursors.Add(entry);
        CursorItemImportCursorFileCommand.NotifyCanExecuteChanged();
        try
        {
            if (!_storageService.CanOpenFiles)
                throw new InvalidOperationException("Storage provider does not support opening files.");

            var files = await _storageService.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Pick Cursor File (.cur, .ani)",
                AllowMultiple = false,
                FileTypeFilter = [_cursorPickerType],
                SuggestedFileType = _cursorPickerType
            });

            if (files is null || files.Count == 0)
                return;

            var file = files[0];
            if (!_cursorService.IsValidCursorFile(file.Name))
                throw new InvalidOperationException("Selected file is not a cursor file.");

            var newCursor = await _cursorRepository.GetCursorFromFileAsync(file, entry.Assignment);
            if (newCursor is not null)
                entry.Cursor = newCursor;
        }
        finally
        {
            _importingCursors.Remove(entry);
            CursorItemImportCursorFileCommand.NotifyCanExecuteChanged();
        }
    }, browserFireAndForget: true);

    private bool CanImportCursorFile(InfCursorEntry entry) => !_importingCursors.Contains(entry);

    [SupportedOSPlatform("windows")]
    private async Task LoadWindowsCursorsAsync()
    {
        IsLoading = true;
        var cursors = await _cursorRepository.GetCurrWindowsCursorsAsync();
        await LoadCursorsAsync(cursors);
    }

    private async Task LoadCursorsAsync()
    {
        IsLoading = true;
        var cursors = await _cursorRepository.GetDefaultCursorsAsync();
        await LoadCursorsAsync(cursors);
    }

    private async Task LoadCursorsAsync(IEnumerable<ICursor> cursors)
    {
        var filled = await _cursorRepository.AddMissingCursorsAsync(cursors);
        var entries = filled
            .Select(c => new InfCursorEntry { Cursor = c, Assignment = c?.Assignment })
            .OrderBy(x => x.Assignment?.Order);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            CurrentInfFile.Cursors.Clear();
            CurrentInfFile.Cursors.AddRange(entries);
            IsLoading = false;
        });
    }

    public async Task ImportInfFileAsync(IStorageFile file)
    {
        if (!Path.GetExtension(file.Name).Equals(".inf", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Selected file is not an INF file.");

        var infFile = await _infFileRepository.GetInfFileFromFileAsync(file);
        if (infFile is null)
            return;

        await _infService.LoadCursorsAsync(infFile, file);
        CurrentInfFile = infFile;
    }
}

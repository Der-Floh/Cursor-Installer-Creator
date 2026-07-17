using System.ComponentModel;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Platform.Storage;

using Cursor_Installer_Creator.Data;
using Cursor_Installer_Creator.Repository.CursorRepo;
using Cursor_Installer_Creator.Service.CursorServ;
using Cursor_Installer_Creator.Service.NotificationServ;

using Microsoft.Extensions.DependencyInjection;

namespace Cursor_Installer_Creator.Views;

public sealed partial class CursorItemView : UserControl
{
    public static readonly StyledProperty<int> PreviewSizeProperty =
        AvaloniaProperty.Register<CursorItemView, int>(nameof(PreviewSize), defaultValue: 32);

    public int PreviewSize
    {
        get => GetValue(PreviewSizeProperty);
        set => SetValue(PreviewSizeProperty, value);
    }

    private readonly BitmapAnimation _previewAnimation;
    private readonly TranslateTransform _previewTransform = new();
    private OverlayLayer? _overlayLayer;
    private InfCursorEntry? _currentEntry;
    private bool _previewVisible;

    public CursorItemView()
    {
        InitializeComponent();

        _previewAnimation = new BitmapAnimation
        {
            Width = PreviewSize,
            Height = PreviewSize,
            IsHitTestVisible = false,
            IsVisible = false,
            ClipToBounds = false,
            RenderTransform = _previewTransform,
            ApplyHotspotOffset = true,
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PreviewSizeProperty)
        {
            var size = change.GetNewValue<int>();
            _previewAnimation.Width = size;
            _previewAnimation.Height = size;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _overlayLayer = OverlayLayer.GetOverlayLayer(this);
        if (_overlayLayer is not null && !_overlayLayer.Children.Contains(_previewAnimation))
            _overlayLayer.Children.Add(_previewAnimation);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HidePreview();
        _overlayLayer?.Children.Remove(_previewAnimation);
        _overlayLayer = null;

        _currentEntry?.PropertyChanged -= OnEntryPropertyChanged;
        _currentEntry = null;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        _currentEntry?.PropertyChanged -= OnEntryPropertyChanged;
        _currentEntry = DataContext as InfCursorEntry;
        _currentEntry?.PropertyChanged += OnEntryPropertyChanged;
        _previewAnimation.ICursor = _currentEntry?.Cursor;
    }

    private void OnEntryPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(InfCursorEntry.Cursor))
            _previewAnimation.ICursor = _currentEntry?.Cursor;
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (_currentEntry?.Cursor is null)
            return;

        UpdateOverlayPosition(e);
        _previewVisible = true;
        CursorDisplay.Cursor = new Cursor(StandardCursorType.None);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_previewVisible)
            return;

        UpdateOverlayPosition(e);
        _previewAnimation.IsVisible = true;
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
        => HidePreview();

    private void UpdateOverlayPosition(PointerEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
            return;

        var pt = e.GetPosition(topLevel);
        _previewTransform.X = pt.X;
        _previewTransform.Y = pt.Y;
    }

    private void HidePreview()
    {
        _previewAnimation.IsVisible = false;
        _previewVisible = false;
        CursorDisplay.Cursor = Cursor.Default;
        // Keep ICursor intact to preserve the frame cache and hotspot for the next hover.
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (sender is Border border && IsValidCursorDrop(e))
            border[!Border.BorderBrushProperty] = new DynamicResourceExtension("AccentColor");
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        if (sender is Border border)
            border[!Border.BorderBrushProperty] = new DynamicResourceExtension("RegionColorDamp");
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = IsValidCursorDrop(e)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        if (sender is Border border)
            border[!Border.BorderBrushProperty] = new DynamicResourceExtension("RegionColorDamp");

        var file = GetSingleAcceptedFile(e);
        if (file is null)
            return;

        if (DataContext is not InfCursorEntry entry)
        {
            App.Services.GetRequiredService<INotificationService>().ShowError("Invalid data context. Expected an InfCursorEntry instance.");
            return;
        }

        try
        {
            var repository = App.Services.GetRequiredService<ICursorRepository>();
            entry.Cursor = await repository.GetCursorFromFileAsync(file, entry.Assignment);
        }
        catch (Exception ex)
        {
            App.Services.GetRequiredService<INotificationService>().ShowError(ex.Message);
        }
    }

    private static bool IsValidCursorDrop(DragEventArgs e)
        => OperatingSystem.IsBrowser()
            ? e.DataTransfer.Formats.Contains(DataFormat.File)
            : GetSingleAcceptedFile(e) is not null;

    private static IStorageFile? GetSingleAcceptedFile(DragEventArgs e)
    {
        if (!e.DataTransfer.Formats.Contains(DataFormat.File))
            return null;

        var files = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().ToArray();
        if (files is null || files.Length == 0)
            return null;

        var file = files[0];

        return App.Services.GetRequiredService<ICursorService>().IsValidCursorFile(file.Name)
            ? file
            : null;
    }
}

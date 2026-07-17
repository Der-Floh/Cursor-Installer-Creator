using System.Diagnostics;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;

using Cursor_Installer_Creator.Data;

namespace Cursor_Installer_Creator.Views;

public sealed partial class BitmapAnimation : UserControl
{
    public static readonly StyledProperty<ICursor?> ICursorProperty =
        AvaloniaProperty.Register<BitmapAnimation, ICursor?>(nameof(ICursor));

    public ICursor? ICursor
    {
        get => GetValue(ICursorProperty);
        set => SetValue(ICursorProperty, value);
    }

    private readonly DispatcherTimer _timer;
    private readonly Stopwatch _stopwatch = new();
    private TimeSpan _nextFrameAt;
    private CursorAnimationFrame[] _frames = [];
    private int _frameIndex;
    private CancellationTokenSource? _loadCts;
    private ICursor? _loadedForCursor;
    private int _loadedForSize;
    private readonly TranslateTransform _imageTransform = new();

    public BitmapAnimation()
    {
        InitializeComponent();

        RenderOptions.SetBitmapInterpolationMode(AnimationImage, BitmapInterpolationMode.LowQuality);
        AnimationImage.RenderTransform = _imageTransform;

        _timer = new DispatcherTimer();
        _timer.Tick += Timer_Tick;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ICursorProperty && this.IsAttachedToVisualTree())
            _ = ReloadAsync();

        if (change.Property == WidthProperty && this.IsAttachedToVisualTree() && ICursor is not null)
            _ = ReloadAsync();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _ = ReloadAsync();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopAnimation();
        CancelLoad();
        _loadedForSize = 0;
        // Keep _frames alive — if the same ICursor is reattached we can skip re-decoding.
    }

    public async Task ReloadAsync()
    {
        StopAnimation();
        CancelLoad();

        var cursor = ICursor;

        if (cursor is null)
        {
            DisposeFrames();
            ShowFrameDirect(null);
            return;
        }

        // If the same cursor is already decoded at the same size, just restart the animation.
        var targetSize = (int)Math.Max(0, double.IsNaN(Width) ? 0 : Width);
        if (cursor == _loadedForCursor && targetSize == _loadedForSize && _frames.Length > 0)
        {
            _frameIndex = 0;
            RestartAnimation();
            return;
        }

        DisposeFrames();

        var cts = new CancellationTokenSource();
        _loadCts = cts;

        CursorAnimationFrame[] frames;
        try
        {
            frames = await cursor.GetCursorFramesAsync(targetSize);
            if (cts.IsCancellationRequested)
                return;
        }
        catch (OperationCanceledException)
        {
            return;
        }
        finally
        {
            if (cts.IsCancellationRequested)
                CancelLoad();
        }

        _frames = [.. frames.Where(x => x is not null && x.Frame is not null)];
        _loadedForCursor = cursor;
        _loadedForSize = targetSize;
        _frameIndex = 0;

        if (_frames.Length == 0)
        {
            ShowFrameDirect(null);
            return;
        }

        // Non-ANI cursors are treated as static images.
        if (cursor.Type != CursorType.ani)
        {
            ShowFrameDirect(_frames[0].Frame);
            return;
        }

        RestartAnimation();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_frames.Length == 0)
            return;

        var elapsed = _stopwatch.Elapsed;

        // Advance at least one frame; skip additional frames if the UI thread was busy
        do
        {
            _frameIndex = (_frameIndex + 1) % _frames.Length;
            _nextFrameAt += GetFrameDuration(_frameIndex);
        }
        while (_nextFrameAt <= elapsed);

        ShowFrameDirect(_frames[_frameIndex].Frame);
        ScheduleNextFrame();
    }

    public bool ApplyHotspotOffset { get; set; }

    private void ShowFrameDirect(Bitmap? bitmap)
    {
        if (ApplyHotspotOffset && _frameIndex >= 0 && _frameIndex < _frames.Length && bitmap is not null)
        {
            var scale = CalculateUniformScale(bitmap);
            _imageTransform.X = -_frames[_frameIndex].HotspotX * scale;
            _imageTransform.Y = -_frames[_frameIndex].HotspotY * scale;
        }
        else
        {
            _imageTransform.X = 0;
            _imageTransform.Y = 0;
        }

        AnimationImage.Source = bitmap;
    }

    private double CalculateUniformScale(Bitmap bitmap)
    {
        var scaleX = bitmap.PixelSize.Width > 0 ? Width / bitmap.PixelSize.Width : 1;
        var scaleY = bitmap.PixelSize.Height > 0 ? Height / bitmap.PixelSize.Height : 1;
        return Math.Min(scaleX, scaleY);
    }

    private void RestartAnimation()
    {
        ShowFrameDirect(_frames.Length > 0 ? _frames[0].Frame : null);

        if (_frames.Length > 1)
        {
            _stopwatch.Restart();
            _nextFrameAt = GetFrameDuration(0);
            ScheduleNextFrame();
        }
    }

    private void ScheduleNextFrame()
    {
        if (_frames.Length == 0)
            return;

        var remaining = _nextFrameAt - _stopwatch.Elapsed;
        if (remaining < TimeSpan.FromMilliseconds(1))
            remaining = TimeSpan.FromMilliseconds(1);

        _timer.Stop();
        _timer.Interval = remaining;
        _timer.Start();
    }

    private TimeSpan GetFrameDuration(int index)
    {
        var d = _frames[index].Duration;
        return d > TimeSpan.Zero ? d : TimeSpan.FromMilliseconds(100);
    }

    private void StopAnimation()
    {
        _timer.Stop();
        _stopwatch.Stop();
    }

    private void CancelLoad()
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = null;
    }

    private void DisposeFrames()
    {
        foreach (var frame in _frames)
            frame.Dispose();

        _frames = [];
        _loadedForCursor = null;
        _frameIndex = 0;
    }
}

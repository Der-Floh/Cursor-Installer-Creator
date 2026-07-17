using Avalonia.Media.Imaging;

namespace Cursor_Installer_Creator.Data;

public sealed class CursorAnimationFrame : IDisposable
{
    public TimeSpan Duration { get; }
    public Bitmap Frame { get; }
    public ushort HotspotX { get; }
    public ushort HotspotY { get; }

    public CursorAnimationFrame(TimeSpan duration, Bitmap frame, ushort hotspotX, ushort hotspotY)
    {
        Duration = duration;
        Frame = frame;
        HotspotX = hotspotX;
        HotspotY = hotspotY;
    }

    public void Dispose() => Frame.Dispose();
}

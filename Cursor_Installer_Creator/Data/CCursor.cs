using Avalonia.Media.Imaging;

using Ico.Reader;
using Ico.Reader.Data;

namespace Cursor_Installer_Creator.Data;

public sealed class CCursor : CursorBase
{
    public CCursor(byte[] cursorBytes, CursorType type, CursorAssignment? assignment = null) : base(cursorBytes, type, assignment) { }

    public override async Task<CursorAnimationFrame[]> GetCursorFramesAsync(int targetSize = 0)
    {
        var icoReader = new IcoReader();
        var icoData = await Task.Run(() => icoReader.Read(CursorBytes));
        if (icoData is null)
            return [];

        var index = SelectBestImageIndex(icoData, targetSize);
        var bytes = await icoData.GetImageAsync(index);
        if (bytes is null)
            return [];
        var stream = new MemoryStream(bytes);
        var bitmap = new Bitmap(stream);

        var imageRef = icoData.ImageReferences[index];
        var hotspotX = imageRef.HotspotX;
        var hotspotY = imageRef.HotspotY;

        return [new CursorAnimationFrame(TimeSpan.Zero, bitmap, hotspotX, hotspotY)];
    }

    private static int SelectBestImageIndex(IcoData icoData, int targetSize)
    {
        if (targetSize <= 0)
            return icoData.PreferredImageIndex();

        var refs = icoData.ImageReferences;
        var candidates = refs
            .Select((r, i) => (r, i))
            .Where(x => x.r.Width >= targetSize && x.r.Height >= targetSize)
            .ToArray();

        if (candidates.Length == 0)
            return icoData.PreferredImageIndex();

        return candidates
            .OrderBy(x => x.r.Width * x.r.Height)
            .ThenByDescending(x => x.r.BitCount)
            .First().i;
    }
}

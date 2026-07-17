using Ani.Reader;
using Ani.Reader.Models;

using Avalonia.Media.Imaging;

namespace Cursor_Installer_Creator.Data;

public sealed class ACursor : CursorBase
{
    public ACursor(byte[] cursorBytes, CursorType type, CursorAssignment? assignment = null) : base(cursorBytes, type, assignment) { }

    public override async Task<CursorAnimationFrame[]> GetCursorFramesAsync(int targetSize = 0)
    {
        var frames = new List<CursorAnimationFrame>();
        var aniReader = new AniReader();
        var aniDatas = await Task.Run(() => aniReader.Read(CursorBytes));
        if (aniDatas is null)
            return [];

        var aniData = aniDatas.FirstOrDefault();
        if (aniData is null)
            return [];

        var index = SelectBestAnimationIndex(aniData, targetSize);

        var hotspots = aniData.Animations[index].FrameHotspots.ToArray();
        var allHotspotXSame = hotspots.All(item => item.HotspotX == hotspots[0].HotspotX);
        var allHotspotYSame = hotspots.All(item => item.HotspotY == hotspots[0].HotspotY);
        var hasGlobalHotspot = allHotspotXSame && allHotspotYSame;
        ushort? globalHotspotX = hasGlobalHotspot ? hotspots[0].HotspotX : null;
        ushort? globalHotspotY = hasGlobalHotspot ? hotspots[0].HotspotY : null;

        foreach (var frame in aniData.Frames)
        {
            var bytes = await aniData.GetFrameBytes(aniData.Animations[index], frame);
            if (bytes is null)
                continue;

            var hotspotX = globalHotspotX ?? 0;
            var hotspotY = globalHotspotY ?? 0;
            if (!hasGlobalHotspot)
            {
                var hotspot = hotspots.FirstOrDefault(x => x.FramePosition == frame.Position);
                hotspotX = hotspot?.HotspotX ?? 0;
                hotspotY = hotspot?.HotspotY ?? 0;
            }

            var stream = new MemoryStream(bytes);
            var bitmap = new Bitmap(stream);
            frames.Add(new CursorAnimationFrame(frame.Duration, bitmap, hotspotX, hotspotY));
        }
        return [.. frames];
    }

    private static int SelectBestAnimationIndex(AniData aniData, int targetSize)
    {
        if (targetSize <= 0)
            return aniData.PreferredAnimationIndex();

        var candidates = aniData.Animations
            .Select((a, i) => (a, i))
            .Where(x => x.a.Width >= targetSize && x.a.Height >= targetSize)
            .ToArray();

        if (candidates.Length == 0)
            return aniData.PreferredAnimationIndex();

        return candidates
            .OrderBy(x => x.a.Width * x.a.Height)
            .ThenByDescending(x => x.a.BitCount)
            .First().i;
    }
}

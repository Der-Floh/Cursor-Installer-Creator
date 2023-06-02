namespace Cursor_Installer_Creator;
public static class PanelExtensions
{
    public static readonly string BorderColorTag = "BorderColor";
    public static Color BorderColor;

    public static void SetBorderColor(this Panel panel, Color color)
    {
        if (HasBorderColor(panel))
            RemoveBorderColor(panel);

        BorderColor = color;
        panel.Paint += Panel_Paint;
        panel.Tag = BorderColorTag;
        panel.Invalidate();
    }

    public static void RemoveBorderColor(this Panel panel)
    {
        panel.Paint -= Panel_Paint;
        panel.Tag = null;
        panel.Invalidate();
    }

    public static bool HasBorderColor(this Panel panel)
    {
        return panel.Tag != null && panel.Tag.ToString() == BorderColorTag;
    }

    private static void Panel_Paint(object sender, PaintEventArgs e)
    {
        Panel panel = (Panel)sender;
        using Pen pen = new Pen(BorderColor);
        e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, ((Panel)sender).Width - 1, ((Panel)sender).Height - 1));
    }
}

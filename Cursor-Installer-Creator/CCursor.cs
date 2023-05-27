using System.Text.Json.Serialization;

namespace Cursor_Installer_Creator;

public enum CCursorType
{
    cur,
    ani,
    unknown,
}

public sealed class CCursor
{
    public string? Name { get; set; }
    public string? CursorName { get; set; }
    public string? CursorPath { get; set; }
    public string? ImagePath { get; set; }
    [JsonIgnore]
    public CCursorType Type
    {
        get
        {
            if (string.IsNullOrEmpty(CursorPath))
                return CCursorType.unknown;

            if (CursorPath.EndsWith(".ani"))
                return CCursorType.ani;
            else if (CursorPath.EndsWith(".cur"))
                return CCursorType.cur;

            return CCursorType.unknown;
        }

    }
}

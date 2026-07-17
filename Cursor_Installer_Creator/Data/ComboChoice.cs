namespace Cursor_Installer_Creator.Data;

public sealed class ComboChoice<T>
{
    public required string Display { get; init; }
    public required T Value { get; init; }
}

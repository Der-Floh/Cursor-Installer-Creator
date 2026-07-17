namespace Cursor_Installer_Creator.Data;

public sealed class CursorAssignment : IComparable<CursorAssignment>
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? WindowsDefault { get; set; }
    public string? WindowsReg { get; set; }
    public string? WindowsInstall { get; set; }
    public string? Avalonia { get; set; }

    public bool Equals(CursorAssignment? other)
        => other is not null && Id == other.Id;

    public override bool Equals(object? obj)
        => obj is CursorAssignment other && Equals(other);

    public override int GetHashCode()
        => Id.GetHashCode();

    public int CompareTo(CursorAssignment? other)
        => other is null ? 1 : Id.CompareTo(other.Id);

    public static bool operator ==(CursorAssignment? left, CursorAssignment? right)
        => Equals(left, right);

    public static bool operator !=(CursorAssignment? left, CursorAssignment? right)
        => !Equals(left, right);

    public static bool operator <(CursorAssignment left, CursorAssignment right)
        => left.Id < right.Id;

    public static bool operator <=(CursorAssignment left, CursorAssignment right)
        => left.Id <= right.Id;

    public static bool operator >(CursorAssignment left, CursorAssignment right)
        => left.Id > right.Id;

    public static bool operator >=(CursorAssignment left, CursorAssignment right)
        => left.Id >= right.Id;

    public static implicit operator int(CursorAssignment value)
        => value.Id;

    public override string ToString()
        => $"{Name ?? "Unknown"} | {WindowsInstall}";
}

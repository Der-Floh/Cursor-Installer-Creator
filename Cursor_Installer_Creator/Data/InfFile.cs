using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Cursor_Installer_Creator.Data;

public sealed partial class InfFile : ObservableObject
{
    [ObservableProperty]
    public partial string? SchemeName { get; set; }

    [ObservableProperty]
    public partial string? Provider { get; set; }

    [ObservableProperty]
    public partial string? Signature { get; set; }

    [ObservableProperty]
    public partial Version? DriverVer { get; set; }

    [ObservableProperty]
    public partial DateOnly? DriverVerDate { get; set; }

    [ObservableProperty]
    public partial string? CursorDirectory { get; set; }

    public ObservableCollection<InfCursorEntry> Cursors { get; } = [];

    public override string ToString()
        => $"{SchemeName ?? "Unknown Scheme"} by {Provider ?? "Unknown Provider"}";
}

public sealed partial class InfCursorEntry : ObservableObject
{
    [ObservableProperty]
    public partial ICursor? Cursor { get; set; }

    [ObservableProperty]
    public partial CursorAssignment? Assignment { get; set; }

    [ObservableProperty]
    public partial string? SourcePath { get; set; }

    public override string ToString()
        => $"{Assignment?.ToString() ?? "Unknown"} ({Cursor?.Type})";
}

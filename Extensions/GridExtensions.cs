using Avalonia.Controls;
using System.Linq;

namespace Cursor_Installer_Creator.Extensions;

public static class GridExtensions
{
    public static T? GetByRowColumn<T>(this Grid grid, int row, int column) => grid.Children.OfType<T>().FirstOrDefault(child => Grid.GetRow(child as Control) == row && Grid.GetColumn(child as Control) == column);
    public static (int, int) GetRowColumnByIndex(this Grid grid, int index)
    {
        var row = index / grid.ColumnDefinitions.Count;
        var column = index % grid.ColumnDefinitions.Count;

        return (row, column);
    }
}

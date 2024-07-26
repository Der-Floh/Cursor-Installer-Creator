using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cursor_Installer_Creator.Views;

public partial class CursorItemView : UserControl
{
    public CCursor CCursor
    {
        get => _cCursor;
        set
        {
            _cCursor = value;
            UpdateCCursorDisplay();
        }
    }
    private CCursor _cCursor;

    private Bitmap? _cursorImage;
    private readonly CCursor _defaultCursor = new CCursor
    {
        Assignment = CursorAssignment.FromName("IDC_ARROW", CursorAssignmentType.Name)!,
        CursorPath = @"C:\Windows\Cursors\aero_arrow.cur"
    };

    public CursorItemView()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DropEvent, Drop);
        DragDrop.SetAllowDrop(this, true);

        Avalonia.Media.RenderOptions.SetBitmapInterpolationMode(CursorImage, BitmapInterpolationMode.HighQuality);
        Avalonia.Media.RenderOptions.SetBitmapInterpolationMode(ResetImage, BitmapInterpolationMode.HighQuality);
        Avalonia.Media.RenderOptions.SetBitmapInterpolationMode(FileOpenImage, BitmapInterpolationMode.HighQuality);

        _cCursor = _defaultCursor;
        UpdateCursorFromFile(CCursor.CursorPath);

        UpdateCCursorDisplay();
    }

    private void UpdateCCursorDisplay()
    {
        CursorNameTextBlock.Text = CCursor.Assignment.WindowsReg;

        var avaloniaCursorName = CCursor.Assignment.Avalonia;
        if (string.IsNullOrEmpty(avaloniaCursorName))
        {
            avaloniaCursorName = "Arrow";
        }
        CursorItemGrid.Cursor = Cursor.Parse(avaloniaCursorName);

        var imagePath = CCursor.GetImagePath();
        if (!string.IsNullOrEmpty(imagePath))
        {
            _cursorImage?.Dispose();
            _cursorImage = new Bitmap(imagePath);
            CursorImage.Source = _cursorImage;
        }
    }

    private void UpdateCursorFromFile(string filePath)
    {
        if (CursorHelper.ConvertCursorFile(filePath, CCursor.Assignment.ID) is CCursor cursor)
        {
            CCursor = cursor;
        }
    }

    private async Task<Uri?> GetCursorFile()
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Pick Cursor File",
            AllowMultiple = false,
            FileTypeFilter = [CursorAll],
        });

        return files is null || files.Count == 0 ? null : files[0].Path;
    }

    public static FilePickerFileType CursorAll { get; } = new("Cursor Files")
    {
        Patterns = ["*.cur", "*.ani"]
    };

    public static bool IsFileMatchingPattern(string filePath)
    {
        return CursorAll.Patterns?.Any(pattern =>
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
            return Regex.IsMatch(Path.GetFileName(filePath), regexPattern, RegexOptions.IgnoreCase);
        }) ?? false;
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        e.Handled = true;

        var files = e.Data.GetFiles()?.ToArray();
        e.DragEffects = files is null || files.Length != 1
            ? DragDropEffects.None
            : IsFileMatchingPattern(files[0].Name) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        e.Handled = true;

        var files = e.Data.GetFiles()?.ToArray();
        if (files is not null && files.Length == 1)
        {
            UpdateCursorFromFile(files[0].Path.AbsolutePath);
        }
    }

    private void CursorResetButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var cursor = CursorHelper.GetSelectedCursor(CCursor.Assignment);
        cursor ??= _defaultCursor;
        CCursor = cursor;
    }

    private async void CursorPickButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var filePath = await GetCursorFile();
        if (filePath is null)
            return;
        UpdateCursorFromFile(filePath.AbsolutePath);
    }
}
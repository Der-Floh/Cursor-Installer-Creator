using Avalonia.Platform.Storage;

namespace Cursor_Installer_Creator.Extensions;

public static class StorageFolderExtensions
{
    public static async Task<IStorageFolder?> GetFolderAsync(this IStorageFolder folder, string name, StringComparison comparison)
    {
        await foreach (var item in folder.GetItemsAsync())
        {
            if (item is IStorageFolder sub && sub.Name.Equals(name, comparison))
                return sub;
        }
        return null;
    }

    public static async Task<IStorageFile?> GetFileAsync(this IStorageFolder folder, string name, StringComparison comparison)
    {
        await foreach (var item in folder.GetItemsAsync())
        {
            if (item is IStorageFile file && file.Name.Equals(name, comparison))
                return file;
        }
        return null;
    }
}

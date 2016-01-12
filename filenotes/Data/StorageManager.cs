using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Sbs20.Filenotes.Data
{
    public class StorageManager
    {
        public static async Task<IReadOnlyList<StorageFile>> LoadFilesAsync()
        {
            var folder = await Settings.GetStorageFolderAsync();
            var files = await folder.GetFilesAsync();
            return files;
        }

        public static async Task SaveFileAsync(string name, string data)
        {
            var folder = await Settings.GetStorageFolderAsync();
            StorageFile file = await folder.GetFileAsync(name);
            await FileIO.WriteTextAsync(file, data);
        }

        public static async Task<string> CreateFileAsync(string name, string data)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteTextAsync(file, data);
            return file.Name;
        }

        public static async Task DeleteFileAsync(string name)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(name);
            await file.DeleteAsync(StorageDeleteOption.Default);
        }

        public static async Task<string> RenameFileAsync(string name, string desiredName)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(name);
            await file.RenameAsync(desiredName, NameCollisionOption.GenerateUniqueName);
            return file.Name;
        }
    }
}

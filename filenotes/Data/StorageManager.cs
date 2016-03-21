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
            if (folder != null)
            {
                var files = await folder.GetFilesAsync();
                return files;
            }

            throw new InvalidOperationException("Storage folder not set");
        }

        public static async Task SaveFileAsync(string name, string data)
        {
            var folder = await Settings.GetStorageFolderAsync();

            if (folder != null)
            {
                StorageFile file = null;
                try
                {
                    file = await folder.GetFileAsync(name);
                    await FileIO.WriteTextAsync(file, data);
                }
                catch (System.IO.FileNotFoundException)
                {
                    // In order to get here, something has to have deleted the file in the background
                    // WHILE someone was editing. This is all a bit weird but it's better to save 
                    // someone's data and let them delete it again properly than to lose something or
                    // just crash
                    await CreateFileAsync(name, data);
                }
            }
        }

        public static async Task<string> CreateFileAsync(string name, string data)
        {
            var folder = await Settings.GetStorageFolderAsync();
            if (folder != null)
            {
                var file = await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
                await FileIO.WriteTextAsync(file, data);
                return file.Name;
            }

            throw new InvalidOperationException("Storage folder not set");
        }

        public static async Task DeleteFileAsync(string name)
        {
            var folder = await Settings.GetStorageFolderAsync();
            if (folder != null)
            {
                var file = await folder.GetFileAsync(name);
                await file.DeleteAsync(StorageDeleteOption.Default);
            }
        }

        public static async Task<string> RenameFileAsync(string name, string desiredName)
        {
            var folder = await Settings.GetStorageFolderAsync();
            if (folder != null)
            {
                var file = await folder.GetFileAsync(name);
                await file.RenameAsync(desiredName, NameCollisionOption.GenerateUniqueName);
                return file.Name;
            }

            throw new InvalidOperationException("Storage folder not set");
        }
    }
}

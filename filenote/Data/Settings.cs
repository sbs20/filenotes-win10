using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace Sbs20.Filenote.Data
{
    public class Settings
    {
        private const string LocalStorageDirectory = "LocalStorageDirectory";

        public static async Task SelectLocalDirectoryAsync()
        {
            var fp = new FolderPicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            fp.FileTypeFilter.Add("*");

            var folder = await fp.PickSingleFolderAsync();

            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(LocalStorageDirectory, folder);
            }
        }

        public static async Task<StorageFolder> GetStorageFolderAsync()
        {
            if (!StorageApplicationPermissions.FutureAccessList.ContainsItem(LocalStorageDirectory))
            {
                await SelectLocalDirectoryAsync();
            }

            return await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(LocalStorageDirectory);
        }

        public static INote CreateNote()
        {
            return new ViewModels.NoteViewModel();
        }
    }
}

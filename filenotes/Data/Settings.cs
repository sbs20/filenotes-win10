using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace Sbs20.Filenotes.Data
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

            while (true)
            {
                var folder = await fp.PickSingleFolderAsync();

                if (folder != null)
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(LocalStorageDirectory, folder);
                }

                if (StorageApplicationPermissions.FutureAccessList.ContainsItem(LocalStorageDirectory))
                {
                    break;
                }
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
            return new ViewModels.Note();
        }

        public static ApplicationTheme ApplicationTheme
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("IsLightTheme"))
                {
                    return (bool)ApplicationData.Current.LocalSettings.Values["IsLightTheme"] ? ApplicationTheme.Light : ApplicationTheme.Dark;
                }

                return ApplicationTheme.Light;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["IsLightTheme"] = value == ApplicationTheme.Light;
            }
        }
    }
}

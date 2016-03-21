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
        private const string SettingLocalStorageDirectory = "LocalStorageDirectory";
        private const string SettingIsSaveButtonVisible = "IsSaveButtonVisible";

        public static void ClearLocalFileReference()
        {
            if (StorageApplicationPermissions.FutureAccessList.ContainsItem(SettingLocalStorageDirectory))
            {
                StorageApplicationPermissions.FutureAccessList.Remove(SettingLocalStorageDirectory);
            }
        }

        public static async Task SelectLocalDirectoryAsync()
        {
            var folderPicker = new FolderPicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            folderPicker.FileTypeFilter.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(SettingLocalStorageDirectory, folder);
            }
        }

        public static async Task<StorageFolder> GetStorageFolderAsync()
        {
            if (StorageApplicationPermissions.FutureAccessList.ContainsItem(SettingLocalStorageDirectory))
            {
                try
                {
                    return await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(SettingLocalStorageDirectory);
                }
                catch { }
            }

            return null;
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

        private static object get(string key, object def)
        {
            return ApplicationData.Current.LocalSettings.Values[key] ?? def;
        }

        public static bool IsSaveButtonVisible
        {
            get { return Convert.ToBoolean(get(SettingIsSaveButtonVisible, false)); }
            set { ApplicationData.Current.LocalSettings.Values[SettingIsSaveButtonVisible] = value; }
        }
    }
}

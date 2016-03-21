using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Sbs20.Filenotes.Data;
using Sbs20.Filenotes.ViewModels;

namespace Sbs20.Filenotes.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            this.DarkThemeToggle.IsOn = Settings.ApplicationTheme == ApplicationTheme.Dark;
            this.SaveButtonVisibleToggle.IsOn = Settings.IsSaveButtonVisible;
        }

        private async void ChangeStorageLocation_Click(object sender, RoutedEventArgs e)
        {
            await Settings.SelectLocalDirectoryAsync();
        }

        private void ClearStorageLocation_Click(object sender, RoutedEventArgs e)
        {
            Settings.ClearLocalFileReference();
            NoteManager.Notes.Clear();
        }

        private void ApplyTheme_Click(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Dark;
        }

        private void DarkThemeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationTheme selectedTheme = this.DarkThemeToggle.IsOn ? ApplicationTheme.Dark : ApplicationTheme.Light;
            Settings.ApplicationTheme = selectedTheme;
            var appShell = Window.Current.Content as AppShell;
            appShell.RequestedTheme = selectedTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
        }

        private void SaveButtonVisibleToggle_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.IsSaveButtonVisible = this.SaveButtonVisibleToggle.IsOn;
        }
    }
}

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Sbs20.Filenote.Data;
using Sbs20.Filenote.ViewModels;
using Windows.System;
using Windows.UI.Xaml.Input;
using Sbs20.Filenote.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Sbs20.Filenote.Views
{
    public sealed partial class DetailPage : Page
    {
        private static DependencyProperty s_noteProperty
            = DependencyProperty.Register("Note", typeof(NoteViewModel), typeof(DetailPage), new PropertyMetadata(null));

        public static DependencyProperty NoteProperty
        {
            get { return s_noteProperty; }
        }

        public NoteViewModel Note
        {
            get { return (NoteViewModel)GetValue(s_noteProperty); }
            set { SetValue(s_noteProperty, value); }
        }

        public DetailPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Parameter is item ID
            this.Note = await StorageManager.GetNoteByNameAsync((string)e.Parameter) as NoteViewModel;
            
            var backStack = Frame.BackStack;
            var backStackCount = backStack.Count;

            if (backStackCount > 0)
            {
                var masterPageEntry = backStack[backStackCount - 1];
                backStack.RemoveAt(backStackCount - 1);

                // Doctor the navigation parameter for the master page so it
                // will show the correct item in the side-by-side view.
                var modifiedEntry = new PageStackEntry(
                    masterPageEntry.SourcePageType,
                    this.Note.Name,
                    masterPageEntry.NavigationTransitionInfo);

                backStack.Add(modifiedEntry);
            }

            // Register for hardware and software back request from the system
            SystemNavigationManager.GetForCurrentView().BackRequested += DetailPage_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            SystemNavigationManager.GetForCurrentView().BackRequested -= DetailPage_BackRequested;
        }

        private void OnBackRequested()
        {
            // Page above us will be our master view.
            // Make sure we are using the "drill out" animation in this transition.
            
            Frame.GoBack(new DrillInNavigationTransitionInfo());
        }

        void NavigateBackForWideState(bool useTransition)
        {
            // Evict this page from the cache as we may not need it again.
            NavigationCacheMode = NavigationCacheMode.Disabled;

            if (useTransition)
            {
                Frame.GoBack(new EntranceNavigationTransitionInfo());
            }
            else
            {
                Frame.GoBack(new SuppressNavigationTransitionInfo());
            }
        }

        private bool ShouldGoToWideState()
        {
            return Window.Current.Bounds.Width >= 720;
        }

        private void PageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (ShouldGoToWideState())
            {
                // We shouldn't see this page since we are in "wide master-detail" mode.
                // Play a transition as we are navigating from a separate page.
                NavigateBackForWideState(useTransition:true);
            }
            else
            {
                // Realize the main page content.
                FindName("RootPanel");
            }

            Window.Current.SizeChanged += Window_SizeChanged;
        }

        private void PageRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (ShouldGoToWideState())
            {
                // Make sure we are no longer listening to window change events.
                Window.Current.SizeChanged -= Window_SizeChanged;

                // We shouldn't see this page since we are in "wide master-detail" mode.
                NavigateBackForWideState(useTransition:false);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackRequested();
        }

        private void DetailPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            // Mark event as handled so we don't get bounced out of the app.
            e.Handled = true;

            OnBackRequested();
        }

        private void RenameStart()
        {
            var headerEdit = this.AllChildren().OfType<FrameworkElement>().Where(el => el.Name == "PageHeaderEdit").First();
            headerEdit.Visibility = Visibility.Visible;

            var header = this.AllChildren().OfType<FrameworkElement>().Where(el => el.Name == "PageHeader").First();
            header.Visibility = Visibility.Collapsed;
        }

        private async Task RenameFinish()
        {
            // Make the textbox invisible
            var headerEdit = this.AllChildren().OfType<FrameworkElement>().Where(el => el.Name == "PageHeaderEdit").First();
            headerEdit.Visibility = Visibility.Collapsed;

            // Get the textbox itself
            var box = (TextBox)headerEdit.AllChildren().OfType<TextBox>().First();
            var desiredName = box.Text;
            var note = this.Note;

            // Rename
            await StorageManager.RenameNoteAsync(note, desiredName);
        }

        private void PageHeader_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.RenameStart();
        }

        private async void DetailTitleBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                // Stop this being handled again
                e.Handled = true;

                // Now handle the actual change
                await this.RenameFinish();
            }
        }

        private async void DetailTitleBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;

            // The text box loses focus when a user hits enter... we don't want to run this again
            // So we check to see if it's still visible
            if (box.IsVisible())
            {
                await this.RenameFinish();
            }
        }
    }
}

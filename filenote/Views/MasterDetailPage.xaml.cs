using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Sbs20.Filenote.ViewModels;

namespace Sbs20.Filenote.Views
{
    public sealed partial class MasterDetailPage : Page
    {
        private NoteViewModel lastSelectedNote;

        public MasterDetailPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (this.MasterListView.ItemsSource == null)
            {
                this.MasterListView.ItemsSource = await NoteCollectionViewModel.LoadAsync();
            }

            if (e.Parameter != null)
            {
                // Parameter is item ID
                var id = (string)e.Parameter;
                this.lastSelectedNote = ((NoteCollectionViewModel)this.MasterListView.ItemsSource)
                    .Where((item) => item.FullName == id)
                    .FirstOrDefault();
            }

            this.UpdateForVisualState(AdaptiveStates.CurrentState);

            // Don't play a content transition for first item load.
            // Sometimes, this content will be animated as part of the page transition.
            this.DisableContentTransitions();
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            this.UpdateForVisualState(e.NewState, e.OldState);
        }

        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == NarrowState;

            if (isNarrow && oldState == this.DefaultState && lastSelectedNote != null)
            {
                // Resize down to the detail item. Don't play a transition.
                Frame.Navigate(typeof(DetailPage), this.lastSelectedNote.FullName, new SuppressNavigationTransitionInfo());
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(MasterListView, isNarrow);
            if (this.DetailContentPresenter != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
            }
        }

        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (NoteViewModel)e.ClickedItem;
            lastSelectedNote = clickedItem;

            if (AdaptiveStates.CurrentState == NarrowState)
            {
                // Use "drill in" transition for navigating from master list to detail view
                Frame.Navigate(typeof(DetailPage), clickedItem.FullName, new DrillInNavigationTransitionInfo());
            }
            else
            {
                // Play a refresh animation when the user switches detail items.
                EnableContentTransitions();
            }
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            // Assure we are displaying the correct item. This is necessary in certain adaptive cases.
            MasterListView.SelectedItem = this.lastSelectedNote;
        }

        private void EnableContentTransitions()
        {
            this.DetailContentPresenter.ContentTransitions.Clear();
            this.DetailContentPresenter.ContentTransitions.Add(new EntranceThemeTransition());
        }

        private void DisableContentTransitions()
        {
            if (this.DetailContentPresenter != null)
            {
                this.DetailContentPresenter.ContentTransitions.Clear();
            }
        }

        private async void NoteText_LostFocus(object sender, RoutedEventArgs e)
        {
            // Do saves
            var notes = (NoteCollectionViewModel)this.MasterListView.ItemsSource;
            var save = notes.SaveAsync();
            await save;
        }

        private void NoteText_TextChanged(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var note = (NoteViewModel)textBox.DataContext;
            if (note != null)
            {
                note.Text = textBox.Text;
            }
        }

        private void MasterListView_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.F2)
            {
                // Do nothing for now
                //var item = this.MasterListView.SelectedItem;

                //var container = this.MasterListView.ContainerFromItem(item);
                //var elements = container.AllChildren();
                //var block = elements.First(c => c.Name == "titleBlock") as TextBlock;
                //var box = elements.First(c => c.Name == "titleBox") as TextBox;

                //block.Visibility = Visibility.Collapsed;
                //box.Visibility = Visibility.Visible;
            }
        }
    }
}
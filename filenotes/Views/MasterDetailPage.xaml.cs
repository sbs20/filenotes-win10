using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Sbs20.Filenotes.Extensions;
using Sbs20.Filenotes.ViewModels;
using Sbs20.Filenotes.Data;
using Windows.UI.Popups;

namespace Sbs20.Filenotes.Views
{
    public sealed partial class MasterDetailPage : Page
    {
        private NoteViewModel selectedNote;
        private NoteCollectionViewModel notes;

        public MasterDetailPage()
        {
            this.InitializeComponent();
            this.notes = new NoteCollectionViewModel();
            this.MasterListView.ItemsSource = this.notes;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.MasterListView.IsEnabled = false;
            await StorageManager.IsReady();
            this.MasterListView.IsEnabled = true;

            await this.notes.LoadAsync();

            if (e.Parameter != null)
            {
                // Parameter is item name
                var title = (string)e.Parameter;
                this.selectedNote = this.notes
                    .Where((item) => item.Name == title)
                    .FirstOrDefault();
            }

            this.SelectMostAppropriateNote();

            this.UpdateForVisualState(AdaptiveStates.CurrentState);

            VisualStateManager.GoToState(this, this.MasterState.Name, true);

            // Don't play a content transition for first item load.
            // Sometimes, this content will be animated as part of the page transition.
            this.DisableContentTransitions();
        }

        private void SelectMostAppropriateNote()
        {
            if (this.notes.Count > 0)
            {
                if (this.selectedNote != null && !this.MasterListView.Items.Contains(this.selectedNote))
                {
                    // If we're navigating back to this page then we reload all the notes from disk in which
                    // case we will have new references.
                    this.selectedNote = this.notes.Where(n => n.Name == this.selectedNote.Name).FirstOrDefault();
                }

                if (this.selectedNote == null)
                {
                    this.selectedNote = this.notes[0];
                }

                this.MasterListView.SelectedItem = this.selectedNote;
            }
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            this.UpdateForVisualState(e.NewState, e.OldState);
        }

        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == NarrowState;

            if (isNarrow && oldState == this.DefaultState && selectedNote != null)
            {
                // Resize down to the detail item. Don't play a transition.
                Frame.Navigate(typeof(DetailPage), this.selectedNote.Name, new SuppressNavigationTransitionInfo());
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(MasterListView, isNarrow);
            if (this.DetailContentPresenter != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
            }
        }

        private async void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.MasterDetailsStatesGroup.CurrentState != this.MultipleSelectionState)
            {
                this.selectedNote = (NoteViewModel)e.ClickedItem;

                // Force a reload of the file in case it's changed in the background
                try
                {
                    await this.selectedNote.ReloadAsync();
                }
                catch (InvalidOperationException ex)
                {
                    var dialog = new MessageDialog(ex.Message);
                    await dialog.ShowAsync();
                    await this.notes.LoadAsync();
                    this.SelectMostAppropriateNote();
                    return;
                }

                if (this.MasterListView.SelectedItems.Count == 1)
                {
                    if (AdaptiveStates.CurrentState == NarrowState)
                    {
                        // Use "drill in" transition for navigating from master list to detail view
                        Frame.Navigate(typeof(DetailPage), this.selectedNote.Name, new DrillInNavigationTransitionInfo());
                    }
                    else
                    {
                        // Play a refresh animation when the user switches detail items.
                        EnableContentTransitions();
                    }
                }
                else
                {

                }
            }
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            // Assure we are displaying the correct item. This is necessary in certain adaptive cases.
            this.MasterListView.SelectedItem = this.selectedNote;
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

        private async void NoteTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Do saves
            await this.notes.SaveAllAsync();
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            await this.notes.CreateNote();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            IList<NoteViewModel> toBeDeleted = new List<NoteViewModel>();

            // We have to make a temporary list of things to delete as doing so in the loop
            // will invalidate the IEnumerable
            foreach (NoteViewModel note in this.MasterListView.SelectedItems)
            {
                toBeDeleted.Add(note);
            }

            foreach (var note in toBeDeleted)
            {
                await this.notes.DeleteNote(note);
            }
        }

        private void RenameStart()
        {
            // First make the alternative header with the textbox visible
            var header = this.AllChildren().OfType<FrameworkElement>().Where(el => el.Name == "PageHeaderEdit").First();
            header.Visibility = Visibility.Visible;

            // And disable the list view to stop selecting other notes and confusing matters
            this.MasterListView.IsEnabled = false;
        }

        private async Task RenameFinish()
        {
            // Make the textbox invisible
            var header = this.AllChildren().OfType<FrameworkElement>().Where(el => el.Name == "PageHeaderEdit").First();
            header.Visibility = Visibility.Collapsed;

            // Get the textbox itself
            var box = (TextBox)header.AllChildren().OfType<TextBox>().First();
            var desiredName = box.Text;
            var note = this.selectedNote;

            if (note.Name != desiredName)
            {
                // Rename
                await this.notes.RenameNote(note, desiredName);
            }

            // Reenable the list view
            this.MasterListView.IsEnabled = true;

            // Reset the selected item, otherwise nothing is set. Seems to be lost following disablement
            this.MasterListView.SelectedItem = this.selectedNote;
            this.MasterListView.ScrollIntoView(this.selectedNote);
        }

        private void PageHeader_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.RenameStart();
        }

        private void MasterListView_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.F2)
            {
                // Stop this being handled again
                e.Handled = true;
                this.RenameStart();
            }
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

        private void Multiselect_Click(object sender, RoutedEventArgs e)
        {
            if (this.MasterListView.Items.Count > 0)
            {
                VisualStateManager.GoToState(this, this.MultipleSelectionState.Name, true);
                this.MasterListView.SelectedItem = null;
            }
        }

        private void CancelMultiselect_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, this.MasterState.Name, true);
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await this.notes.LoadAsync();
            this.SelectMostAppropriateNote();
        }
    }
}
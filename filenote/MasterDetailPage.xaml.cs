using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Sbs20.Filenote.Data;
using Sbs20.Filenote.ViewModels;

namespace Sbs20.Filenote
{
    public sealed partial class MasterDetailPage : Page
    {
        private NoteViewModel _lastSelectedNote;

        public MasterDetailPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var noteVms = this.MasterListView.ItemsSource as List<NoteViewModel>;

            if (noteVms == null)
            {
                noteVms = new List<NoteViewModel>();
                var notes = await NoteManager.GetAllItemsAsync();
                foreach (var note in notes)
                {
                    noteVms.Add(NoteViewModel.FromNote(note));
                }

                MasterListView.ItemsSource = noteVms;
            }

            if (e.Parameter != null)
            {
                // Parameter is item ID
                var id = (string)e.Parameter;
                _lastSelectedNote = noteVms
                    .Where((item) => item.FullName == id)
                    .FirstOrDefault();
            }
            
            UpdateForVisualState(AdaptiveStates.CurrentState);

            // Don't play a content transition for first item load.
            // Sometimes, this content will be animated as part of the page transition.
            DisableContentTransitions();
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateForVisualState(e.NewState, e.OldState);
        }

        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == NarrowState;

            if (isNarrow && oldState == DefaultState && _lastSelectedNote != null)
            {
                // Resize down to the detail item. Don't play a transition.
                Frame.Navigate(typeof(DetailPage), _lastSelectedNote.FullName, new SuppressNavigationTransitionInfo());
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(MasterListView, isNarrow);
            if (DetailContentPresenter != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
            }
        }

        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (NoteViewModel)e.ClickedItem;
            _lastSelectedNote = clickedItem;

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
            MasterListView.SelectedItem = _lastSelectedNote;
        }

        private void EnableContentTransitions()
        {
            DetailContentPresenter.ContentTransitions.Clear();
            DetailContentPresenter.ContentTransitions.Add(new EntranceThemeTransition());
        }

        private void DisableContentTransitions()
        {
            if (DetailContentPresenter != null)
            {
                DetailContentPresenter.ContentTransitions.Clear();
            }
        }

        private void NoteText_LostFocus(object sender, RoutedEventArgs e)
        {
            // Do saves
        }

        private void NoteText_TextChanged(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var note = (NoteViewModel)textBox.DataContext;
            note.Text = textBox.Text;
            note.IsDirty = true;
        }
    }
}

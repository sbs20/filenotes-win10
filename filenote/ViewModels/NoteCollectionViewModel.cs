using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Sbs20.Filenote.Data;

namespace Sbs20.Filenote.ViewModels
{
    public class NoteCollectionViewModel : ObservableCollection<NoteViewModel>
    {
        public bool IsListening { get; set; }

        private NoteCollectionViewModel()
        {
            this.IsListening = false;
            this.CollectionChanged += NoteCollectionViewModel_CollectionChanged;
        }

        private bool IsExistingTitle(string title)
        {
            return this.FirstOrDefault(n => n.Title.Equals(title, StringComparison.OrdinalIgnoreCase)) != null;
        }

        public string CreateNewUniqueName()
        {
            const string stem = "_New{0}.txt";
            string attempt = string.Format(stem, "");
            int i = 0;
            while (this.IsExistingTitle(attempt))
            {
                ++i;
                attempt = string.Format(stem, i);
            }

            return attempt;
        }

        private async void NoteCollectionViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsListening)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (NoteViewModel note in e.NewItems)
                        {
                            await NoteManager.CreateNoteAsync(note.ToNote());
                        }

                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (NoteViewModel note in e.OldItems)
                        {
                            await NoteManager.DeleteNoteAsync(note.ToNote());
                        }

                        break;
                }
            }
        }

        public static async Task<NoteCollectionViewModel> LoadAsync()
        {
            NoteCollectionViewModel items = new NoteCollectionViewModel();
            var notes = await NoteManager.GetAllItemsAsync();
            foreach (var note in notes)
            {
                items.Add(NoteViewModel.Create(note));
            }

            return items;
        }

        public async Task SaveAsync()
        {
            foreach (var note in this)
            {
                await note.SyncNoteViewModelAsync();
            }
        }

        private void InsertInOrder(NoteViewModel note)
        {
            var firstNote = this.FirstOrDefault(nvm => nvm.Title.CompareTo(note.Title) > 0);
            if (firstNote == null)
            {
                this.Add(note);
            }
            else
            {
                int index = this.IndexOf(firstNote);
                this.Insert(index, note);
            }
        }

        public void CreateNew()
        {
            string name = this.CreateNewUniqueName();
            var note = new NoteViewModel
            {
                DateCreated = DateTime.Now,
                Title = name,
                FullName = name,
                Text = string.Empty
            };

            this.InsertInOrder(note);
        }
    }
}
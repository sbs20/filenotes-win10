using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Sbs20.Filenote.Data;

namespace Sbs20.Filenote.ViewModels
{
    public class NoteCollectionViewModel : ObservableCollection<NoteViewModel>
    {
        private NoteCollectionViewModel()
        {
            this.CollectionChanged += NoteCollectionViewModel_CollectionChanged;
        }

        private bool IsExistingTitle(string title)
        {
            return this.FirstOrDefault(n => n.Name.Equals(title, StringComparison.OrdinalIgnoreCase)) != null;
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

        private void NoteCollectionViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public static async Task<NoteCollectionViewModel> LoadAsync()
        {
            NoteCollectionViewModel items = new NoteCollectionViewModel();
            var notes = await StorageManager.GetAllNotesAsync();
            foreach (var note in notes)
            {
                items.Add(note as NoteViewModel);
            }

            return items;
        }

        public async Task SaveAllAsync()
        {
            foreach (var note in this)
            {
                await note.SyncNoteViewModelAsync();
            }
        }

        private void InsertInOrder(NoteViewModel note)
        {
            var firstNote = this.FirstOrDefault(nvm => nvm.Name.CompareTo(note.Name) > 0);
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

        public async Task CreateNote()
        {
            string name = this.CreateNewUniqueName();
            var note = new NoteViewModel
            {
                DateCreated = DateTime.Now,
                Name = name,
                Text = string.Empty
            };

            this.InsertInOrder(note);
            await StorageManager.CreateNoteAsync(note);
        }

        public async Task RenameNote(NoteViewModel note, string desiredName)
        {
            note.Name = await StorageManager.RenameNoteAsync(note, desiredName);
            this.Remove(note);
            this.InsertInOrder(note);
        }
    
        public async Task DeleteNote(NoteViewModel note)
        {
            this.Remove(note);
            await StorageManager.DeleteNoteAsync(note);
        }
    }
}
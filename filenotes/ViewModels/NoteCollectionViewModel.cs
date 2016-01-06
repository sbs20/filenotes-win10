using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Sbs20.Filenote.Data;

namespace Sbs20.Filenote.ViewModels
{
    public class NoteCollectionViewModel : ObservableCollection<NoteViewModel>
    {
        public NoteCollectionViewModel()
        {
        }

        private bool IsExistingName(string name)
        {
            return this.FirstOrDefault(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) != null;
        }

        public string CreateNewUniqueName()
        {
            const string stem = "_New{0}.txt";
            string attempt = string.Format(stem, "");
            int i = 0;
            while (this.IsExistingName(attempt))
            {
                ++i;
                attempt = string.Format(stem, i);
            }

            return attempt;
        }

        public async Task LoadAsync()
        {
            this.Clear();
            var notes = await StorageManager.LoadNotesAsync();
            foreach (var note in notes)
            {
                this.Add(note as NoteViewModel);
            }
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
                DateModified = DateTime.Now,
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
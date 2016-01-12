using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Sbs20.Filenotes.Data;

namespace Sbs20.Filenotes.ViewModels
{
    public class NoteCollection : ObservableCollection<Note>
    {
        public NoteCollection()
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
                this.Add(note as Note);
            }
        }

        public async Task SaveAllAsync()
        {
            foreach (var note in this)
            {
                await note.SaveAsync();
            }
        }

        private void InsertInOrder(Note note)
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
            var note = new Note
            {
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                Name = name,
                Text = string.Empty
            };

            this.InsertInOrder(note);
            await StorageManager.CreateNoteAsync(note);
        }

        public async Task RenameNote(Note note, string desiredName)
        {
            note.Name = await StorageManager.RenameNoteAsync(note, desiredName);
            this.Remove(note);
            this.InsertInOrder(note);
        }
    
        public async Task DeleteNote(Note note)
        {
            this.Remove(note);
            await StorageManager.DeleteNoteAsync(note);
        }
    }
}
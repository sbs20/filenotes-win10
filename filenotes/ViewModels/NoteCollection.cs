using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sbs20.Filenotes.ViewModels
{
    public class NoteCollection : ObservableCollection<Note>
    {
        private static NoteCollection instance = null;

        public static NoteCollection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NoteCollection();
                }

                return instance;
            }
        }

        private NoteCollection()
        {
        }

        public bool IsExistingName(string name)
        {
            return this.FirstOrDefault(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) != null;
        }

        public void InsertInOrder(Note note)
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

        public Note GetByName(string name)
        {
            return this.Where(n => n.Name == name).FirstOrDefault();
        }
    }
}
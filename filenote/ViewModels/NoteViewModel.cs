using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sbs20.Filenote.Data;
using Windows.Globalization.DateTimeFormatting;

namespace Sbs20.Filenote.ViewModels
{
    public class NoteViewModel
    {
        private string originalContent;

        public string FullName { get; private set; }

        public string DateCreatedHourMinute
        {
            get
            {
                var formatter = new DateTimeFormatter("hour minute");
                return formatter.Format(DateCreated);
            }
        }

        public string DateCreatedFull
        {
            get
            {
                return this.DateCreated.ToString("dd MMM yyyy hh:mm:ss");
            }
        }

        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }

        public bool IsDirty
        {
            get { return this.originalContent != this.Text; }
        }

        public NoteViewModel()
        {
        }

        public static NoteViewModel FromNote(Note note)
        {
            return new NoteViewModel()
            {
                FullName = note.FullName,
                DateCreated = note.DateCreated,
                Title = note.Title,
                Text = note.Text,
                originalContent = note.Text
            };
        }

        public Note ToNote()
        {
            return new Note
            {
                FullName = this.FullName,
                DateCreated = this.DateCreated,
                Title = this.Title,
                Text = this.Text
            };
        }

        public async Task SyncNoteViewModelAsync()
        {
            if (this.IsDirty)
            {
                await NoteManager.SaveNoteAsync(this.ToNote());
                this.originalContent = this.Text;
            }
        }

        public static async Task SaveNotesViewModelsAsync(IEnumerable<NoteViewModel> notes)
        {
            foreach (var note in notes)
            {
                await note.SyncNoteViewModelAsync();
            }
        }
    }
}

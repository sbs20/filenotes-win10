using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Sbs20.Filenote.Data;
using Windows.Globalization.DateTimeFormatting;

namespace Sbs20.Filenote.ViewModels
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        private string title;
        private string originalContent;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public string Title
        {
            get { return this.title; }
            set
            {
                this.title = value;
                this.OnPropertyChanged("Title");
            }
        }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }

        public NoteViewModel()
        {
        }

        public static NoteViewModel Create(Note note)
        {
            return new NoteViewModel()
            {
                DateCreated = note.DateCreated,
                Title = note.Title,
                Text = note.Text,
                originalContent = note.Text
            };
        }


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

        public bool IsDirty
        {
            get { return this.originalContent != this.Text; }
        }

        public Note ToNote()
        {
            return new Note
            {
                DateCreated = this.DateCreated,
                Title = this.Title,
                Text = this.Text
            };
        }

        public async Task SyncNoteViewModelAsync()
        {
            if (this.IsDirty)
            {
                var save = NoteManager.SaveNoteAsync(this.ToNote());
                this.originalContent = this.Text;
                this.OnPropertyChanged("Text");
                await save;
            }
        }

    }
}

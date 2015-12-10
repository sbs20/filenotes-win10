using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Sbs20.Filenote.Data;
using Windows.Globalization.DateTimeFormatting;

namespace Sbs20.Filenote.ViewModels
{
    public class NoteViewModel : INotifyPropertyChanged, INote
    {
        private string name;
        private string text;
        private string originalText;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.OnPropertyChanged("Title");
            }
        }

        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                this.OnPropertyChanged("Text");
                if (this.originalText == null)
                {
                    this.originalText = this.text;
                }
            }
        }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public NoteViewModel()
        {
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
            get { return this.originalText != this.Text; }
        }

        public async Task SyncNoteViewModelAsync()
        {
            if (this.IsDirty)
            {
                var save = StorageManager.SaveNoteAsync(this);
                this.originalText = this.Text;
                await save;
            }
        }
    }
}

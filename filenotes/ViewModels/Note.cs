using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Sbs20.Filenotes.Data;
using Windows.Globalization.DateTimeFormatting;

namespace Sbs20.Filenotes.ViewModels
{
    public class Note : INotifyPropertyChanged
    {
        private string name;
        private string text;
        private string originalText;
        private DateTime dateModified;

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
                this.OnPropertyChanged("Name");
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

        public DateTime DateModified
        {
            get { return this.dateModified; }
            set
            {
                this.dateModified = value;
                this.OnPropertyChanged("DateModified");
                this.OnPropertyChanged("DateModifiedString");
            }
        }

        public DateTime DateCreated { get; set; }

        public Note()
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

        public string DateCreatedString
        {
            get { return this.DateCreated.ToString("g"); }
        }

        public string DateModifiedString
        {
            get { return this.DateModified.ToString("g"); }
        }

        public bool IsDirty
        {
            get { return this.originalText != this.Text; }
        }

        public void Reset()
        {
            this.originalText = this.text;
        }
    }
}

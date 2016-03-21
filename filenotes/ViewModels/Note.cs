using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Sbs20.Filenotes.Data;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI.Xaml;

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
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        public string Text
        {
            get { return this.text; }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged("Text");
                }
            }
        }

        private string OriginalText
        {
            get { return this.originalText; }
            set
            {
                if (this.originalText != value)
                {
                    this.originalText = value;
                    this.OnPropertyChanged("SizeString");
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

        public string SizeString
        {
            get
            {
                // This isn't exactly correct - but to be honest, it's close enough for now
                long size = this.originalText == null ? 0 : this.originalText.Length;
                long kb = 1 << 10;
                long mb = kb << 10;

                if (size < 0)
                {
                    return "0";
                }
                else if (size == 1)
                {
                    return "1 Byte";
                }
                else if (size < kb << 1)
                {
                    return size + " Bytes";
                }
                else if (size < mb << 1)
                {
                    return ((int)(size / kb)) + " KB";
                }
                else
                {
                    return Math.Round(100.0 * size / mb) / 100.0 + " MB";
                }
            }
        }

        public bool IsDirty
        {
            get { return this.OriginalText != this.Text; }
        }

        public void MarkAsClean()
        {
            this.OriginalText = this.text;
        }

        public Visibility SaveButtonVisibility
        {
            get { return Settings.IsSaveButtonVisible ? Visibility.Visible : Visibility.Collapsed; }
        }
    }
}

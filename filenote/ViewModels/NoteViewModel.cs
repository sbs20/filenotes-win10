using System;
using Sbs20.Filenote.Data;
using Windows.Globalization.DateTimeFormatting;

namespace Sbs20.Filenote.ViewModels
{
    public class NoteViewModel
    {
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
        public bool IsDirty { get; set; }

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
                Text = note.Text
            };
        }
    }
}

using System;

namespace Sbs20.Filenote.Data
{
    public class Note
    {
        public string FullName { get; set; }
        public DateTime DateCreated { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}

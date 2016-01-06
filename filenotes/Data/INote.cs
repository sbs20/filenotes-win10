using System;

namespace Sbs20.Filenote.Data
{
    public interface INote
    {
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
        string Name { get; set; }
        string Text { get; set; }
    }
}

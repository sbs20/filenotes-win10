using System;
using System.Threading.Tasks;

namespace Sbs20.Filenotes.Data
{
    public interface INote
    {
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
        string Name { get; set; }
        string Text { get; set; }
        Task ReloadAsync();
        Task SaveAsync();
    }
}

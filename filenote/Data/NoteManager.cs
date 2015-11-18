using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Sbs20.Filenote.Data
{
    public class NoteManager
    {
        private static List<Note> cache = null;

        public static async Task<IList<Note>> GetAllItemsAsync()
        {
            if (cache == null)
            {
                var folder = await Settings.GetStorageFolderAsync();
                var files = await folder.GetFilesAsync();
                var notes = files
                    .Select(async (f) =>
                    {
                        return new Note
                        {
                            FullName = f.Name,
                            DateCreated = f.DateCreated.LocalDateTime,
                            Title = f.Name,
                            Text = await FileIO.ReadTextAsync(f)
                        };
                    });

                cache = new List<Note>(await Task.WhenAll(notes));
            }

            return cache ?? new List<Note>();
        }

        public static async Task<Note> GetItemByIdAsync(string id)
        {
            var notes = await GetAllItemsAsync();
            return notes.Where(n => n.FullName == id).FirstOrDefault();
        }
    }
}

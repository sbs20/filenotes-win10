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
                            DateCreated = f.DateCreated.LocalDateTime,
                            Title = f.Name,
                            Text = await FileIO.ReadTextAsync(f)
                        };
                    });

                cache = new List<Note>(await Task.WhenAll(notes));
            }

            return cache ?? new List<Note>();
        }

        public static async Task<Note> GetItemByTitleAsync(string title)
        {
            var notes = await GetAllItemsAsync();
            return notes.Where(n => n.Title == title).FirstOrDefault();
        }

        public static async Task SaveNoteAsync(Note note)
        {
            var folder = await Settings.GetStorageFolderAsync();
            StorageFile file = await folder.GetFileAsync(note.Title);
            await FileIO.WriteTextAsync(file, note.Text);
        }

        public static async Task<string> CreateNoteAsync(Note note)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.CreateFileAsync(note.Title, CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteTextAsync(file, note.Text);
            return file.Name;
        }

        public static async Task DeleteNoteAsync(Note note)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(note.Title);
            await file.DeleteAsync(StorageDeleteOption.Default);
        }

        public static async Task<string> RenameNoteAsync(Note note, string desiredName)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(note.Title);
            await file.RenameAsync(desiredName, NameCollisionOption.GenerateUniqueName);
            return file.Name;
        }
    }
}

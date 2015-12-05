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

        public static async Task<Note> GetItemByFullNameAsync(string fullName)
        {
            var notes = await GetAllItemsAsync();
            return notes.Where(n => n.FullName == fullName).FirstOrDefault();
        }

        public static async Task SaveNoteAsync(Note note)
        {
            var folder = await Settings.GetStorageFolderAsync();
            StorageFile file = await folder.GetFileAsync(note.FullName);
            await FileIO.WriteTextAsync(file, note.Text);
        }

        public static async Task CreateNoteAsync(Note note)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.CreateFileAsync(note.FullName, CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteTextAsync(file, note.Text);
        }

        public static async Task DeleteNoteAsync(Note note)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(note.FullName);
            await file.DeleteAsync(StorageDeleteOption.Default);
        }

        public static async Task RenameNoteAsync(Note note, string desiredName)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(note.FullName);
            await file.RenameAsync(desiredName);
        }
    }
}

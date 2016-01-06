using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Sbs20.Filenote.Data
{
    public class StorageManager
    {
        private static List<INote> cache = null;

        public static void ClearCache()
        {
            cache = null;
        }

        public static async Task<IList<INote>> GetAllNotesAsync()
        {
            if (cache == null)
            {
                var folder = await Settings.GetStorageFolderAsync();
                var files = await folder.GetFilesAsync();
                var notes = files
                    .Select(async (f) =>
                    {
                        INote note = Settings.CreateNote();
                        note.DateCreated = f.DateCreated.LocalDateTime;
                        BasicProperties properties =await f.GetBasicPropertiesAsync();
                        note.DateModified = properties.DateModified.LocalDateTime;
                        note.Name = f.Name;
                        note.Text = await FileIO.ReadTextAsync(f);
                        return note;
                    });

                cache = new List<INote>(await Task.WhenAll(notes));
            }

            return cache ?? new List<INote>();
        }

        public static async Task<INote> GetNoteByNameAsync(string name)
        {
            var notes = await GetAllNotesAsync();
            return notes.Where(n => n.Name == name).FirstOrDefault();
        }

        public static async Task SaveNoteAsync(INote note)
        {
            var folder = await Settings.GetStorageFolderAsync();
            StorageFile file = await folder.GetFileAsync(note.Name);
            await FileIO.WriteTextAsync(file, note.Text);
        }

        public static async Task<string> CreateNoteAsync(INote note)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.CreateFileAsync(note.Name, CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteTextAsync(file, note.Text);
            return file.Name;
        }

        public static async Task DeleteNoteAsync(INote note)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(note.Name);
            await file.DeleteAsync(StorageDeleteOption.Default);
        }

        public static async Task<string> RenameNoteAsync(INote note, string desiredName)
        {
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(note.Name);
            await file.RenameAsync(desiredName, NameCollisionOption.GenerateUniqueName);
            return file.Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Sbs20.Filenotes.Data
{
    public class StorageManager
    {
        private static int busyStack = 0;

        public static async Task IsReady()
        {
            while (busyStack > 0)
            {
                await Task.Delay(100);
            }
        }

        public static async Task<IList<INote>> LoadNotesAsync()
        {
            busyStack++;
            var folder = await Settings.GetStorageFolderAsync();
            var files = await folder.GetFilesAsync();
            var notes = files
                .Select(async (f) =>
                {
                    INote note = Settings.CreateNote();
                    note.DateCreated = f.DateCreated.LocalDateTime;
                    BasicProperties properties = await f.GetBasicPropertiesAsync();
                    note.DateModified = properties.DateModified.LocalDateTime;
                    note.Name = f.Name;
                    note.Text = await FileIO.ReadTextAsync(f);
                    return note;
                });

            var list = new List<INote>(await Task.WhenAll(notes));
            busyStack--;
            return list;
        }

        public static async Task<INote> LoadNoteAsync(string name)
        {
            busyStack++;
            var notes = await LoadNotesAsync();
            busyStack--;
            return notes.Where(n => n.Name == name).FirstOrDefault();
        }

        public static async Task SaveNoteAsync(INote note)
        {
            busyStack++;
            var folder = await Settings.GetStorageFolderAsync();
            StorageFile file = await folder.GetFileAsync(note.Name);
            await FileIO.WriteTextAsync(file, note.Text);
            busyStack--;
        }

        public static async Task<string> CreateNoteAsync(INote note)
        {
            busyStack++;
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.CreateFileAsync(note.Name, CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteTextAsync(file, note.Text);
            busyStack--;
            return file.Name;
        }

        public static async Task DeleteNoteAsync(INote note)
        {
            busyStack++;
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(note.Name);
            await file.DeleteAsync(StorageDeleteOption.Default);
            busyStack--;
        }

        public static async Task<string> RenameNoteAsync(INote note, string desiredName)
        {
            busyStack++;
            var folder = await Settings.GetStorageFolderAsync();
            var file = await folder.GetFileAsync(note.Name);
            await file.RenameAsync(desiredName, NameCollisionOption.GenerateUniqueName);
            busyStack--;
            return file.Name;
        }
    }
}

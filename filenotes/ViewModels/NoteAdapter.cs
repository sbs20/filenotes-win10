using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Sbs20.Filenotes.Data;

namespace Sbs20.Filenotes.ViewModels
{
    public static class NoteAdapter
    {
        public static NoteCollection Notes
        {
            get { return NoteCollection.Instance; }
        }

        private static async Task MergeFileIntoNote(StorageFile file, Note note)
        {
            note.DateCreated = file.DateCreated.LocalDateTime;
            BasicProperties properties = await file.GetBasicPropertiesAsync();
            note.DateModified = properties.DateModified.LocalDateTime;
            note.Name = file.Name;
            note.Text = await FileIO.ReadTextAsync(file);
        }

        public static async Task TryReadAllFromStorageAsync()
        {
            // Sometimes other saves will be going on. And we have a reasonably
            // aggressive reload strategy going on - so rather than deal with
            // serious marshalling just try it and don't worry too much
            try
            {
                var files = await StorageManager.LoadFilesAsync();

                // Ensure all files are in notes and up to date
                foreach (var file in files)
                {
                    var note = Notes.GetByName(file.Name);
                    if (note == null)
                    {
                        note = new Note { Name = file.Name };
                        Notes.InsertInOrder(note);
                    }

                    await MergeFileIntoNote(file, note);
                }

                // Now ensure that any notes NOT in a file is removed
                for (int index = 0; index < Notes.Count; index++)
                {
                    var note = Notes[index];
                    var file = files.Where(f => f.Name == note.Name).FirstOrDefault();
                    if (file == null)
                    {
                        Notes.RemoveAt(index);
                    }
                }
            }
            catch { }
        }

        public static async Task WriteAllToStorageAsync()
        {
            foreach (var note in Notes)
            {
                await NoteAdapter.WriteToStorageAsync(note);
            }
        }

        public static async Task WriteToStorageAsync(Note note)
        {
            if (note.IsDirty)
            {
                var save = StorageManager.SaveFileAsync(note.Name, note.Text);
                note.Reset();
                await save;
                note.DateModified = DateTime.Now;
            }
        }

        public static async Task RenameNoteAsync(Note note, string desiredName)
        {
            note.Name = await StorageManager.RenameFileAsync(note.Name, desiredName);;
            Notes.Remove(note as Note);
            Notes.InsertInOrder(note as Note);
        }

        public static async Task DeleteNoteAsync(Note note)
        {
            await StorageManager.DeleteFileAsync(note.Name);
            Notes.Remove(note as Note);
        }

        private static string CreateNewUniqueName()
        {
            const string stem = "_New{0}.txt";
            string attempt = string.Format(stem, "");
            int i = 0;
            while (Notes.IsExistingName(attempt))
            {
                ++i;
                attempt = string.Format(stem, i);
            }

            return attempt;
        }
        
        public static async Task CreateNoteAsync()
        {
            string name = CreateNewUniqueName();
            var note = new Note
            {
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                Name = name,
                Text = string.Empty
            };

            Notes.InsertInOrder(note);
            await StorageManager.CreateFileAsync(note.Name, note.Text);
        }
    }
}

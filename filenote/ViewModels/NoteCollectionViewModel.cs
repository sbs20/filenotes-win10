using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Sbs20.Filenote.Data;

namespace Sbs20.Filenote.ViewModels
{
    public class NoteCollectionViewModel : IEnumerable<NoteViewModel>, INotifyCollectionChanged
    {
        private IList<NoteViewModel> list;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, args);
            }
        }

        private NoteCollectionViewModel()
        {
            this.list = new List<NoteViewModel>();
        }

        public static async Task<NoteCollectionViewModel> LoadAsync()
        {
            NoteCollectionViewModel items = new NoteCollectionViewModel();
            var notes = await NoteManager.GetAllItemsAsync();
            foreach (var note in notes)
            {
                items.Add(NoteViewModel.Create(note));
            }

            return items;
        }

        public async Task SaveAsync()
        {
            foreach (var note in this)
            {
                await note.SyncNoteViewModelAsync();
            }
        }

        public void Add(NoteViewModel item)
        {
            this.list.Add(item);
            this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public IEnumerator<NoteViewModel> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
}

//public void Add(CollectionItem item)
//{
//    this._lstItems.Add(item);
//    this.OnNotifyCollectionChanged(
//      new NotifyCollectionChangedEventArgs(
//        NotifyCollectionChangedAction.Add, item));
//}

//public void Remove(CollectionItem item)
//{
//    this._lstItems.Remove(item);
//    this.OnNotifyCollectionChanged(
//      new NotifyCollectionChangedEventArgs(
//        NotifyCollectionChangedAction.Remove, item));
//}

//// ... other actions for the collection ...

//public CollectionItem this[Int32 index]
//{
//    get
//    {
//        return this._lstItems[index];
//    }
//}

//#region INotifyCollectionChanged
//private void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
//{
//    if (this.CollectionChanged != null)
//    {
//        this.CollectionChanged(this, args);
//    }
//}
//public event NotifyCollectionChangedEventHandler CollectionChanged;
//#endregion INotifyCollectionChanged

//#region IEnumerable
//public List<CollectionItem>.Enumerator GetEnumerator()
//{
//    return this._lstItems.GetEnumerator();
//}
//IEnumerator IEnumerable.GetEnumerator()
//{
//    return (IEnumerator)this.GetEnumerator();
//}
//#endregion IEnumerable
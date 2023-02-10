using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BfevLibrary.Core.Collections
{
    public class BfevList<T> : ObservableCollection<T> where T : BfevListItem
    {
        private readonly Flowchart _parent;

        public BfevList(Flowchart parent)
        {
            _parent = parent;
            CollectionChanged += BfevList_CollectionChanged;
        }

        public BfevList(Flowchart parent, IEnumerable<T> items) : base(items)
        {
            _parent = parent;
            CollectionChanged += BfevList_CollectionChanged;
        }

        private void BfevList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null) {
                foreach (var item in e.NewItems) {
                    ((BfevListItem)item)._parent = _parent;
                }
            }
        }
    }
}

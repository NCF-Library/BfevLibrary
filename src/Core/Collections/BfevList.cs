using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BfevLibrary.Core.Collections;

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

    private void BfevList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null) {
            foreach (var item in e.NewItems) {
                ((BfevListItem)item)._parent = _parent;
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null) {
            bool isActorList = typeof(T) == typeof(Actor);
            foreach (var @event in _parent.Events) {
                if (isActorList) {
                    @event.AlterActorIndex(e.OldStartingIndex);
                }
                else {
                    @event.AlterEventIndex(e.OldStartingIndex);
                }
            }
        }
    }
}

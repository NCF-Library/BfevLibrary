using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BfevLibrary.Core.Collections;

public class EventList : ObservableCollection<Event>
{
    private readonly Flowchart _parent;

    public EventList(Flowchart parent)
    {
        _parent = parent;
        CollectionChanged += EventList_CollectionChanged;
    }

    public EventList(Flowchart parent, IEnumerable<Event> items) : base(items)
    {
        _parent = parent;
        CollectionChanged += EventList_CollectionChanged;
    }

    public void Remove(Event item, bool recursive) => RemoveInternal(item, IndexOf(item), recursive);
    public void RemoveAt(int index, bool recursive) => RemoveInternal(this[index], index, recursive);
    private void RemoveInternal(Event item, int index, bool _)
    {
        RemoveAt(index);

        List<int> children = item.GetChildIndices();
        foreach (var child in children.Distinct().OrderDescending()) {
            RemoveAt(child);
        }
    }

    private void EventList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null) {
            foreach (var item in e.NewItems) {
                ((BfevListItem)item)._parent = _parent;
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null) {
            foreach (var @event in _parent.Events) {
                @event.AlterEventIndex(e.OldStartingIndex);
            }

            foreach ((_, var entryPoint) in _parent.EntryPoints) {
                AlterEntryPointIndices(e.OldStartingIndex, entryPoint);
            }
        }
    }

    private void AlterEntryPointIndices(int index, EntryPoint entryPoint)
    {
        if (index < entryPoint.EventIndex) {
            entryPoint.EventIndex--;
        }
        else if (index == entryPoint.EventIndex) {
            entryPoint.EventIndex = -1;
        }
    }
}

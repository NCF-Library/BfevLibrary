using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core.Collections;

public class EventList : ObservableCollection<Event>
{
    internal Flowchart _parent;

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

    [JsonConstructor]
    public EventList() => CollectionChanged += EventList_CollectionChanged;

    /// <summary>
    /// Renames every event to match there index
    /// </summary>
    public void RemapEventNames()
    {
        for (int i = 0; i < Count; i++) {
            this[i].Name = "Event" + i;
        }
    }

    public void Remove(Event item, bool recursive) => RemoveInternal(item, IndexOf(item), recursive);
    public void RemoveAt(int index, bool recursive) => RemoveInternal(this[index], index, recursive);
    internal void RemoveInternal(Event item, int index, bool _, List<int>? ignoreIndices = null)
    {
        List<int> indices = new();
        item.GetIndices(indices, index, -1, ignoreIndices);

        List<int> tmp = indices.Distinct().OrderDescending().ToList();
        foreach (var child in tmp) {
            RemoveAt(child);
        }
    }

    internal void RemapEntryPointSubflowIndices()
    {
        foreach ((_, var entryPoint) in _parent.EntryPoints) {
            entryPoint.SubFlowEventIndices.Clear();

            List<int> indices = new();
            Event @event = Items[entryPoint.EventIndex];
            @event.GetIndices(indices, entryPoint.EventIndex, -1);

            foreach (var index in indices) {
                if (Items[index] is SubflowEvent) {
                    entryPoint.SubFlowEventIndices.Add((short)index);
                }
            }
        }
    }

    private void EventList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null) {
            foreach (var item in e.NewItems) {
                ((Event)item)._parent = _parent;
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

    private static void AlterEntryPointIndices(int index, EntryPoint entryPoint)
    {
        if (index < entryPoint.EventIndex) {
            entryPoint.EventIndex--;
        }
        else if (index == entryPoint.EventIndex) {
            entryPoint.EventIndex = -1;
        }
    }
}

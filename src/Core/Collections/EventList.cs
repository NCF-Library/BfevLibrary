﻿using System.Collections.ObjectModel;
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

    public void Remove(Event item, bool recursive) => RemoveInternal(item, IndexOf(item), recursive);
    public void RemoveAt(int index, bool recursive) => RemoveInternal(this[index], index, recursive);
    private void RemoveInternal(Event item, int index, bool _)
    {
        item.GetIndices(_parent.CachedIndices, index);

        foreach (var child in _parent.CachedIndices.Distinct().OrderDescending()) {
            RemoveAt(child);
        }
    }

    internal void ResetEntryPointIndices()
    {
        foreach ((_, var entryPoint) in _parent.EntryPoints) {
            entryPoint.SubFlowEventIndices.Clear();
        }

        for (short i = 0; i < Count; i++) {
            if (Items[i] is SubflowEvent subflow && _parent.EntryPoints.TryGetValue(subflow.EntryPointName, out EntryPoint? entryPoint)) {
                entryPoint.SubFlowEventIndices.Add(i);
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

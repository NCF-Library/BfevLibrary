using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core.Collections;

public class ActorList : ObservableCollection<Actor>
{
    internal Flowchart _parent;

    public ActorList(Flowchart parent)
    {
        _parent = parent;
        CollectionChanged += ActorList_CollectionChanged;
    }

    public ActorList(Flowchart parent, IEnumerable<Actor> items) : base(items)
    {
        _parent = parent;
        CollectionChanged += ActorList_CollectionChanged;
    }

    [JsonConstructor]
    public ActorList() => CollectionChanged += ActorList_CollectionChanged;

    private void ActorList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null) {
            foreach (var @event in _parent.Events) {
                @event.AlterActorIndex(e.OldStartingIndex);
            }
        }
    }
}

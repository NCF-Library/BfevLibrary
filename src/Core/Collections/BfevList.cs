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
                    AlterActorIndices(@event, e.OldStartingIndex);
                }
                else {
                    AlterEventIndices(@event, e.OldStartingIndex);
                }
            }
        }
    }

    private static void AlterActorIndices(Event @event, int index)
    {
        if (@event is ActionEvent actionEvent) {
            if (index < actionEvent.ActorIndex) {
                actionEvent.ActorIndex--;
            }
            else if (index == actionEvent.ActorIndex) {
                actionEvent.ActorIndex = -1;
            }
        }
        else if (@event is SwitchEvent switchEvent) {
            if (index < switchEvent.ActorIndex) {
                switchEvent.ActorIndex--;
            }
            else if (index == switchEvent.ActorIndex) {
                switchEvent.ActorIndex = -1;
            }
        }
    }

    private static void AlterEventIndices(Event @event, int index)
    {
        if (@event is ActionEvent actionEvent) {
            if (index < actionEvent.NextEventIndex) {
                actionEvent.NextEventIndex--;
            }
            else if (index == actionEvent.NextEventIndex) {
                actionEvent.NextEventIndex = -1;
            }
        }
        else if (@event is ForkEvent forkEvent) {
            if (index < forkEvent.JoinEventIndex) {
                forkEvent.JoinEventIndex--;
            }
            else if (index == forkEvent.JoinEventIndex) {
                forkEvent.JoinEventIndex = -1;
            }

            for (int i = 0; i < forkEvent.ForkEventIndicies.Count; i++) {
                if (index < forkEvent.ForkEventIndicies[i]) {
                    forkEvent.ForkEventIndicies[i]--;
                }
                else if (index == forkEvent.ForkEventIndicies[i]) {
                    forkEvent.ForkEventIndicies.RemoveAt(i);
                    i++;
                }
            }
        }
        else if (@event is JoinEvent joinEvent) {
            if (index < joinEvent.NextEventIndex) {
                joinEvent.NextEventIndex--;
            }
            else if (index == joinEvent.NextEventIndex) {
                joinEvent.NextEventIndex = -1;
            }
        }
        else if (@event is SubflowEvent subflowEvent) {
            if (index < subflowEvent.NextEventIndex) {
                subflowEvent.NextEventIndex--;
            }
            else if (index == subflowEvent.NextEventIndex) {
                subflowEvent.NextEventIndex = -1;
            }
        }
        else if (@event is SwitchEvent switchEvent) {
            for (int i = 0; i < switchEvent.SwitchCases.Count; i++) {
                if (index < switchEvent.SwitchCases[i].EventIndex) {
                    switchEvent.SwitchCases[i].EventIndex--;
                }
                else if (index == switchEvent.SwitchCases[i].EventIndex) {
                    switchEvent.SwitchCases.RemoveAt(i);
                    i++;
                }
            }
        }
    }
}

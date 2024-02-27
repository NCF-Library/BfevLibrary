using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core;

public class ActionEvent : Event, IBfevDataBlock
{
    public short NextEventIndex { get; set; } = -1;
    public short ActorIndex { get; set; }
    public short ActorActionIndex { get; set; }
    public Container? Parameters { get; set; }

    [JsonIgnore]
    public Event? NextEvent => NextEventIndex > -1 ? Flowchart?.Events[NextEventIndex] : null;
    public string? NextEventName => NextEvent?.Name;

    [JsonIgnore]
    public Actor? Actor => ActorIndex > -1 ? Flowchart?.Actors[ActorIndex] : null;
    public string? ActorName => Actor?.Name;
    public string? ActorAction => ActorActionIndex > -1 ? Actor?.Actions[ActorActionIndex] : null;

    [JsonConstructor]
    public ActionEvent(string name) : base(name, EventType.Action)
    {
        Parameters = new();
    }

    public ActionEvent(BfevReader reader) : base(reader)
    {
        NextEventIndex = reader.ReadInt16();
        ActorIndex = reader.ReadInt16();
        ActorActionIndex = reader.ReadInt16();
        Parameters = reader.ReadObjectPtr<Container>(() => new(reader));
        reader.BaseStream.Position += 8 + 8; // unused pointers
    }

    public new void Write(BfevWriter writer)
    {
        base.Write(writer);
        writer.Write(NextEventIndex);
        writer.Write(ActorIndex);
        writer.Write(ActorActionIndex);
        Action insertParamsPtr = writer.ReservePtrIf(Parameters?.CanWrite() ?? false);
        writer.Write(0L);
        writer.Write(0L);

        writer.ReserveBlockWriter("EventArrayDataBlock", () => {
            if (Parameters?.CanWrite() ?? false) {
                insertParamsPtr();
                Parameters?.Write(writer);
            }
        });
    }

    public override void AlterActorIndex(int index)
    {
        if (index < ActorIndex) {
            ActorIndex--;
        }
        else if (index == ActorIndex) {
            ActorIndex = -1;
        }
    }

    public override void AlterEventIndex(int index)
    {
        if (index < NextEventIndex) {
            NextEventIndex--;
        }
        else if (index == NextEventIndex) {
            NextEventIndex = -1;
        }
    }

    internal override bool GetIndices(List<int> indices, int index, int joinIndex, List<int>? ignoreIndices = null)
    {
        if (!base.GetIndices(indices, index, joinIndex, ignoreIndices)) {
            return false;
        }

        if (NextEventIndex != joinIndex && NextEventIndex > -1) {
            Flowchart?.Events[NextEventIndex].GetIndices(indices, NextEventIndex, joinIndex, ignoreIndices);
        }

        return true;
    }
}

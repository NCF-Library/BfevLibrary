using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core;

public class JoinEvent : Event, IBfevDataBlock
{
    public short NextEventIndex { get; set; } = -1;

    [JsonIgnore]
    public Event? NextEvent => NextEventIndex > -1 ? Flowchart?.Events[NextEventIndex] : null;
    public string? NextEventName => NextEvent?.Name;

    [JsonConstructor]
    public JoinEvent(string name) : base(name, EventType.Join) { }
    public JoinEvent(BfevReader reader) : base(reader)
    {
        NextEventIndex = reader.ReadInt16();
        reader.BaseStream.Position += 2 + 2; // unused ushorts
        reader.BaseStream.Position += 8 + 8 + 8; // unused pointers
    }

    public new void Write(BfevWriter writer)
    {
        base.Write(writer);
        writer.Write(NextEventIndex);
        writer.Write((ushort)0);
        writer.Write((ushort)0);
        writer.Write(0L);
        writer.Write(0L);
        writer.Write(0L);
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

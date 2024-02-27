using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core;

public class SubflowEvent : Event, IBfevDataBlock
{
    public short NextEventIndex { get; set; } = -1;
    public Container? Parameters { get; set; }
    public string FlowchartName { get; set; } = string.Empty;
    public string EntryPointName { get; set; } = string.Empty;

    [JsonIgnore]
    public Event? NextEvent => NextEventIndex > -1 ? Flowchart?.Events[NextEventIndex] : null;
    public string? NextEventName => NextEvent?.Name;

    [JsonConstructor]
    public SubflowEvent(string name) : base(name, EventType.Subflow)
    {
        Parameters = new();
    }

    public SubflowEvent(BfevReader reader) : base(reader)
    {
        NextEventIndex = reader.ReadInt16();
        reader.BaseStream.Position += 2 + 2; // unused ushorts
        Parameters = reader.ReadObjectPtr<Container>(() => new(reader));
        FlowchartName = reader.ReadStringPtr();
        EntryPointName = reader.ReadStringPtr();
    }

    public new void Write(BfevWriter writer)
    {
        base.Write(writer);
        writer.Write(NextEventIndex);
        writer.Write((ushort)0);
        writer.Write((ushort)0);
        Action insertParamsPtr = writer.ReservePtrIf(Parameters?.CanWrite() ?? false);
        writer.WriteStringPtr(FlowchartName);
        writer.WriteStringPtr(EntryPointName);
        writer.ReserveBlockWriter("EventArrayDataBlock", () => {
            if (Parameters?.CanWrite() ?? false) {
                insertParamsPtr();
                Parameters?.Write(writer);
            }
        });
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

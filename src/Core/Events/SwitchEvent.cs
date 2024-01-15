using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core;

public class SwitchEvent : Event, IBfevDataBlock
{
    public class SwitchCase
    {
        public int Value { get; set; }
        public ushort EventIndex { get; set; }

        public SwitchCase(int value, ushort eventIndex)
        {
            Value = value;
            EventIndex = eventIndex;
        }
    }

    public short ActorIndex { get; set; }
    public short ActorQueryIndex { get; set; }
    public Container? Parameters { get; set; }
    public List<SwitchCase> SwitchCases { get; set; }

    [JsonIgnore]
    public Actor? Actor => ActorIndex > -1 ? _parent?.Actors[ActorIndex] : null;
    public string? ActorName => Actor?.Name;
    public string? ActorQuery => ActorQueryIndex < -1 ? Actor?.Queries[ActorQueryIndex] : null;

    [JsonConstructor]
    public SwitchEvent(string name) : base(name, EventType.Switch)
    {
        Parameters = new();
        SwitchCases = new();
    }

    public SwitchEvent(BfevReader reader) : base(reader)
    {
        ushort switchCaseCount = reader.ReadUInt16();
        ActorIndex = reader.ReadInt16();
        ActorQueryIndex = reader.ReadInt16();
        Parameters = reader.ReadObjectPtr<Container>(() => new(reader));
        SwitchCases = reader.ReadObjectsPtr(new SwitchCase[switchCaseCount], () => {
            SwitchCase switchCase = new(reader.ReadInt32(), reader.ReadUInt16());
            reader.ReadInt16();
            reader.Align(8);
            return switchCase;
        }).ToList();
        reader.BaseStream.Position += 8; // Unused pointer
    }

    public new void Write(BfevWriter writer)
    {
        base.Write(writer);
        writer.Write((ushort)SwitchCases.Count);
        writer.Write(ActorIndex);
        writer.Write(ActorQueryIndex);
        Action insertParamsPtr = writer.ReservePtrIf(Parameters?.CanWrite() ?? false);
        Action insertSwitchCasesPtr = writer.ReservePtrIf(SwitchCases.Count > 0, register: true);
        writer.Write(0L);
        writer.ReserveBlockWriter("EventArrayDataBlock", () => {
            if (SwitchCases.Count > 0) {
                writer.Align(8);
                insertSwitchCasesPtr();
                for (int i = 0; i < SwitchCases.Count; i++) {
                    SwitchCase switchCase = SwitchCases[i];
                    writer.Write(switchCase.Value);
                    writer.Write(switchCase.EventIndex);
                    writer.Write((ushort)0); // Padding
                    writer.Align(8);
                }
            }

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
        for (int i = 0; i < SwitchCases.Count; i++) {
            if (index < SwitchCases[i].EventIndex) {
                SwitchCases[i].EventIndex--;
            }
            else if (index == SwitchCases[i].EventIndex) {
                SwitchCases.RemoveAt(i);
                i--;
            }
        }
    }

    internal override bool GetIndices(List<int> indices, int index, int joinIndex, List<int>? ignoreIndices = null)
    {
        if (!base.GetIndices(indices, index, joinIndex, ignoreIndices)) {
            return false;
        }

        foreach (var switchCase in SwitchCases) {
            _parent!.Events[switchCase.EventIndex].GetIndices(indices, switchCase.EventIndex, joinIndex, ignoreIndices);
        }

        return true;
    }
}

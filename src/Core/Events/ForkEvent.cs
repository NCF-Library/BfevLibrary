using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core;

public class ForkEvent : Event, IBfevDataBlock
{
    public short JoinEventIndex { get; set; } = -1;
    public List<short> ForkEventIndicies { get; set; }

    [JsonConstructor]
    public ForkEvent(string name) : base(name, EventType.Fork)
    {
        ForkEventIndicies = new();
    }

    public ForkEvent(BfevReader reader) : base(reader)
    {
        ushort forkCount = reader.ReadUInt16();
        JoinEventIndex = reader.ReadInt16();
        reader.BaseStream.Position += 2; // unused ushort
        ForkEventIndicies = reader.ReadObjectsPtr(new short[forkCount], reader.ReadInt16).ToList();
        reader.BaseStream.Position += 8 + 8; // unused pointers
    }

    public new void Write(BfevWriter writer)
    {
        base.Write(writer);
        writer.Write((ushort)ForkEventIndicies.Count);
        writer.Write(JoinEventIndex);
        writer.Write((ushort)0);
        Action insertForkEventIndiciesPtr = writer.ReservePtr();
        writer.Write(0L);
        writer.Write(0L);
        writer.ReserveBlockWriter("EventArrayDataBlock", () => {
            if (ForkEventIndicies.Count > 0) {
                insertForkEventIndiciesPtr();
                for (int i = 0; i < ForkEventIndicies.Count; i++) {
                    writer.Write(ForkEventIndicies[i]);
                }
                writer.Align(8);
            }
        });
    }

    public override void AlterEventIndex(int index)
    {
        if (index < JoinEventIndex) {
            JoinEventIndex--;
        }
        else if (index == JoinEventIndex) {
            JoinEventIndex = -1;
        }

        for (int i = 0; i < ForkEventIndicies.Count; i++) {
            if (index < ForkEventIndicies[i]) {
                ForkEventIndicies[i]--;
            }
            else if (index == ForkEventIndicies[i]) {
                ForkEventIndicies.RemoveAt(i);
                i--;
            }
        }
    }

    internal override bool GetIndices(List<int> indices, int index, int joinIndex, List<int>? ignoreIndices = null)
    {
        if (!base.GetIndices(indices, index, joinIndex, ignoreIndices)) {
            return false;
        }

        foreach (var i in ForkEventIndicies) {
            if (i > -1) {
                _parent!.Events[i].GetIndices(indices, i, JoinEventIndex);
            }
        }

        if (JoinEventIndex > -1) {
            _parent!.Events[JoinEventIndex].GetIndices(indices, JoinEventIndex, joinIndex, ignoreIndices);
        }

        return true;
    }
}

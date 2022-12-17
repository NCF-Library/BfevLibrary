using EvflLibrary.Common;
using EvflLibrary.Parsers;
using System.Text.Json.Serialization;

namespace EvflLibrary.Core
{
    public class ForkEvent : Event, IEvflDataBlock
    {
        public short JoinEventIndex { get; set; }
        public List<short> ForkEventIndicies { get; set; }

        [JsonConstructor]
        public ForkEvent(string name, EventType type) : base(name, type)
        {
            ForkEventIndicies = new();
        }

        public ForkEvent(EvflReader reader) : base(reader)
        {
            ushort forkCount = reader.ReadUInt16();
            JoinEventIndex = reader.ReadInt16();
            reader.BaseStream.Position += 2; // unused ushort
            ForkEventIndicies = reader.ReadObjectsPtr(new short[forkCount], reader.ReadInt16).ToList();
            reader.BaseStream.Position += 8 + 8; // unused pointers
        }

        public new void Write(EvflWriter writer)
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
    }
}

using EvflLibrary.Common;
using EvflLibrary.Parsers;
using System.Text.Json.Serialization;

namespace EvflLibrary.Core
{
    public class ActionEvent : Event, IEvflDataBlock
    {
        public ushort NextEventIndex { get; set; }
        public ushort ActorIndex { get; set; }
        public ushort ActorActionIndex { get; set; }
        public Container? Parameters { get; set; }

        [JsonConstructor]
        public ActionEvent(string name, EventType type) : base(name, type)
        {
            Parameters = new();
        }

        {
            NextEventIndex = reader.ReadUInt16();
            ActorIndex = reader.ReadUInt16();
            ActorActionIndex = reader.ReadUInt16();
            Parameters = reader.ReadObjectPtr<Container>(() => new(reader));
            reader.BaseStream.Position += 8 + 8; // unused pointers
        }

        public new void Write(EvflWriter writer)
        {
            base.Write(writer);
            writer.Write(NextEventIndex);
            writer.Write(ActorIndex);
            writer.Write(ActorActionIndex);
            Action insertParamsPtr = writer.ReservePtrIf((Parameters?.Count ?? 0) > 0);
            writer.Write(0L);
            writer.Write(0L);

            writer.ReserveBlockWriter("EventArrayDataBlock", () => {
                insertParamsPtr();
                Parameters?.Write(writer);
            });
        }
    }
}

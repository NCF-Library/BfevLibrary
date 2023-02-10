using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core
{
    public class ActionEvent : Event, IBfevDataBlock
    {
        public short NextEventIndex { get; set; }
        public short ActorIndex { get; set; }
        public short ActorActionIndex { get; set; }
        public Container? Parameters { get; set; }

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
    }
}

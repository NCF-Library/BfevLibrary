using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core
{
    public class SubflowEvent : Event, IBfevDataBlock
    {
        public short NextEventIndex { get; set; }
        public Container? Parameters { get; set; }
        public string FlowchartName { get; set; }
        public string EntryPointName { get; set; }

        [JsonConstructor]
        public SubflowEvent(string name, EventType type) : base(name, type)
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
    }
}

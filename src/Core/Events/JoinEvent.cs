using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core
{
    public class JoinEvent : Event, IBfevDataBlock
    {
        public short NextEventIndex { get; set; }

        [JsonConstructor]
        public JoinEvent(string name, EventType type) : base(name, type) { }
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
    }
}

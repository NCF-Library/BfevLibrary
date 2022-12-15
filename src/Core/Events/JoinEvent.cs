using EvflLibrary.Common;
using EvflLibrary.Parsers;

namespace EvflLibrary.Core
{
    public class JoinEvent : Event, IEvflDataBlock
    {
        public ushort NextEventIndex { get; set; }

        public JoinEvent(EvflReader reader, Event baseEvent) : base(baseEvent)
        {
            NextEventIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 2 + 2; // unused ushorts
            reader.BaseStream.Position += 8 + 8 + 8; // unused pointers
        }

        public new void Write(EvflWriter writer)
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

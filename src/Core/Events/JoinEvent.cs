using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class JoinEvent : Event
    {
        public ushort NextEventIndex;

        public JoinEvent(BinaryReader reader, Event baseEvent) : base(baseEvent)
        {
            NextEventIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 2 + 2; // unused ushorts
            reader.BaseStream.Position += 8 + 8 + 8; // unused pointers
        }
    }
}

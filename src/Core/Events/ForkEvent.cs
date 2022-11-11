using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ForkEvent : Event
    {
        public ushort ForkCount;
        public ushort JoinEventIndex;
        public long ForkEventIndexesPtr;

        public ForkEvent(BinaryReader reader, Event baseEvent) : base(baseEvent)
        {
            ForkCount = reader.ReadUInt16();
            JoinEventIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 2; // unused ushort
            ForkEventIndexesPtr = reader.ReadInt64();
            reader.BaseStream.Position += 8 + 8; // unused pointers
        }
    }
}

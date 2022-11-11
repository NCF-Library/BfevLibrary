using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class SwitchEvent : Event
    {
        public ushort SwitchCaseCount;
        public ushort ActorIndex;
        public ushort ActorQueryIndex;
        public long ContainerPtr;
        public long SwitchCasesPtr;

        public SwitchEvent(BinaryReader reader, Event baseEvent) : base(baseEvent)
        {
            SwitchCaseCount = reader.ReadUInt16();
            ActorIndex = reader.ReadUInt16();
            ActorQueryIndex = reader.ReadUInt16();
            ContainerPtr = reader.ReadInt64();
            SwitchCasesPtr = reader.ReadInt64();
            reader.BaseStream.Position += 8; // unused pointer
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ActionEvent : Event
    {
        public ushort NextEventIndex; // Action: next event index, Switch: number of cases, Fork: number of forks
        public ushort ActorIndex; // Action & Switch: actor index, Fork: number join event index, Else: unused
        public ushort ActorActionIndex; // Action: actor action index, Switch: actor query index, Else: unused
        public long ContainerPtr;

        public ActionEvent(BinaryReader reader, Event baseEvent) : base(baseEvent)
        {
            NextEventIndex = reader.ReadUInt16();
            ActorIndex = reader.ReadUInt16();
            ActorActionIndex = reader.ReadUInt16();
            ContainerPtr = reader.ReadInt64();
            reader.BaseStream.Position += 8 + 8; // unused pointers
        }
    }
}

using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class SubflowEvent : Event
    {
        public ushort NextEventIndex;
        public long ContainerPtr;
        public long FlowchartNamePtr;
        public long EntryPointNamePtr;

        public string FlowchartName;
        public string EntryPointName;

        public SubflowEvent(BinaryReader reader, Event baseEvent) : base(baseEvent)
        {
            NextEventIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 2 + 2; // unused ushorts
            ContainerPtr = reader.ReadInt64();

            // only for debug
            FlowchartNamePtr = reader.ReadInt64();
            EntryPointNamePtr = reader.ReadInt64();
            reader.BaseStream.Position -= 8 + 8;

            FlowchartName = reader.ReadStringPtr();
            EntryPointName = reader.ReadStringPtr();
        }
    }
}

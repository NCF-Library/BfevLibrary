using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public enum TriggerType : byte
    {
        Enter = 1, Leave = 2
    }

    public class Trigger
    {
        public ushort ClipIndex;
        public TriggerType Type;

        public Trigger(BinaryReader reader)
        {
            ClipIndex = reader.ReadUInt16();
            Type = (TriggerType)reader.ReadByte();
            reader.BaseStream.Position += 1; // padding
        }
    }
}

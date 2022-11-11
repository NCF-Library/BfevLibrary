using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ResDicEntry
    {
        public uint BitIndex;
        public ushort NextWhere0;
        public ushort NextWhere1;
        public string Name;

        public ResDicEntry(BinaryReader reader)
        {
            BitIndex = reader.ReadUInt32();
            NextWhere0 = reader.ReadUInt16();
            NextWhere1 = reader.ReadUInt16();
            Name = reader.ReadStringPtr();
        }
    }
}

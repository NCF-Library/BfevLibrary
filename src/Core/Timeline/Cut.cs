using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class Cut
    {
        public float StartTime;
        public uint Unknown;
        public string Name;
        public Container Parameters;

        public Cut(BinaryReader reader)
        {
            StartTime = reader.ReadSingle();
            reader.BaseStream.Position += 4; // unknown
            Name = reader.ReadStringPtr();
            Parameters = reader.TemporarySeek<Container>(reader.ReadInt64(), SeekOrigin.Begin, () => new(reader));
        }
    }
}

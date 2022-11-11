using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class Oneshot
    {
        public float Time;
        public ushort ActorIndex;
        public ushort ActorActionIndex;
        public Container Parameters;

        public Oneshot(BinaryReader reader)
        {
            Time = reader.ReadSingle();
            ActorIndex = reader.ReadUInt16();
            ActorActionIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 8; // padding
            Parameters = reader.TemporarySeek<Container>(reader.ReadInt64(), SeekOrigin.Begin, () => new(reader));
        }
    }
}

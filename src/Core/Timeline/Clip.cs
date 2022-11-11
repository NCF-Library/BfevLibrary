using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class Clip
    {
        public float StartTime;
        public float Duration;
        public ushort ActorIndex;
        public ushort ActorActionIndex;
        public Container Parameters;

        public Clip(BinaryReader reader)
        {
            StartTime = reader.ReadSingle();
            Duration = reader.ReadSingle();
            ActorIndex = reader.ReadUInt16();
            ActorActionIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 1 + 3; // unknown + padding
            Parameters = reader.TemporarySeek<Container>(reader.ReadInt64(), SeekOrigin.Begin, () => new(reader));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public class ResClip
    {
        public float StartTime;
        public float Duration;
        public ushort ActorIndex;
        public ushort ActorActionIndex;
        public byte Unknown;
        public byte Padding1;
        public byte Padding2;
        public byte Padding3;
        public ResMeta Parameters;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ResClip
    {
        internal float StartTime;
        internal float Duration;
        internal ushort ActorIndex;
        internal ushort ActorActionIndex;
        internal byte Unknown;
        internal byte Padding1;
        internal byte Padding2;
        internal byte Padding3;
        internal ResMeta Parameters;
    }
}

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

    public class ResTrigger
    {
        internal ushort ClipIndex;
        internal TriggerType Type;
        internal byte Padding;
    }
}

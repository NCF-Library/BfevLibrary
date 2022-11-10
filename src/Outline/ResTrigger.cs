using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public enum TriggerType : byte
    {
        Enter = 1, Leave = 2
    }

    public class ResTrigger
    {
        public ushort ClipIndex;
        public TriggerType Type;
        public byte Padding;
    }
}

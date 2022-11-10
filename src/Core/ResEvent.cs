using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public enum EventType : byte
    {
        Action, Switch, Fork, Join, SubFlow
    }

    public class ResEvent
    {
        internal string Name;
        internal EventType Type;
        internal byte Padding1;
        internal ushort NextEventIndex; // Switch: Number of cases, Fork: Number of forks
        internal ushort ActorIndex; // Fork: Number Join event index, Else: unused
        internal ushort ActorActionIndex; // Switch: Actor query index, Else: unused
        internal dynamic? Void1; // see wiki for impl
        internal dynamic? Void2; // see wiki for impl
        internal dynamic? Void3; // see wiki for impl
    }
}

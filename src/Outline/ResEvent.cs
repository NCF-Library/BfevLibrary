using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public enum EventType : byte
    {
        Action, Switch, Fork, Join, SubFlow
    }

    public class ResEvent
    {
        public string Name;
        public EventType Type;
        public byte Padding1;
        public ushort NextEventIndex; // Switch: Number of cases, Fork: Number of forks
        public ushort ActorIndex; // Fork: Number Join event index, Else: unused
        public ushort ActorActionIndex; // Switch: Actor query index, Else: unused
        public dynamic? Void1; // see wiki for impl
        public dynamic? Void2; // see wiki for impl
        public dynamic? Void3; // see wiki for impl
    }
}

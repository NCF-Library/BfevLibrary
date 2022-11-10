using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ResTimeline
    {
        internal string Magic = "TLIN";
        internal uint StringPoolOffset;
        internal uint Padding1;
        internal uint Padding2;
        internal float Duration;
        internal ushort ActorCount;
        internal ushort ActionCount;
        internal ushort ClipCount;
        internal ushort OneshotCount;
        internal ushort SubTimelineCount;
        internal ushort CutCount;
        internal string Name;
        internal ResActor Actors;
        internal ResClip Clips;
        internal ResOneshot Oneshots;
        internal ResTrigger Triggers;
        internal ResSubTimeline Subtimelines;
        internal ResCut Cuts;
        internal ResMeta Parameters;
    }
}

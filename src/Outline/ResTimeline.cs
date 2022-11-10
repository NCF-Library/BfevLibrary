using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public class ResTimeline
    {
        public string Magic = "TLIN";
        public uint StringPoolOffset;
        public uint Padding1;
        public uint Padding2;
        public float Duration;
        public ushort ActorCount;
        public ushort ActionCount;
        public ushort ClipCount;
        public ushort OneshotCount;
        public ushort SubTimelineCount;
        public ushort CutCount;
        public string Name;
        public ResActor Actors;
        public ResClip Clips;
        public ResOneshot Oneshots;
        public ResTrigger Triggers;
        public ResSubTimeline Subtimelines;
        public ResCut Cuts;
        public ResMeta Parameters;
    }
}

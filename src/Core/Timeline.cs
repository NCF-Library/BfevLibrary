using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvflLibrary.Extensions;

namespace EvflLibrary.Core
{
    public class Timeline
    {
        public string Magic = "TLIN";
        public uint StringPoolOffset;
        public float Duration;
        public ushort ActorCount;
        public ushort ActionCount;
        public ushort ClipCount;
        public ushort OneshotCount;
        public ushort SubTimelineCount;
        public ushort CutCount;
        public string Name;

        public Actor[] Actors;
        public Clip[] Clips;
        public Oneshot[] Oneshots;
        public Trigger[] Triggers;
        public SubTimeline[] Subtimelines;
        public Cut[] Cuts;
        public Container Parameters;

        public Timeline(BinaryReader reader)
        {
            Magic = new(reader.ReadChars(4));
            StringPoolOffset = reader.ReadUInt32();
            reader.BaseStream.Position += 4 + 4;
            Duration = reader.ReadSingle();
            ActorCount = reader.ReadUInt16();
            ActionCount = reader.ReadUInt16();
            ClipCount = reader.ReadUInt16();
            OneshotCount = reader.ReadUInt16();
            SubTimelineCount = reader.ReadUInt16();
            CutCount = reader.ReadUInt16();
            Name = reader.ReadStringPtr();

            Actors = new Actor[ActorCount];
            reader.TemporarySeek(reader.ReadInt64(), SeekOrigin.Begin, () => {
                for (int i = 0; i < ActorCount; i++) {
                    Actors[i] = new(reader);
                }
            });

            Clips = new Clip[ClipCount];
            reader.TemporarySeek(reader.ReadInt64(), SeekOrigin.Begin, () => {
                for (int i = 0; i < ClipCount; i++) {
                    Clips[i] = new(reader);
                }
            });

            Oneshots = new Oneshot[OneshotCount];
            reader.TemporarySeek(reader.ReadInt64(), SeekOrigin.Begin, () => {
                for (int i = 0; i < OneshotCount; i++) {
                    Oneshots[i] = new(reader);
                }
            });

            Triggers = new Trigger[ClipCount * 2];
            reader.TemporarySeek(reader.ReadInt64(), SeekOrigin.Begin, () => {
                for (int i = 0; i < (ClipCount * 2); i++) {
                    Triggers[i] = new(reader);
                }
            });

            Subtimelines = new SubTimeline[SubTimelineCount];
            reader.TemporarySeek(reader.ReadInt64(), SeekOrigin.Begin, () => {
                for (int i = 0; i < SubTimelineCount; i++) {
                    Subtimelines[i] = new(reader);
                }
            });

            Cuts = new Cut[CutCount];
            reader.TemporarySeek(reader.ReadInt64(), SeekOrigin.Begin, () => {
                for (int i = 0; i < CutCount; i++) {
                    Cuts[i] = new(reader);
                }
            });

            Parameters = reader.TemporarySeek<Container>(reader.ReadInt64(), SeekOrigin.Begin, () => new(reader));
        }
    }
}

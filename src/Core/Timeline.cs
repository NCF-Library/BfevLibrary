using BfevLibrary.Common;
using BfevLibrary.Parsers;

namespace BfevLibrary.Core
{
    public class Timeline : IBfevDataBlock
    {
        public const string Magic = "TLIN";

        public string Name { get; set; }
        public float Duration { get; set; }
        public List<Actor> Actors { get; set; }
        public List<Clip> Clips { get; set; }
        public List<Oneshot> Oneshots { get; set; }
        public List<Trigger> Triggers { get; set; }
        public List<SubTimeline> SubTimelines { get; set; }
        public List<Cut> Cuts { get; set; }
        public Container Parameters { get; set; }

        public Timeline()
        {
            Actors = new();
            Clips = new();
            Oneshots = new();
            Triggers = new();
            SubTimelines = new();
            Cuts = new();
            Parameters = new();
        }
        public Timeline(BfevReader reader)
        {
            Read(reader);
        }

        public IBfevDataBlock Read(BfevReader reader)
        {
            reader.CheckMagic(Magic);
            reader.BaseStream.Position += 4 + 4 + 4; // String Pool Offset (uint), Padding (uint), Padding (uint)
            Duration = reader.ReadSingle();

            ushort actorCount = reader.ReadUInt16();
            ushort actionCount = reader.ReadUInt16();
            ushort clipCount = reader.ReadUInt16();
            ushort oneshotCount = reader.ReadUInt16();
            ushort subTimelineCount = reader.ReadUInt16();
            ushort cutCount = reader.ReadUInt16();

            Name = reader.ReadStringPtr();

            Actors = reader.ReadObjectsPtr(new Actor[actorCount], () => new(reader)).ToList();
            Clips = reader.ReadObjectsPtr(new Clip[clipCount], () => new(reader)).ToList();
            Oneshots = reader.ReadObjectsPtr(new Oneshot[oneshotCount], () => new(reader)).ToList();
            Triggers = reader.ReadObjectsPtr(new Trigger[clipCount * 2], () => new(reader)).ToList();
            reader.Align(8);
            SubTimelines = reader.ReadObjectsPtr(new SubTimeline[subTimelineCount], () => new(reader)).ToList();
            Cuts = reader.ReadObjectsPtr(new Cut[cutCount], () => new(reader)).ToList();
            Parameters = reader.TemporarySeek<Container>(reader.ReadInt64(), SeekOrigin.Begin, () => new(reader));

            return this;
        }

        public void Write(BfevWriter writer)
        {
            // Nintendo is weird sometimes
            for (int i = 0; i < Actors.Count; i++) {
                Actors[i].WriteData(writer);
                writer.Align(8);
            }

            long? paramOffset = null;
            if (Parameters.Count > 0) {
                paramOffset = writer.BaseStream.Position;
                Parameters.Write(writer);
            }

            writer.Align(8);
            writer.WriteReserved("insertTimelinesOffsets");
            writer.WriteReserved("insertFirstBlockOffset", remove: true);
            writer.Write(Magic.AsSpan());
            writer.ReserveRelativeOffset("insertTimelineStringPoolOffset", writer.BaseStream.Position - 4);
            writer.Seek(4 + 4, SeekOrigin.Current);
            writer.Write(Duration);
            writer.Write((ushort)Actors.Count);
            writer.Write((ushort)Actors.Sum(x => x.Actions.Count));
            writer.Write((ushort)Clips.Count);
            writer.Write((ushort)Oneshots.Count);
            writer.Write((ushort)SubTimelines.Count);
            writer.Write((ushort)Cuts.Count);
            writer.WriteStringPtr(Name);

            Action insertActorsPtr = writer.ReservePtrIf(Actors.Count > 0, register: true);
            Action insertClipsPtr = writer.ReservePtrIf(Clips.Count > 0, register: true);
            Action insertOneshotsPtr = writer.ReservePtrIf(Oneshots.Count > 0, register: true);
            Action insertTriggersPtr = writer.ReservePtrIf(Triggers.Count > 0, register: true);
            Action insertSubTimelinesPtr = writer.ReservePtrIf(SubTimelines.Count > 0, register: true);
            Action insertCutsPtr = writer.ReservePtrIf(Cuts.Count > 0, register: true);

            if (paramOffset != null) {
                writer.RegisterPtr();
                writer.Write((long)paramOffset);
            }

            // Nintendo is weird sometimes
            insertActorsPtr();
            for (int i = 0; i < Actors.Count; i++) {
                Actors[i].WriteHeader(writer);
            }
            writer.Align(8);

            List<Tuple<Action, IEnumerable<IBfevDataBlock>>> arrayBlocks = new() {
                new(insertClipsPtr, Clips),
                new(insertOneshotsPtr, Oneshots),
                new(insertSubTimelinesPtr, SubTimelines),
                new(insertTriggersPtr, Triggers),
                new(insertCutsPtr, Cuts)
            };

            foreach ((var insertAction, var items) in arrayBlocks) {
                insertAction();
                writer.WriteObjects(items);
                writer.Align(8);
            }

            writer.WriteReserved("ClipArrayDataBlock", alignment: 8);
            writer.WriteReserved("OneshotArrayDataBlock", alignment: 8);
            writer.WriteReserved("CutArrayDataBlock", alignment: 8);
            writer.Align(8);
        }
    }
}

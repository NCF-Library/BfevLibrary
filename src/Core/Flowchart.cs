using BfevLibrary.Common;
using BfevLibrary.Parsers;

namespace BfevLibrary.Core
{
    public class Flowchart : IBfevDataBlock
    {
        public const string Magic = "EVFL";

        public string Name { get; set; }
        public List<Actor> Actors { get; set; }
        public List<Event> Events { get; set; }
        public RadixTree<EntryPoint> EntryPoints { get; set; }

        public Flowchart()
        {
            Actors = new();
            Events = new();
            EntryPoints = new();
        }

        public Flowchart(string name) : this()
        {
            Name = name;
        }

        public Flowchart(BfevReader reader)
        {
            Read(reader);
        }

        public IBfevDataBlock Read(BfevReader reader)
        {
            // Check the file magic
            reader.CheckMagic(Magic);

            // String pool offset (uint), Padding (long)
            reader.BaseStream.Position += 4 + 8;

            // Actor count (ushort)
            ushort actorCount = reader.ReadUInt16();

            // Total action count (ushort), total query count (ushort)
            reader.BaseStream.Position += 2 + 2;

            // Event count (ushort), entry point count (ushort)
            ushort eventCount = reader.ReadUInt16();
            ushort entryPointCount = reader.ReadUInt16();

            // Padding (byte[6])
            reader.BaseStream.Position += 6;

            // Name ptr (ulong)
            Name = reader.ReadStringPtr();

            // Actors ptr (ulong)
            Actors = new(reader.ReadObjectsPtr(new Actor[actorCount], () => new(reader)));

            // Events ptr (ulong)
            Events = new(reader.ReadObjectsPtr(new Event[eventCount], () => Event.LoadTypeInstance(reader)));

            // Entry points dictionary ptr (ulong), entry point ptr (ulong)
            EntryPoints = reader.ReadObjectPtr<RadixTree<EntryPoint>>(() => new(reader))!;
            EntryPoints.LinkToArray(reader.ReadObjectsPtr(new EntryPoint[entryPointCount], () => new(reader)));

            return this;
        }

        public void Write(BfevWriter writer)
        {
            writer.WriteReserved("insertFlowchartsOffsets");
            writer.WriteReserved("insertFirstBlockOffset", remove: true);
            writer.Write(Magic.AsSpan());
            writer.ReserveRelativeOffset("insertFlowchartStringPoolOffset", writer.BaseStream.Position - 4);
            writer.Write(0L); // Padding
            writer.Write((ushort)Actors.Count);
            writer.Write((ushort)Actors.Sum(x => x.Actions.Count));
            writer.Write((ushort)Actors.Sum(x => x.Queries.Count));
            writer.Write((ushort)Events.Count);
            writer.Write((ushort)EntryPoints.Count);
            writer.Write(new byte[6]); // Padding
            writer.WriteStringPtr(Name);
            Action insertActorsPtr = writer.ReservePtrIf(Actors.Count > 0, register: true);
            Action insertEventsPtr = writer.ReservePtrIf(Events.Count > 0, register: true);
            Action insertEntryPointsDictPtr = writer.ReservePtr();
            Action insertEntryPointsPtr = writer.ReservePtrIf(EntryPoints.Count > 0, register: true);

            insertActorsPtr();
            writer.WriteObjects(Actors);

            insertEventsPtr();
            writer.WriteObjects(Events);

            insertEntryPointsDictPtr();
            writer.WriteRadixTree(EntryPoints.Keys.ToArray());
            writer.Align(8);

            insertEntryPointsPtr();
            writer.WriteObjects(EntryPoints.Values);

            writer.WriteReserved("EventArrayDataBlock", alignment: 8);
            writer.WriteReserved("ActorArrayDataBlock", alignment: 8);
            writer.WriteReserved("EntryPointArrayDataBlock", alignment: 8);
            writer.Align(8);
        }
    }
}

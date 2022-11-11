using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvflLibrary.Extensions;

namespace EvflLibrary.Core
{
    public class Flowchart
    {
        public string Magic = "EVFL";
        public uint StringPoolOffset;
        public ushort ActorCount;
        public ushort ActionCount;
        public ushort QueryCount;
        public ushort EventCount;
        public ushort EntryPointCount;
        public string Name;

        public long ActorsOffset;
        public long EventsOffset;
        public long EntryPointDictionaryOffset;
        public long EntryPointsOffset;

        public Actor[] Actors;
        public Event[] Events;
        public ResDic EntryPointDictionary;
        public EntryPoint[] EntryPoints;

        public Flowchart(BinaryReader reader)
        {
            Magic = new(reader.ReadChars(4));
            StringPoolOffset = reader.ReadUInt32();
            reader.BaseStream.Position += 8; // Padding
            ActorCount = reader.ReadUInt16();
            ActionCount = reader.ReadUInt16();
            QueryCount = reader.ReadUInt16();
            EventCount = reader.ReadUInt16();
            EntryPointCount = reader.ReadUInt16();
            reader.BaseStream.Position += 6; // Padding
            Name = reader.ReadStringPtr();

            ActorsOffset = reader.ReadInt64();
            EventsOffset = reader.ReadInt64();
            EntryPointDictionaryOffset = reader.ReadInt64();
            EntryPointsOffset = reader.ReadInt64();

            Actors = new Actor[ActorCount];
            for (int i = 0; i < ActorCount; i++) {
                Actors[i] = new(reader);
            }

            Events = new Event[EventCount];
            for (int i = 0; i < EventCount; i++) {
                Event baseEvent = new(reader);
                Events[i] = baseEvent.Type switch {
                    EventType.Action => new ActionEvent(reader, baseEvent),
                    EventType.Switch => new SwitchEvent(reader, baseEvent),
                    EventType.Fork => new ForkEvent(reader, baseEvent),
                    EventType.Join => new JoinEvent(reader, baseEvent),
                    EventType.Subflow => new SubflowEvent(reader, baseEvent),
                    _ => throw new NotImplementedException()
                };
            }

            EntryPointDictionary = new(reader);

            EntryPoints = new EntryPoint[EntryPointCount];
            for (int i = 0; i < EntryPointCount; i++) {
                EntryPoints[i] = new(reader);
            }
        }
    }
}

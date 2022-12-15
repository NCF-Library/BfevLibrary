using EvflLibrary.Common;
using EvflLibrary.Parsers;
using System.Text.Json.Serialization;

namespace EvflLibrary.Core
{
    public enum EventType
    {
        Action, Switch, Fork, Join, Subflow
    }

    [JsonDerivedType(typeof(ActionEvent))]
    [JsonDerivedType(typeof(ForkEvent))]
    [JsonDerivedType(typeof(JoinEvent))]
    [JsonDerivedType(typeof(SubflowEvent))]
    [JsonDerivedType(typeof(SwitchEvent))]
    public class Event : IEvflDataBlock
    {
        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventType Type { get; set; }

        /// <summary></summary>
        /// <returns>
        /// An <see cref="Event"/> sub-class instance defined by the read <see cref="EventType"/>.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Event LoadTypeInstance(EvflReader reader)
        {
            Event baseEvent = new(reader);
            return baseEvent.Type switch {
                EventType.Action => new ActionEvent(reader, baseEvent),
                EventType.Switch => new SwitchEvent(reader, baseEvent),
                EventType.Fork => new ForkEvent(reader, baseEvent),
                EventType.Join => new JoinEvent(reader, baseEvent),
                EventType.Subflow => new SubflowEvent(reader, baseEvent),
                _ => throw new NotImplementedException()
            };
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            Name = reader.ReadStringPtr();
            Type = (EventType)reader.ReadByte();
            reader.BaseStream.Position += 1;
            return this;
        }

        public void Write(EvflWriter writer)
        {
            writer.WriteStringPtr(Name);
            writer.Write((byte)Type);
            writer.Seek(1, SeekOrigin.Current); // Padding (byte)
        }

        public Event(string name, EventType type)
        {
            Name = name;
            Type = type;
        }

        internal Event(Event baseEvent)
        {
            Name = baseEvent.Name;
            Type = baseEvent.Type;
        }

        internal Event(EvflReader reader)
        {
            Read(reader);
        }
    }
}

using EvflLibrary.Common;
using EvflLibrary.Parsers;
using System.Text.Json.Serialization;

namespace EvflLibrary.Core
{
    public enum EventType
    {
        Action, Switch, Fork, Join, Subflow
    }

    [JsonConverter(typeof(EventConverter))]
    public abstract class Event : IEvflDataBlock
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
            // Read event type ahead
            EventType type = reader.TemporarySeek(8, SeekOrigin.Current, () => (EventType)reader.ReadByte());

            return type switch {
                EventType.Action => new ActionEvent(reader),
                EventType.Switch => new SwitchEvent(reader),
                EventType.Fork => new ForkEvent(reader),
                EventType.Join => new JoinEvent(reader),
                EventType.Subflow => new SubflowEvent(reader),
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

        internal Event(EvflReader reader)
        {
            Read(reader);
        }
    }
}

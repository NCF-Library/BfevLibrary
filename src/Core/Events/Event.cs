using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core
{
    public enum EventType
    {
        Action, Switch, Fork, Join, Subflow
    }

    [JsonConverter(typeof(EventConverter))]
    public abstract class Event : IBfevDataBlock
    {
        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventType Type { get; set; }

        /// <summary></summary>
        /// <returns>
        /// An <see cref="Event"/> sub-class instance defined by the read <see cref="EventType"/>.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Event LoadTypeInstance(BfevReader reader)
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

        public IBfevDataBlock Read(BfevReader reader)
        {
            Name = reader.ReadStringPtr();
            Type = (EventType)reader.ReadByte();
            reader.BaseStream.Position += 1;
            return this;
        }

        public void Write(BfevWriter writer)
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

        internal Event(BfevReader reader)
        {
            Read(reader);
        }
    }
}

using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public enum EventType : byte
    {
        Action, Switch, Fork, Join, Subflow
    }

    public class Event
    {
        public string Name;
        public EventType Type;

        public Event(Event baseEvent)
        {
            Name = baseEvent.Name;
            Type = baseEvent.Type;
        }

        public Event(BinaryReader reader)
        {
            Name = reader.ReadStringPtr();
            Type = (EventType)reader.ReadByte();
            reader.BaseStream.Position += 1;
        }
    }
}

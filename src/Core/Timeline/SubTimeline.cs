using EvflLibrary.Common;
using EvflLibrary.Parsers;
using System.Text.Json.Serialization;

namespace EvflLibrary.Core
{
    public class SubTimeline : IEvflDataBlock
    {
        public string Name { get; set; }

        [JsonConstructor]
        public SubTimeline(string name)
        {
            Name = name;
        }

        public SubTimeline(EvflReader reader)
        {
            Read(reader);
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            Name = reader.ReadStringPtr();
            return this;
        }

        public void Write(EvflWriter writer)
        {
            writer.WriteStringPtr(Name);
        }
    }
}

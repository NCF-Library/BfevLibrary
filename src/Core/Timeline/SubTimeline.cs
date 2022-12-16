using EvflLibrary.Common;
using EvflLibrary.Parsers;

namespace EvflLibrary.Core
{
    public class SubTimeline : IEvflDataBlock
    {
        public string Name;

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

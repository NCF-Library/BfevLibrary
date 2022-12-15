using EvflLibrary.Parsers;

namespace EvflLibrary.Common
{
    public interface IEvflDataBlock
    {
        public IEvflDataBlock Read(EvflReader reader);
        public void Write(EvflWriter writer);
    }
}

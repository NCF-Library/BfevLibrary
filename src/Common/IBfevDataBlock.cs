using BfevLibrary.Parsers;

namespace BfevLibrary.Common
{
    public interface IBfevDataBlock
    {
        public IBfevDataBlock Read(BfevReader reader);
        public void Write(BfevWriter writer);
    }
}

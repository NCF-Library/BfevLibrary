using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class RelocationTableSection
    {
        public long BasePointer;
        public int BasePointerOffset;
        public int Size;
        public int FirstEntryIndex;
        public int EntryCount;

        public RelocationTableSection(BinaryReader reader)
        {
            BasePointer = reader.ReadInt64();
            BasePointerOffset = reader.ReadInt32();
            Size = reader.ReadInt32();
            FirstEntryIndex = reader.ReadInt32();
            EntryCount = reader.ReadInt32();
        }
    }
}

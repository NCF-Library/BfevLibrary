using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public struct RelocationTableEntry
    {
        public uint PointerOffsets; // Offset to pointers to relocate, relative to the table base pointer
        public uint RelocatedPointers; // Bit field that determines which pointers need to be relocated (up to 32 contiguous pointers starting from the listed offset)
    }
}

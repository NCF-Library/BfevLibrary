using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public struct RelocationTableSection
    {
        public dynamic BasePointer; // Optional
        public int BasePointerOffset; // Optional
        public int Size;
        public int FirstEntryIndex;
        public int EntryCount;
    }
}

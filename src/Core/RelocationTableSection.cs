using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public struct RelocationTableSection
    {
        internal dynamic BasePointer; // Optional
        internal int BasePointerOffset; // Optional
        internal int Size;
        internal int FirstEntryIndex;
        internal int EntryCount;
    }
}

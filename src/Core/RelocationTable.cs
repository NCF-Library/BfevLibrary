using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class RelocationTable
    {
        internal string Magic = "RELT";
        internal int Offset;
        internal int SectionCount;
        internal uint Padding;
        internal dynamic? Sections;
        internal dynamic? Entry; // emitted in the same order as each section
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public class RelocationTable
    {
        public string Magic = "RELT";
        public int Offset;
        public int SectionCount;
        public uint Padding;
        public dynamic? Sections;
        public dynamic? Entry; // emitted in the same order as each section
    }
}

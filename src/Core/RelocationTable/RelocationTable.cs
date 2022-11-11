using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class RelocationTable
    {
        public string Magic = "RELT";
        public int Offset;
        public int Count;
        public RelocationTableSection[] Sections;
        public RelocationTableEntry[] Entries;

        public RelocationTable(BinaryReader reader)
        {
            Magic = new(reader.ReadChars(4));
            Offset = reader.ReadInt32();
            Count = reader.ReadInt32();
            reader.BaseStream.Position += 4;

            Sections = new RelocationTableSection[Count];
            for (int i = 0; i < Count; i++) {
                Sections[i] = new(reader);
            }

            Entries = new RelocationTableEntry[Count];
            for (int i = 0; i < Count; i++) {
                Entries[i] = new(reader);
            }
        }
    }
}

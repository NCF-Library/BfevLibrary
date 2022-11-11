using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ResDic
    {
        public string Magic = "DIC ";
        public uint EntryCount;
        public ResDicEntry RootEntry;
        public ResDicEntry[] Entries;

        public ResDic(BinaryReader reader)
        {
            Magic = new(reader.ReadChars(4));
            EntryCount = reader.ReadUInt32();
            RootEntry = new ResDicEntry(reader);
            Entries = new ResDicEntry[EntryCount];

            for (int i = 0; i < EntryCount; i++) {
                Entries[i] = new ResDicEntry(reader);
            }
        }
    }
}

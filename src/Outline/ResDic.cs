using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public class ResDic
    {
        public string Magic = "DIC ";
        public uint EntryCount; // ignoring root entry
        public uint RootEntry;
        public dynamic? Entries;
    }
}

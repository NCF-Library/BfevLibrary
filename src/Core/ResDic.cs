using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ResDic
    {
        internal string Magic = "DIC ";
        internal uint EntryCount; // ignoring root entry
        internal uint RootEntry;
        internal dynamic? Entries;
    }
}

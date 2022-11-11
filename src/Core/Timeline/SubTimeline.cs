using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class SubTimeline
    {
        public string Name;

        public SubTimeline(BinaryReader reader)
        {
            Name = reader.ReadStringPtr();
        }
    }
}

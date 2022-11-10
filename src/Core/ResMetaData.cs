using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ResMetaData
    {
        internal string Argument;
        internal ResMetaItem[] Container;
        internal int Int;
        internal bool Bool;
        internal float Float;
        internal string String;
        internal string WideString;
        internal int[] IntArray;
        internal bool[] BoolArray; // 0x80000001 if true, 0x00000000 otherwise
        internal float[] FloatArray;
        internal string[] StringArray;
        internal string[] WstringArray;
        internal Tuple<string, string> ActorIdentifier; // Two strings: actor name + secondary name
    }
}

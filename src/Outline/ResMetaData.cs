using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public class ResMetaData
    {
        public string Argument;
        public ResMetaItem[] Container;
        public int Int;
        public bool Bool;
        public float Float;
        public string String;
        public string WideString;
        public int[] IntArray;
        public bool[] BoolArray; // 0x80000001 if true, 0x00000000 otherwise
        public float[] FloatArray;
        public string[] StringArray;
        public string[] WstringArray;
        public Tuple<string, string> ActorIdentifier; // Two strings: actor name + secondary name
    }
}

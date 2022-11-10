using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public enum ContainerDataType : byte
    {
        Argument,
        Container,
        Int,
        Bool,
        Float,
        String,
        WString,
        IntArray,
        BoolArray,
        FloatArray,
        StringArray,
        WStringArray,
        ActorIdentifier,
    }

    public class ResMeta
    {
        public ResMetaItem ContainerItem;
    }
}

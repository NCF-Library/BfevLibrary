using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ResMetaItem
    {
        internal ContainerDataType DataType;
        internal byte Padding1;
        internal ushort ItemCount;
        internal uint Padding2;
        internal dynamic? Dictionary; // only for the Container data type
        internal ResMetaData Data;
    }
}

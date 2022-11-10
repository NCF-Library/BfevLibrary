using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public class ResMetaItem
    {
        public ContainerDataType DataType;
        public byte Padding1;
        public ushort ItemCount;
        public uint Padding2;
        public dynamic? Dictionary; // only for the Container data type
        public ResMetaData Data;
    }
}

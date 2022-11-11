using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ContainerItem
    {
        public ContainerDataType DataType;
        public ushort ItemCount;
        public long DictionaryOffset;
        public ContainerData Data;

        public ResDic? Dictionary;

        public ContainerItem(BinaryReader reader)
        {
            DataType = (ContainerDataType)reader.ReadByte();
            reader.BaseStream.Position += 1;
            ItemCount = reader.ReadUInt16();
            reader.BaseStream.Position += 4;
            DictionaryOffset = reader.ReadInt64();

            if (DataType == ContainerDataType.Container) {
                Dictionary = reader.TemporarySeek<ResDic>(DictionaryOffset, SeekOrigin.Begin, () => new(reader));
            }

            Data = new(reader, ItemCount, DataType);
        }
    }
}

using EvflLibrary.Common;
using EvflLibrary.Parsers;
using System.Text.Json.Serialization;

namespace EvflLibrary.Core
{
    public class ContainerItem : ContainerData, IEvflDataBlock
    {
        internal readonly bool IsRoot = false;
        internal ushort Count;

        [JsonIgnore]
        public ContainerDataType DataType { get; set; }

        public ContainerItem(EvflReader reader, bool isRoot = false)
        {
            IsRoot = isRoot;
            Read(reader);
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            DataType = (ContainerDataType)reader.ReadByte();
            reader.BaseStream.Position += 1;

            Count = reader.ReadUInt16();
            reader.BaseStream.Position += 4;

            long dictionaryOffset = reader.ReadInt64();
            if (DataType == ContainerDataType.Container) {
                Items = reader.ReadObjectPtr<RadixTree<ContainerItem>>(() => new(reader), dictionaryOffset);
            }

            if (!IsRoot) {
                // Let the parent (root) handle loading
                ReadData(reader, Count, DataType);
            }

            return this;
        }

        public void Write(EvflWriter writer)
        {
            writer.Write((byte)DataType);
            writer.Write(new byte());

            ContainerDataType type = GetDataType();
            writer.Write((ushort)GetCount(type));
            writer.Write(0U);
            if (type == ContainerDataType.Container) {
                writer.RegisterPtr();
                writer.Write(writer.BaseStream.Position + 8);
            }
            else {
                writer.Write(0L);
            }
            WriteData(writer);
        }
    }
}

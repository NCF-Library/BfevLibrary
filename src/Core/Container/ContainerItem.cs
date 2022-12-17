using BfevLibrary.Common;
using BfevLibrary.Parsers;

namespace BfevLibrary.Core
{
    public class ContainerItem : ContainerData, IBfevDataBlock
    {
        internal readonly bool IsRoot = false;
        internal ushort Count;

        public ContainerItem() { }
        public ContainerItem(BfevReader reader, bool isRoot = false)
        {
            IsRoot = isRoot;
            Read(reader);
        }

        public IBfevDataBlock Read(BfevReader reader)
        {
            ContainerDataType dataType = (ContainerDataType)reader.ReadByte();
            reader.BaseStream.Position += 1;

            Count = reader.ReadUInt16();
            reader.BaseStream.Position += 4;

            long dictionaryOffset = reader.ReadInt64();
            if (dataType == ContainerDataType.Container) {
                Items = reader.ReadObjectPtr<RadixTree<ContainerItem>>(() => new(reader), dictionaryOffset);
            }

            if (!IsRoot) {
                // Let the parent (root) handle loading
                ReadData(reader, Count, dataType);
            }

            return this;
        }

        public void Write(BfevWriter writer)
        {
            ContainerDataType type = GetDataType();

            writer.Write((byte)type);
            writer.Write(new byte());

            writer.Write((ushort)GetCount(type));
            writer.Write(0U);
            writer.Write(0L);
            WriteData(writer);
        }
    }
}

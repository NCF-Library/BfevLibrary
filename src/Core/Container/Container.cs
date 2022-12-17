using EvflLibrary.Common;
using EvflLibrary.Parsers;

namespace EvflLibrary.Core
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

    public class Container : RadixTree<ContainerItem>, IEvflDataBlock
    {
        public Container() { }
        public Container(EvflReader reader)
        {
            Read(reader);
        }

        public new IEvflDataBlock Read(EvflReader reader)
        {
            ContainerItem root = new(reader, isRoot: true);
            StaticKeys = root.Items!.StaticKeys;
            LinkToArray(reader.ReadObjectOffsetsPtr(new ContainerItem[root.Count], () => new(reader), reader.BaseStream.Position));

            return this;
        }

        public new void Write(EvflWriter writer)
        {
            writer.Write((byte)ContainerDataType.Container);
            writer.Write(new byte());
            writer.Write((ushort)Count);
            writer.Write(0U);
            Action insertDicPtr = writer.ReservePtr();
            List<Action> insertItemsPtrs = new();

            for (int i = 0; i < Values.Count; i++) {
                insertItemsPtrs.Add(writer.ReservePtr());
            }

            insertDicPtr();
            writer.WriteRadixTree(Keys.ToArray());

            int idx = 0;
            foreach (var item in Values) {
                writer.Align(8);
                insertItemsPtrs[idx].Invoke();
                item.Write(writer);
                idx++;
            }
        }
    }
}

using BfevLibrary.Common;
using BfevLibrary.Parsers;

namespace BfevLibrary.Core
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

    public class Container : RadixTree<ContainerItem>, IBfevDataBlock
    {
        public Container() { }
        public Container(BfevReader reader)
        {
            Read(reader);
        }

        public bool? CanWrite()
        {
            if (Values.Count <= 0) {
                return null;
            }

            return true;
        }

        public new IBfevDataBlock Read(BfevReader reader)
        {
            ContainerItem root = new(reader, isRoot: true);
            StaticKeys = root.Items!.StaticKeys;
            LinkToArray(reader.ReadObjectOffsetsPtr(new ContainerItem[root.Count], () => new(reader), reader.BaseStream.Position));

            return this;
        }

        public new void Write(BfevWriter writer)
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

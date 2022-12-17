using BfevLibrary.Common;
using BfevLibrary.Parsers;

namespace BfevLibrary.Core
{
    public class RadixTree<T> : Dictionary<string, T>, IBfevDataBlock
    {
        internal string[] StaticKeys = Array.Empty<string>();

        // internal Dictionary<string, T> Values = new();
        // public int Count => Values.Count;

        public T this[int index] {
            get => this[StaticKeys[index]];
        }

        public RadixTree() { }
        public RadixTree(BfevReader reader, T[]? array = null)
        {
            Read(reader);

            if (array != null) {
                LinkToArray(array);
            }
        }

        public void LinkToArray(T[] array)
        {
            if (array.Length != StaticKeys.Length) {
                throw new Exception($"Could not link {typeof(T).Name}[{array.Length}] to RadixTree<{typeof(T).Name}> because the array lengths did not match.",
                    new InvalidDataException($"Could not fit an array with length of '{array.Length}' into {StaticKeys.Length}.")
                );
            }

            try {
                for (int i = 0; i < array.Length; i++) {
                    Add(StaticKeys[i], array[i]);
                }
            }
            catch { }
        }

        public IBfevDataBlock Read(BfevReader reader)
        {
            reader.CheckMagic(RadixTreeWriter.Magic);
            int count = reader.ReadInt32();
            reader.BaseStream.Position += 4 + 2 + 2 + 8; // Root entry

            StaticKeys = new string[count];
            for (int i = 0; i < count; i++) {
                reader.BaseStream.Position += 4 + 2 + 2;
                StaticKeys[i] = reader.ReadStringPtr();
            }

            return this;
        }

        public void Write(BfevWriter writer)
        {
            RadixTreeHelper.WriteRadixTree(writer, Keys.ToArray());
        }
    }
}

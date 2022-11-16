using EvflLibrary.Common;
using EvflLibrary.Parsers;

namespace EvflLibrary.Core
{
    public class RadixTree<T> : IEvflDataBlock
    {
        internal string[] Keys = Array.Empty<string>();
        internal Dictionary<string, T> Values = new();

        public int Count => Values.Count;

        public T this[string key] {
            get => Values[key];
            set {
                if (Values.ContainsKey(key)) {
                    Values[key] = value;
                }
                else {
                    Values.Add(key, value);
                }
            }
        }

        public RadixTree() { }
        public RadixTree(EvflReader reader) => Read(reader);

        public void LinkToArray(T[] array)
        {
            if (array.Length != Keys.Length) {
                throw new Exception($"Could not link {typeof(T).Name}[{array.Length}] to RadixTree<{typeof(T).Name}> because the array lengths did not match.", 
                    new InvalidDataException($"Could not fit an array with length of '{array.Length}' into {Keys.Length}.")
                );
            }

            try {
                for (int i = 0; i < array.Length; i++) {
                    Values.Add(Keys[i], array[i]);
                }
            } catch { }
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            reader.CheckMagic(RadixTreeWriter.Magic);
            int count = reader.ReadInt32();
            reader.BaseStream.Position += 4 + 2 + 2 + 8; // Root entry

            Keys = new string[count];
            for (int i = 0; i < count; i++) {
                reader.BaseStream.Position += 4 + 2 + 2;
                Keys[i] = reader.ReadString();
            }

            return this;
        }

        public void Write(EvflWriter writer)
        {
            string[] keys = Values.Keys.ToArray();

            writer.Write(RadixTreeWriter.Magic.ToCharArray());
            writer.Write(keys.Length);

            var radixTree = RadixTreeWriter.ComputeTree(keys);
            foreach ((string name, var entry) in radixTree) {
                writer.Write(entry.BitIdx);
                writer.Write((ushort)radixTree[entry.Indices[0].Name].Index);
                writer.Write((ushort)radixTree[entry.Indices[1].Name].Index);
                writer.WriteStringPtr(name);
            }
        }
    }
}

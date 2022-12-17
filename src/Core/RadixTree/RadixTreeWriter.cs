using System.Diagnostics;
using static EvflLibrary.Core.RadixTreeHelper;

namespace EvflLibrary.Core
{
    public class RadixTreeWriter
    {
        public const string Magic = "DIC ";

        [DebuggerDisplay($"[{{{nameof(BitIdx)}}}] {{{nameof(Name)}}}")]
        public record Entry
        {
            public int Index;
            public string Name = "";
            public int BitIdx = -1;
            public int[] NextIndices = new int[2] { 0, 0 };
            public Entry[] Indices;
            public Entry? Parent;

            public Entry() => Indices = new Entry[2] { this, this };
            public Entry(string name, int bitIdx, Entry? parent) : this()
            {
                Name = name;
                BitIdx = bitIdx;
                Parent ??= parent;
            }
        }

        public readonly Entry Root;
        public readonly Dictionary<string, Entry> RadixTree = new();

        /// <returns>
        /// The computed radix tree as a <see cref="Dictionary{string, Entry}"/> constructed from the provided <paramref name="keys"/>
        /// </returns>
        public static Dictionary<string, Entry> ComputeTree(params string[] keys) => new RadixTreeWriter(keys).RadixTree;

        public RadixTreeWriter(string[] keys)
        {
            Root = new("", -1, Root);
            RadixTree.Add("", Root);

            for (int i = 0; i < keys.Length; i++) {
                Insert(keys[i]);
            }
        }

        public Entry FindEntry(string key, bool returntParent = false)
        {
            if (Root.Indices[0] == Root) {
                return Root;
            }

            Entry prev;
            Entry entry = Root.Indices[0];

            while (true) {
                prev = entry;
                entry = entry.Indices[GetNextEntryIndex(key, entry.BitIdx)];

                if (entry.BitIdx <= prev.BitIdx) {
                    break;
                }
            }

            return returntParent ? prev : entry;
        }

        public void Insert(string name)
        {
            Entry entry;
            Entry current = FindEntry(name, returntParent: true);
            int bitIdx = GetBitIndex(name, current.Name);

            // Get the lowest position that is still
            // higher than the new node
            while (bitIdx < current.Parent?.BitIdx)
                current = current.Parent;

            // Create a new entry as a parent of the current entry
            if (bitIdx < current.BitIdx) {
                entry = new(name, bitIdx, current.Parent);
                entry.Indices[GetNextEntryIndex(name, bitIdx) ^ 1] = current;
                current.Parent!.Indices[GetNextEntryIndex(name, current.Parent.BitIdx)] = entry;
                current.Parent = entry;
            }
            else if (bitIdx > current.BitIdx) {
                int entryNextIndex = GetNextEntryIndex(name, bitIdx);

                // Insert as a child of the current node as our bit index is higher,
                // which means the new node is deeper in the tree.

                entry = new(name, bitIdx, current);
                entry.Indices[entryNextIndex ^ 1] = GetNextEntryIndex(current.Name, bitIdx) == (entryNextIndex ^ 1) ? current : Root;
                current.Indices[GetNextEntryIndex(name, current.BitIdx)] = entry;
            }
            else {
                int entryNextIndex = GetNextEntryIndex(name, bitIdx);

                // Both nodes have the same depth: insert the new node as a child of the current node.
                // Preserve tree invariants (bit indices must increase during traversal)
                // by using a higher bit index.
                // Nintendo's algorithm seems to use the index of the first set bit.

                bitIdx = GetFirstSetBitIndex(name);

                // If the current node pointed to another node, use the bit that differentiates
                // the new node from the other one.
                if (current.Indices[entryNextIndex] != Root) {
                    bitIdx = GetBitIndex(current.Indices[entryNextIndex].Name, name);
                }

                entry = new(name, bitIdx, current);
                entry.Indices[GetNextEntryIndex(name, bitIdx) ^ 1] = current.Indices[entryNextIndex];
                current.Indices[entryNextIndex] = entry;
            }

            InsertEntry(name, entry);
        }

        public void InsertEntry(string key, Entry entry)
        {
            entry.Index = RadixTree.Count;
            RadixTree.Add(key, entry);
        }
    }
}

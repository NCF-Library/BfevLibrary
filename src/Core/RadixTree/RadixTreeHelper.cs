﻿using BfevLibrary.Parsers;

namespace BfevLibrary.Core;

public static class RadixTreeHelper
{
    /// <summary>
    /// Writes a RadixTree from an array of keys without initializing a new <see cref="RadixTree{T}"/>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="keys"></param>
    public static void WriteRadixTree(this BfevWriter writer, string[] keys)
    {
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

    /// <returns>
    /// The next traverse index for the given <paramref name="key"/> and <paramref name="bitIndex"/>
    /// </returns>
    public static int GetNextEntryIndex(string key, int bitIndex)
    {
        if (key != "" && bitIndex != -1 && key.Length > bitIndex >> 3) {
            return key[^((bitIndex >> 3) + 1)] >> (bitIndex & 0b0000_0111) & 0b0000_0001;
        }
        else {
            return 0;
        }
    }

    /// <returns>
    /// The index of the first non-zero (set) bit
    /// in the provided <see cref="string"/>.
    /// </returns>
    public static int GetFirstSetBitIndex(string str)
    {
        int index = 0;
        for (int i = 0; i < str.Length * 8; i++) {
            char current = str[^(i + 1)];
            if (current == 0) {
                index += 8;
            }
            else {
                int check = 0b0000_0001;
                int isolated = current & ~(current - 1);
                while ((isolated & check) == 0) {
                    check <<= 1;
                    index++;
                }
                break;
            }
        }

        return index;
    }

    /// <returns>
    /// The index of the first different bit comparing
    /// <see cref="string"/> (<paramref name="compare"/>)
    /// with <see cref="string"/> (<paramref name="compareWith"/>)
    /// </returns>
    public static int GetBitIndex(string compare, string compareWith)
    {
        int len;

        if (compare.Length > compareWith.Length) {
            len = compare.Length;
            compareWith = compareWith.PadLeft(len, '\0');
        }
        else {
            len = compareWith.Length;
            compare = compare.PadLeft(len, '\0');
        }

        int bitIndex = 0;
        for (int i = 0; i < len; i++) {
            int diff = compare[^(i + 1)] ^ compareWith[^(i + 1)];
            if (diff == 0) {
                bitIndex += 8;
            }
            else {
                int check = 0b0000_0001;
                int isolated = diff & ~(diff - 1);
                while ((isolated & check) == 0) {
                    check <<= 1;
                    bitIndex++;
                }
                break;
            }
        }

        return bitIndex;
    }
}

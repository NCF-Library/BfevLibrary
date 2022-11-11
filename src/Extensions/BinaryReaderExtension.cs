using System.Xml.Linq;

namespace EvflLibrary.Extensions
{
    public static class BinaryReaderExtension
    {
        public static string ReadStringPtr(this BinaryReader reader)
        {
            return new(reader.TemporarySeek(reader.ReadInt64(), SeekOrigin.Begin, () => {
                ushort size = reader.ReadUInt16();
                return reader.ReadChars(size);
            }));
        }

        public static void TemporarySeek(this BinaryReader reader, long position, SeekOrigin origin, Action read)
        {
            long initPos = reader.BaseStream.Position;
            reader.BaseStream.Seek(position, origin);
            read();
            reader.BaseStream.Position = initPos;
        }

        public static T TemporarySeek<T>(this BinaryReader reader, long position, SeekOrigin origin, Func<T> read)
        {
            long initPos = reader.BaseStream.Position;
            reader.BaseStream.Seek(position, origin);
            T readResults = read();
            reader.BaseStream.Position = initPos;
            return readResults;
        }
    }
}

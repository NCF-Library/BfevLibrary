namespace EvflLibrary.Parsers
{
    public class EvflReader : BinaryReader
    {
        public EvflReader(Stream stream) : base(stream) { }

        /// <summary>
        /// Aligns the stream to <paramref name="align"/> bytes
        /// </summary>
        public void Align(int align)
        {
            BaseStream.Position = (BaseStream.Position + align - 1) & -align;
        }

        public T? ReadObjectPtr<T>(Func<T> read, long? offset = null)
        {
            offset ??= ReadInt64();
            if (offset > 0) {
                return TemporarySeek<T>((long)offset, SeekOrigin.Begin, read);
            }

            return default;
        }

        public T[] ReadObjects<T>(T[] objects, Func<T> read)
        {
            for (int i = 0; i < objects.Length; i++) {
                objects[i] = read();
            }

            return objects;
        }

        public T[] ReadObjectsPtr<T>(T[] objects, Func<T> read, long? offset = null)
        {
            offset ??= ReadInt64();
            if (offset > 0) {
                TemporarySeek((long)offset, SeekOrigin.Begin, () => {
                    for (int i = 0; i < objects.Length; i++) {
                        objects[i] = read();
                    }
                });
            }

            return objects;
        }

        public T[] ReadObjectOffsetsPtr<T>(T[] objects, Func<T> read, long? offsetsPtr = null)
        {
            offsetsPtr ??= ReadInt64();
            TemporarySeek((long)offsetsPtr, SeekOrigin.Begin, () => {
                for (int i = 0; i < objects.Length; i++) {
                    long offset = ReadInt64();
                    objects[i] = TemporarySeek<T>(offset, SeekOrigin.Begin, read);
                }
            });

            return objects;
        }

        public string ReadStringPtr()
        {
            return new(TemporarySeek(ReadInt64(), SeekOrigin.Begin, () => {
                ushort size = ReadUInt16();
                return ReadChars(size);
            }));
        }

        public string ReadStringAtOffset(uint offset)
        {
            return new(TemporarySeek(offset, SeekOrigin.Begin, () => {
                ushort size = ReadUInt16();
                return ReadChars(size);
            }));
        }

        public void TemporarySeek(long position, SeekOrigin origin, Action read)
        {
            long initPos = BaseStream.Position;
            BaseStream.Seek(position, origin);
            read();
            BaseStream.Position = initPos;
        }

        public T TemporarySeek<T>(long position, SeekOrigin origin, Func<T> read)
        {
            long initPos = BaseStream.Position;
            BaseStream.Seek(position, origin);
            T readResults = read();
            BaseStream.Position = initPos;
            return readResults;
        }

        public bool CheckMagic(string magic, bool throwException = true)
        {
            string foundMagic = new(ReadChars(magic.Length));
            return foundMagic == magic || (throwException ? throw new InvalidDataException($"Invalid magic. The parser found '{foundMagic}' instead of '{magic}'") : false);
        }
    }
}

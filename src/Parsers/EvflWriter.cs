using EvflLibrary.Common;
using System.Text;

namespace EvflLibrary.Parsers
{
    public class EvflWriter : BinaryWriter
    {
        public List<long> Pointers { get; set; } = new();
        public Dictionary<string, List<Action>> Strings { get; set; } = new();
        public Dictionary<string, List<Action>> ReservedBlocks { get; set; } = new();
        public long RelocationTableOffset { get; set; } = 0L;

        public EvflWriter(Stream stream) : base(stream)
        {
            // Add the empty string
            Strings.Add("", new());
        }

        /// <summary>
        /// Aligns the stream to <paramref name="align"/> bytes
        /// </summary>
        public void Align(int align)
        {
            BaseStream.Position = (BaseStream.Position + align - 1) & -align;
        }

        /// <summary>
        /// Reserve an offset with a dynamic size
        /// </summary>
        /// <param name="write">
        /// Invoked when the return action is
        /// called (writes into the offset)
        /// </param>
        /// <param name="key">Adds the invokable action to <see cref="Reserved"/> with this key</param>
        /// <returns></returns>
        public Action ReserveOffset(Action<long> write, string? key = null)
        {
            long offset = BaseStream.Position;
            void insert() {
                long pos = BaseStream.Position;
                TemporarySeek(offset, SeekOrigin.Begin, () => write(pos));
            };

            if (key != null) {
                if (!ReservedBlocks.ContainsKey(key)) {
                    ReservedBlocks.Add(key, new());
                }

                ReservedBlocks[key].Add(insert);
            }

            write(0L);
            return insert;
        }

        /// <summary>Reserves a 32-bit offset</summary>
        public Action ReserveOffset(string? key = null)
        {
            return ReserveOffset((pos) => Write((uint)pos), key);
        }

        /// <summary>Reserves a 32-bit offset relative to the current position</summary>
        public Action ReserveRelativeOffset(string? key = null, long? relativePosition = null)
        {
            relativePosition ??= BaseStream.Position;
            return ReserveOffset((pos) => Write((uint)(pos - relativePosition)), key);
        }

        /// <summary>Reserves a 64-bit offset (ptr)</summary>
        public Action ReservePtr(string? key = null)
        {
            RegisterPtr();
            return ReserveOffset(Write, key);
        }

        /// <summary>Reserves a 64-bit offset (ptr) if <paramref name="condition"/> returns true</summary>
        public Action ReservePtrIf(bool condition, string? key = null, bool register = false, bool nullPtr = true)
        {
            if (condition) {
                return ReservePtr(key);
            }
            else {
                if (nullPtr) {
                    WriteNullPtr(register);
                }
                return () => { };
            }
        }

        /// <summary>
        /// Reserves a set of actions to be executed
        /// at a different position in the stream
        /// </summary>
        /// <param name="key"></param>
        /// <param name="writers"></param>
        public void ReserveBlockWriter(string key, Action blockWriter)
        {
            if (!ReservedBlocks.ContainsKey(key)) {
                ReservedBlocks.Add(key, new());
            }

            ReservedBlocks[key].Add(blockWriter);
        }

        public void WriteNullPtr(bool register = false)
        {
            if (register) {
                RegisterPtr();
            }
            Write(0L);
        }

        public void RegisterPtr(long? offset = null)
        {
            if (offset != null) {
                Pointers.Add((long)offset);
            }
            else {
                Pointers.Add(BaseStream.Position);
            }
        }

        /// <summary>
        /// Executes a set of previously reserved actions
        /// at the current position in the stream
        /// </summary>
        /// <param name="key"></param>
        public bool WriteReserved(string key, bool remove = false, int? alignment = null)
        {
            if (ReservedBlocks.ContainsKey(key)) {
                foreach (var action in ReservedBlocks[key]) {
                    if (alignment != null) {
                        Align((int)alignment);
                    }
                    action();
                }

                if (remove) {
                    ReservedBlocks.Remove(key);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Writes a generic <see cref="IEvflDataBlock"/> into the stream
        /// </summary>
        public void WriteDataBlock(IEvflDataBlock evflDataBlock)
        {
            evflDataBlock.Write(this);
        }

        public void WriteObjects(IEnumerable<IEvflDataBlock> objects)
        {
            foreach (var obj in objects) {
                obj.Write(this);
            }
        }

        public void WriteStringPtr(string str)
        {
            if (!Strings.ContainsKey(str)) {
                Strings.Add(str, new());
            }

            Action<long> insert;
            long offset = BaseStream.Position;

            // Check to see if the string ref is the header name offset (pos 16)
            if (offset == 16) {
                insert = (pos) => Write((uint)pos + 2);
                Write(0U);
            }
            else {
                insert = Write;
                RegisterPtr();
                Write(0L);
            }

            Strings[str].Add(() => {
                long pos = BaseStream.Position;
                TemporarySeek(offset, SeekOrigin.Begin, () => insert(pos));
            });
        }

        public void WriteInlineStringPtrs(int alignment, params string[] strings)
        {
            Dictionary<string, Action> ptrs = new();

            for (int i = 0; i < strings.Length; i++) {
                ptrs.Add(strings[i], ReservePtr());
            }

            foreach (string str in strings) {
                // Strings aligned to 2 aren't aligned
                // until after the first string
                if (alignment == 2 && str != strings[0]) {
                    Align(alignment);
                }
                ptrs[str].Invoke();
                WritePascalString(str);
            }
        }

        public void WritePascalString(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            Write((ushort)data.Length);
            Write(data);
            Write('\x00');
        }

        public void TemporarySeek(long position, SeekOrigin origin, Action write)
        {
            long initPos = BaseStream.Position;
            BaseStream.Seek(position, origin);
            write();
            BaseStream.Position = initPos;
        }

        public void WriteStringPool()
        {
            WriteReserved("insertFlowchartStringPoolOffset");
            WriteReserved("insertTimelineStringPoolOffset");
            Align(8);
            Write("STR "u8);
            Write(0U);
            Write(0UL);
            Write(Strings.Count - 1); // The empty string is not counted

            static string ReverseBinary(string str) => string.Join("", string.Join("", Encoding.UTF8.GetBytes(str).Select(n => Convert.ToString(n, 2).PadLeft(8, '0'))).Reverse());

            foreach (var str in Strings.OrderBy(x => ReverseBinary(x.Key))) {
                long offset = BaseStream.Position;
                foreach (var insertAction in str.Value) {
                    insertAction();
                }

                WritePascalString(str.Key);
                Align(2);
            }
        }


        public void WriteRelocationTable()
        {
            uint dataEnd = (uint)BaseStream.Position;
            Align(8);

            WriteReserved("insertRelocationTableOffset");
            Write("RELT"u8);
            Write((uint)BaseStream.Position - 4);

            // It's extremely unlikely that the number of entries will ever exceed 2^32 - 1,
            // so assume that only one section is needed.
            // (If a file does have that many sections, you should probably worry about the
            // offsets being 32 bit instead.)
            Write(1U);
            Seek(4, SeekOrigin.Current); // Padding (uint)

            Write(0L);
            Write(0U);
            Write(dataEnd);
            Write(0U);

            int entryCount = 0;
            Action insertEntryCount = ReserveOffset((pos) => {
                Write(entryCount);
            });

            List<long> pointers = Pointers.Distinct().ToList();
            IEnumerable<long> pointerList = pointers.Order();

            foreach (var pointer in pointerList) {
                if (!pointers.Contains(pointer)) {
                    continue;
                }

                int flag = 0;
                for (int i = 0; i < 32; i++) {
                    long address = pointer + 8 * i;
                    if (pointers.Contains(address)) {
                        flag |= 1 << i;
                        pointers.Remove(address);
                    }
                }

                Write((uint)pointer);
                Write((uint)flag);

                entryCount++;
            }

            insertEntryCount();
        }
    }
}

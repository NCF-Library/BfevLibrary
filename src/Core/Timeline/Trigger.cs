using EvflLibrary.Common;
using EvflLibrary.Parsers;

namespace EvflLibrary.Core
{
    public enum TriggerType : byte
    {
        Enter = 1, Leave = 2
    }

    public class Trigger : IEvflDataBlock
    {
        public ushort ClipIndex { get; set; }
        public TriggerType Type { get; set; }

        public Trigger(EvflReader reader)
        {
            Read(reader);
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            ClipIndex = reader.ReadUInt16();
            Type = (TriggerType)reader.ReadByte();
            reader.BaseStream.Position += 1; // Padding (byte)

            return this;
        }

        public void Write(EvflWriter writer)
        {
            writer.Write(ClipIndex);
            writer.Write((byte)Type);
            writer.Seek(1, SeekOrigin.Current);
        }
    }
}

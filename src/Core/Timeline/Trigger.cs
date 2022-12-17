using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core
{
    public enum TriggerType : byte
    {
        Enter = 1, Leave = 2
    }

    public class Trigger : IBfevDataBlock
    {
        public short ClipIndex { get; set; }
        public TriggerType Type { get; set; }

        [JsonConstructor]
        public Trigger(short clipIndex, TriggerType type)
        {
            ClipIndex = clipIndex;
            Type = type;
        }

        public Trigger(BfevReader reader)
        {
            Read(reader);
        }

        public IBfevDataBlock Read(BfevReader reader)
        {
            ClipIndex = reader.ReadInt16();
            Type = (TriggerType)reader.ReadByte();
            reader.BaseStream.Position += 1; // Padding (byte)

            return this;
        }

        public void Write(BfevWriter writer)
        {
            writer.Write(ClipIndex);
            writer.Write((byte)Type);
            writer.Seek(1, SeekOrigin.Current);
        }
    }
}

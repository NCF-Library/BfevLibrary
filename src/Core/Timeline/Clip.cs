using EvflLibrary.Common;
using EvflLibrary.Parsers;

namespace EvflLibrary.Core
{
    public class Clip : IEvflDataBlock
    {
        public float StartTime { get; set; }
        public float Duration { get; set; }
        public short ActorIndex { get; set; }
        public short ActorActionIndex { get; set; }
        public byte Unknown { get; set; }
        public Container? Parameters { get; set; }

        public Clip()
        {
            Parameters = new();
        }

        public Clip(EvflReader reader)
        {
            Read(reader);
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            StartTime = reader.ReadSingle();
            Duration = reader.ReadSingle();
            ActorIndex = reader.ReadInt16();
            ActorActionIndex = reader.ReadInt16();
            Unknown = reader.ReadByte();
            reader.BaseStream.Position += 3; // Padding
            Parameters = reader.ReadObjectPtr<Container>(() => new(reader));

            return this;
        }

        public void Write(EvflWriter writer)
        {
            writer.Write(StartTime);
            writer.Write(Duration);
            writer.Write(ActorIndex);
            writer.Write(ActorActionIndex);
            writer.Write(Unknown);
            writer.Seek(3, SeekOrigin.Current); // Padding (byte[3])
            Action insertParamsPtr = writer.ReservePtrIf(Parameters?.CanWrite() ?? false);

            writer.ReserveBlockWriter("ClipArrayDataBlock", () => {
                if (Parameters?.CanWrite() ?? false) {
                insertParamsPtr();
                Parameters?.Write(writer);
                }
            });
        }
    }
}

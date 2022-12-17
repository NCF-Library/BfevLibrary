using EvflLibrary.Common;
using EvflLibrary.Parsers;

namespace EvflLibrary.Core
{
    public class Oneshot : IEvflDataBlock
    {
        public float Time { get; set; }
        public short ActorIndex { get; set; }
        public short ActorActionIndex { get; set; }
        public Container? Parameters { get; set; }

        public Oneshot()
        {
            Parameters = new();
        }

        public Oneshot(EvflReader reader)
        {
            Read(reader);
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            Time = reader.ReadSingle();
            ActorIndex = reader.ReadInt16();
            ActorActionIndex = reader.ReadInt16();
            reader.BaseStream.Position += 4 + 4; // Padding (uint), Padding (uint)
            Parameters = reader.ReadObjectPtr<Container>(() => new(reader));

            return this;
        }

        public void Write(EvflWriter writer)
        {
            writer.Write(Time);
            writer.Write(ActorIndex);
            writer.Write(ActorActionIndex);
            writer.Seek(4 + 4, SeekOrigin.Current); // Padding (uint), Padding (uint)
            Action insertParamsPtr = writer.ReservePtrIf(Parameters?.CanWrite() ?? false);

            writer.ReserveBlockWriter("OneshotArrayDataBlock", () => {
                if (Parameters?.CanWrite() ?? false) {
                    insertParamsPtr();
                    Parameters?.Write(writer);
                }
            });
        }
    }
}

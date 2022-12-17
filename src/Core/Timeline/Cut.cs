using EvflLibrary.Common;
using EvflLibrary.Parsers;

namespace EvflLibrary.Core
{
    public class Cut : IEvflDataBlock
    {
        public float StartTime { get; set; }
        public string Name { get; set; }
        public Container? Parameters { get; set; }

        public Cut()
        {
            Parameters = new();
        }

        public Cut(EvflReader reader)
        {
            Read(reader);
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            StartTime = reader.ReadSingle();
            reader.BaseStream.Position += 4; // Unknown/Padding (uint)
            Name = reader.ReadStringPtr();
            Parameters = reader.ReadObjectPtr<Container>(() => new(reader));

            return this;
        }

        public void Write(EvflWriter writer)
        {
            writer.Write(StartTime);
            writer.Seek(4, SeekOrigin.Current); // Unknown/Padding (uint)
            writer.WriteStringPtr(Name);
            Action insertParamsPtr = writer.ReservePtrIf(Parameters?.CanWrite() ?? false);

            writer.ReserveBlockWriter("CutArrayDataBlock", () => {
                if (Parameters?.CanWrite() ?? false) {
                    insertParamsPtr();
                    Parameters?.Write(writer);
                }
            });
        }
    }
}

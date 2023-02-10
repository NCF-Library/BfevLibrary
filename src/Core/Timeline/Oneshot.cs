using BfevLibrary.Common;
using BfevLibrary.Parsers;

namespace BfevLibrary.Core;

public class Oneshot : IBfevDataBlock
{
    public float Time { get; set; }
    public short ActorIndex { get; set; }
    public short ActorActionIndex { get; set; }
    public Container? Parameters { get; set; }

    public Oneshot()
    {
        Parameters = new();
    }

    public Oneshot(BfevReader reader)
    {
        Read(reader);
    }

    public IBfevDataBlock Read(BfevReader reader)
    {
        Time = reader.ReadSingle();
        ActorIndex = reader.ReadInt16();
        ActorActionIndex = reader.ReadInt16();
        reader.BaseStream.Position += 4 + 4; // Padding (uint), Padding (uint)
        Parameters = reader.ReadObjectPtr<Container>(() => new(reader));

        return this;
    }

    public void Write(BfevWriter writer)
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

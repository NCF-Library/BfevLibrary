using BfevLibrary.Common;
using BfevLibrary.Parsers;

namespace BfevLibrary.Core;

public class EntryPoint : IBfevDataBlock
{
    public List<ushort> SubFlowEventIndices { get; set; }
    public ushort EventIndex { get; set; }

    public EntryPoint() { }
    public EntryPoint(BfevReader reader)
    {
        Read(reader);
    }

    public IBfevDataBlock Read(BfevReader reader)
    {
        long subFlowEventIndicesPtr = reader.ReadInt64();
        reader.BaseStream.Position += 8 + 8; // unused (in botw) VariableDef pointers (ulong, ulong)
        ushort subFlowEventIndicesCount = reader.ReadUInt16();
        reader.BaseStream.Position += 2; // unused (in botw) VariableDef count (ushort)
        EventIndex = reader.ReadUInt16();
        reader.BaseStream.Position += 2; // padding
        SubFlowEventIndices = reader.ReadObjectsPtr(new ushort[subFlowEventIndicesCount], () => reader.ReadUInt16(), subFlowEventIndicesPtr).ToList();
        return this;
    }

    public void Write(BfevWriter writer)
    {
        Action insertSubFlowEventIndicesPtr = writer.ReservePtrIf(SubFlowEventIndices.Count > 0, register: true);
        writer.Write(0L); // Unused (in botw) VariableDef pointer (ulong)
        writer.WriteNullPtr(register: true); // Unused (in botw) VariableDef dict pointer (ulong)
        writer.Write((ushort)SubFlowEventIndices.Count);
        writer.Write((ushort)0); // Unused (in botw) VariableDefs count
        writer.Write(EventIndex);
        writer.Write((ushort)0); // Padding
        writer.ReserveBlockWriter("EntryPointArrayDataBlock", () => {
            if (SubFlowEventIndices.Count > 0) {
                insertSubFlowEventIndicesPtr();
                for (int i = 0; i < SubFlowEventIndices.Count; i++) {
                    writer.Write(SubFlowEventIndices[i]);
                }
                writer.Align(8);
            }

            // Not really sure what this is for, based
            // off evfl by leoetlino (evfl/entry_point.py)
            writer.Seek(24, SeekOrigin.Current);
        });
    }
}

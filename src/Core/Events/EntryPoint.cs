using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class EntryPoint
    {
        public long SubFlowEventIndicesPtr;
        public long VariableDefNamesDicPtr;
        public long VariableDefsPtr;
        public ushort SubFlowEventIndicesCount;
        public ushort VariableDefsCount;
        public ushort EventIndex;

        public EntryPoint(BinaryReader reader)
        {
            SubFlowEventIndicesPtr = reader.ReadInt64();
            VariableDefNamesDicPtr = reader.ReadInt64();
            VariableDefsPtr = reader.ReadInt64();
            SubFlowEventIndicesCount = reader.ReadUInt16();
            VariableDefsCount = reader.ReadUInt16();
            EventIndex = reader.ReadUInt16();
            reader.BaseStream.Position += 2;
        }
    }
}

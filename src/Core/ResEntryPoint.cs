using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ResEntryPoint
    {
        internal ushort SubFlowEventIndices;
        internal ResDic VariableDefNames;
        internal dynamic VariableDefs;
        internal ushort SubFlowEventIndicesCount;
        internal ushort VariableDefsCount;
        internal ushort EventIndex;
        internal ushort Padding;
    }
}

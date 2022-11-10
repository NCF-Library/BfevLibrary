using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public class ResEntryPoint
    {
        public ushort SubFlowEventIndices;
        public ResDic VariableDefNames;
        public dynamic VariableDefs;
        public ushort SubFlowEventIndicesCount;
        public ushort VariableDefsCount;
        public ushort EventIndex;
        public ushort Padding;
    }
}

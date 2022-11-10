using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public class ResActor
    {
        public string Name;
        public string SecondaryName;
        public string ArgumentName;
        public ResAction[] Actions;
        public ResQuery[] Queries;
        public ResMeta Parameters;
        public ushort ActionCount;
        public ushort QueryCount;
        public ushort EntryPointIndex; // Entry point index for associated entry point (0xffff if none)
        public byte CutNumber; // (?) Cut number? This is set to 1 for flowcharts. Timeline actors sometimes use a different value here. In BotW, this value is passed as the @MA actor parameter. https://zeldamods.org/wiki/BFEVFL#Actor
        public byte Padding;
    }
}

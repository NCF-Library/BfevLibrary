using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ResActor
    {
        internal string Name;
        internal string SecondaryName;
        internal string ArgumentName;
        internal ResAction[] Actions;
        internal ResQuery[] Queries;
        internal ResMeta Parameters;
        internal ushort ActionCount;
        internal ushort QueryCount;
        internal ushort EntryPointIndex; // Entry point index for associated entry point (0xffff if none)
        internal byte CutNumber; // (?) Cut number? This is set to 1 for flowcharts. Timeline actors sometimes use a different value here. In BotW, this value is passed as the @MA actor parameter. https://zeldamods.org/wiki/BFEVFL#Actor
        internal byte Padding;
    }
}

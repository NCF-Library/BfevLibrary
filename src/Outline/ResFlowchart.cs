using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Outline
{
    public class ResFlowchart
    {
        public string Magic = "EVFL";
        public uint StringPoolOffset;
        public uint Padding1;
        public uint Padding2;
        public ushort ActorCount;
        public ushort ActionCount;
        public ushort QueryCount;
        public ushort EventCount;
        public ushort EntryPointCount;
        public ushort Padding3;
        public ushort Padding4;
        public ushort Padding5;
        public string Name;
        public ResActor Actors;
        public ResEvent Events;
        public ResDic EntryPointDictionary;
        public ResEntryPoint EntryPoints;
    }
}

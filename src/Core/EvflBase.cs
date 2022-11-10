using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class EvflBase
    {
        //
        // Offsets


        //
        // Meta data

        internal string Magic = "BFEVFL\x00\x00";
        internal byte VersionMajor;
        internal byte VersionMinor;
        internal byte VersionPatch;
        internal byte VersionSubPatch;
        internal short ByteOrder;
        internal byte Padding1;
        internal int FileNameOffset;
        internal ushort IsRelocatedFlag;
        internal ushort FirstBlockOffset;
        internal ushort RelocationTableOffset;
        internal int FileSize;
        internal ushort FlowChartCount;
        internal ushort TimelineCount;
        internal uint Padding2;
        internal dynamic? FlowchartPtr;
        internal dynamic? FlowchartNameDictionary;
        internal dynamic? TimelinePtr;
        internal dynamic? TimelineNameDictionary;
    }
}

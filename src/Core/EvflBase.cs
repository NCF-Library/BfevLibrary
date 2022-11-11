using EvflLibrary.Extensions;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class EvflBase
    {
        public string Magic = "BFEVFL\x00\x00";
        public byte VersionMajor;
        public byte VersionMinor;
        public byte VersionPatch;
        public byte VersionSubPatch;
        public short ByteOrder;
        public int Alignment;
        public int FileNameOffset;
        public ushort IsRelocatedFlag;
        public ushort FirstBlockOffset;
        public int RelocationTableOffset;
        public int FileSize;
        public ushort FlowChartCount;
        public ushort TimelineCount;
        public long FlowchartOffsetPtr;
        public long FlowchartNameDictionaryOffset;
        public long TimelineOffsetPtr;
        public long TimelineNameDictionaryOffset;

        public long FlowchartOffset;
        public long TimelineOffset;

        public RelocationTable RelocationTable;

        public Flowchart? Flowchart;
        public ResDic? FlowchartNameDictionary;

        public Timeline? Timeline;
        public ResDic? TimelineNameDictionary;

        public EvflBase(string file)
        {
            using FileStream fs = File.OpenRead(file);
            using BinaryReader reader = new(fs);

            Magic = new(reader.ReadChars(6));
            fs.Position += 2; // Padding
            VersionMajor = reader.ReadByte();
            VersionMinor = reader.ReadByte();
            VersionPatch = reader.ReadByte();
            VersionSubPatch = reader.ReadByte();
            ByteOrder = reader.ReadInt16();
            Alignment = 1 << reader.ReadByte();
            fs.Position += 1; // Padding
            FileNameOffset = reader.ReadInt32();
            IsRelocatedFlag = reader.ReadUInt16();
            FirstBlockOffset = reader.ReadUInt16();
            RelocationTableOffset = reader.ReadInt32();
            FileSize = reader.ReadInt32();
            FlowChartCount = reader.ReadUInt16();
            TimelineCount = reader.ReadUInt16();
            fs.Position += 4; // Padding
            FlowchartOffsetPtr = reader.ReadInt64();
            FlowchartNameDictionaryOffset = reader.ReadInt64();
            TimelineOffsetPtr = reader.ReadInt64();
            TimelineNameDictionaryOffset = reader.ReadInt64();

            RelocationTable = reader.TemporarySeek<RelocationTable>(RelocationTableOffset, SeekOrigin.Begin, () => new(reader));

            if (FlowchartOffsetPtr != 0) {
                FlowchartOffset = reader.TemporarySeek(FlowchartOffsetPtr, SeekOrigin.Begin, () => reader.ReadInt64());
                FlowchartNameDictionary = reader.TemporarySeek<ResDic>(FlowchartNameDictionaryOffset, SeekOrigin.Begin, () => new(reader));
                Flowchart = reader.TemporarySeek<Flowchart>(FlowchartOffset, SeekOrigin.Begin, () => new(reader));
            }
            else if (TimelineOffsetPtr != 0) {
                TimelineOffset = reader.TemporarySeek(TimelineOffsetPtr, SeekOrigin.Begin, () => reader.ReadInt64());
                TimelineNameDictionary = reader.TemporarySeek<ResDic>(TimelineNameDictionaryOffset, SeekOrigin.Begin, () => new(reader));
                Timeline = reader.TemporarySeek<Timeline>(TimelineOffset, SeekOrigin.Begin, () => new(reader));
            }
            else {
                throw new NotImplementedException();
            }
        }
    }
}

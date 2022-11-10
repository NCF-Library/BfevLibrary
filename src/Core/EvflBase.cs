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
        public byte Alignment;
        public int FileNameOffset;
        public ushort IsRelocatedFlag;
        public ushort FirstBlockOffset;
        public int RelocationTableOffset;
        public int FileSize;
        public ushort FlowChartCount;
        public ushort TimelineCount;
        public ulong FlowchartOffsetPtr;
        public ulong FlowchartNameDictionaryOffset;
        public ulong TimelineOffsetPtr;
        public ulong TimelineNameDictionaryOffset;

        public void Read(string file)
        {
            using FileStream fs = File.OpenRead(file);
            using BinaryReader reader = new(fs);

            Magic = new(reader.ReadChars(8));
            VersionMajor = reader.ReadByte();
            VersionMinor = reader.ReadByte();
            VersionPatch = reader.ReadByte();
            VersionSubPatch = reader.ReadByte();
            ByteOrder = reader.ReadInt16();
            Alignment = reader.ReadByte();
            fs.Position += 1; // Padding
            FileNameOffset = reader.ReadInt32();
            IsRelocatedFlag = reader.ReadUInt16();
            FirstBlockOffset = reader.ReadUInt16();
            RelocationTableOffset = reader.ReadInt32();
            FileSize = reader.ReadInt32();
            FlowChartCount = reader.ReadUInt16();
            TimelineCount = reader.ReadUInt16();
            fs.Position += 4; // Padding
            FlowchartOffsetPtr = reader.ReadUInt64();
            FlowchartNameDictionaryOffset = reader.ReadUInt64();
            TimelineOffsetPtr = reader.ReadUInt64();
            TimelineNameDictionaryOffset = reader.ReadUInt64();

            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                Debug.WriteLine($"{field.Name}: {field.GetValue(this) ?? "null"}");
            }
        }
    }
}

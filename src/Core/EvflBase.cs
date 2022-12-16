using EvflLibrary.Common;
using EvflLibrary.Parsers;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class EvflBase : IEvflDataBlock
    {
        public const string Magic = "BFEVFL";

        public string FileName { get; set; }
        public string Version { get; set; } = "0.3.0.0";
        public RadixTree<Flowchart> Flowcharts { get; set; }
        public RadixTree<Timeline> Timelines { get; set; }

        [JsonIgnore]
        public Flowchart? Flowchart => Flowcharts.Count > 0 ? Flowcharts[0] : null;

        [JsonIgnore]
        public Timeline? Timeline => Timelines.Count > 0 ? Timelines[0] : null;

        public EvflBase(string file)
        {
            using FileStream fs = File.OpenRead(file);
            using EvflReader reader = new(fs);

            Read(reader);
        }

        public EvflBase(byte[] data)
        {
            using MemoryStream ms = new(data);
            using EvflReader reader = new(ms);

            Read(reader);
        }

        public EvflBase(Stream stream)
        {
            using EvflReader reader = new(stream);
            Read(reader);
        }

        public EvflBase()
        {
            Flowcharts = new();
            Timelines = new();
        }

        public IEvflDataBlock Read(EvflReader reader)
        {
            // Check the file magic
            reader.CheckMagic(Magic);

            // Padding
            reader.BaseStream.Position += 2;
            
            // Version (byte[4])
            Version = string.Join('.', reader.ReadBytes(4));
            
            // Byte order (2), alignment (1), padding (1)
            reader.BaseStream.Position += 4;
            
            // FileNameOffset (uint)
            FileName = reader.ReadStringAtOffset(reader.ReadUInt32() - 2);
            
            // IsRelocatedFlag (ushort), FirstBlockOffset (ushort), RelocationTableOffset (uint), FileSize (uint)
            reader.BaseStream.Position += 12;
            
            // FlowchartCount (ushort)
            var flowcharts = new Flowchart[reader.ReadUInt16()];
            
            // TimelineCount (ushort)
            var timelines = new Timeline[reader.ReadUInt16()];
            
            // Padding
            reader.BaseStream.Position += 4;

            // FlowchartOffsetsPtr (long)
            reader.ReadObjectOffsetsPtr(flowcharts, () => new(reader));

            // FlowchartNameDictionaryOffset (long)
            Flowcharts = reader.ReadObjectPtr(() => new RadixTree<Flowchart>(reader, flowcharts))!;

            // TimelineOffsetsPtr (long)
            reader.ReadObjectOffsetsPtr(timelines, () => new(reader));

            // TimelineNameDictionaryOffset (long)
            Timelines = reader.ReadObjectPtr(() => new RadixTree<Timeline>(reader, timelines))!;

            return this;
        }

        public void Write(EvflWriter writer)
        {
            // Write the file magic (byte[6]) and padding (byte[2])
            writer.Write(Magic.AsSpan());
            writer.Write((ushort)0);

            // Version (byte[4])
            writer.Write(Version.Split('.').Select(x => (byte)int.Parse(x)).ToArray());

            // Byte order (2), alignment (1), padding (1)
            writer.Write((ushort)0xFEFF);
            writer.Write((byte)3);
            writer.Write((byte)0);

            // FileNameOffset (uint)
            writer.WriteStringPtr(FileName);

            // IsRelocatedFlag (ushort), FirstBlockOffset (ushort), RelocationTableOffset (uint), FileSize (uint)
            writer.Write((ushort)0);
            Action insertFirstBlockOffset = writer.ReserveOffset(
                (pos) => writer.Write((ushort)pos), "insertFirstBlockOffset");
            writer.ReserveOffset("insertRelocationTableOffset");
            Action insertFileSize = writer.ReserveOffset();

            // FlowchartCount (ushort), TimelineCount (ushort), Padding (uint)
            writer.Write((ushort)Flowcharts.Count);
            writer.Write((ushort)Timelines.Count);
            writer.Write(0U);

            // Flowchart and Timeline block and dict offsets/ptrs
            Action insertFlowchartOffsetsPtr = writer.ReservePtrIf(Flowcharts.Count > 0, register: true);
            Action insertFlowchartDicPtr = writer.ReservePtr();
            Action insertTimelineOffsetsPtr = writer.ReservePtrIf(Timelines.Count > 0, register: true);
            Action insertTimelineDicPtr = writer.ReservePtr();

            // Insert block offsets
            insertFlowchartOffsetsPtr();
            writer.ReservePtrIf(Flowcharts.Count > 0, "insertFlowchartsOffsets", nullPtr: false);

            // Write dictionary
            insertFlowchartDicPtr();
            writer.WriteRadixTree(Flowcharts.Keys.ToArray());

            // Insert block offsets
            insertTimelineOffsetsPtr();
            writer.ReservePtrIf(Timelines.Count > 0, "insertTimelinesOffsets", nullPtr: false);

            // Write dictionary
            insertTimelineDicPtr();
            writer.WriteRadixTree(Timelines.Keys.ToArray());

            // Write Flowcharts
            foreach ((_, var flowchart) in Flowcharts) {
                flowchart.Write(writer);
            }

            // Write Timelines
            foreach ((_, var timeline) in Timelines) {
                timeline.Write(writer);
            }

            // Write String Pool
            writer.WriteStringPool();

            // Write Relocation Table
            writer.WriteRelocationTable();

            insertFileSize();
        }
    }
}

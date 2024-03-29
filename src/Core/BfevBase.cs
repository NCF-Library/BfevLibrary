﻿using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core;

public class BfevBase : IBfevDataBlock
{
    public const string Magic = "BFEVFL";

    public string FileName { get; set; }
    public string Version { get; set; } = "0.3.0.0";
    public RadixTree<Flowchart> Flowcharts { get; set; }
    public RadixTree<Timeline> Timelines { get; set; }

    [JsonIgnore]
    public Flowchart? Flowchart => Flowcharts.Count > 0 ? Flowcharts.First().Value : null;

    [JsonIgnore]
    public Timeline? Timeline => Timelines.Count > 0 ? Timelines.First().Value : null;

    internal BfevBase()
    {
        Flowcharts = new();
        Timelines = new();
    }

    internal BfevBase(Stream stream)
    {
        using BfevReader reader = new(stream);
        Read(reader);
    }

    public IBfevDataBlock Read(BfevReader reader)
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

    public void Write(BfevWriter writer)
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

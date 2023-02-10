using BfevLibrary.Parsers;

namespace BfevLibrary.Core;

public class ContainerData
{
    public string? Argument { get; set; }
    public RadixTree<ContainerItem>? Items { get; set; }
    public int? Int { get; set; }
    public bool? Bool { get; set; } // 0x80000001 if true, 0x00000000 otherwise
    public float? Float { get; set; }
    public string? String { get; set; }
    public string? WString { get; set; }
    public int[]? IntArray { get; set; }
    public bool[]? BoolArray { get; set; }
    public float[]? FloatArray { get; set; }
    public string[]? StringArray { get; set; }
    public string[]? WStringArray { get; set; }
    public Tuple<string, string>? ActorIdentifier { get; set; }

    public ContainerData() { }
    public void ReadData(BfevReader reader, ushort count, ContainerDataType type)
    {
        if (type == ContainerDataType.Argument) {
            Argument = reader.ReadStringPtr();
        }
        else if (type == ContainerDataType.Container) {
            Items!.LinkToArray(reader.ReadObjectOffsetsPtr(new ContainerItem[count], () => new(reader)));
        }
        else if (type == ContainerDataType.Int) {
            Int = reader.ReadInt32();
        }
        else if (type == ContainerDataType.Bool) {
            Bool = reader.ReadUInt32() == 0x80000001;
        }
        else if (type == ContainerDataType.Float) {
            Float = reader.ReadSingle();
        }
        else if (type == ContainerDataType.String) {
            String = reader.ReadStringPtr();
        }
        else if (type == ContainerDataType.WString) {
            WString = reader.ReadStringPtr();
        }
        else if (type == ContainerDataType.IntArray) {
            IntArray = new int[count];
            for (int i = 0; i < count; i++) {
                IntArray[i] = reader.ReadInt32();
            }
        }
        else if (type == ContainerDataType.BoolArray) {
            BoolArray = new bool[count];
            for (int i = 0; i < count; i++) {
                BoolArray[i] = reader.ReadUInt32() == 0x80000001;
            }
        }
        else if (type == ContainerDataType.FloatArray) {
            FloatArray = new float[count];
            for (int i = 0; i < count; i++) {
                FloatArray[i] = reader.ReadSingle();
            }
        }
        else if (type == ContainerDataType.StringArray) {
            StringArray = new string[count];
            for (int i = 0; i < count; i++) {
                StringArray[i] = reader.ReadStringPtr();
            }
        }
        else if (type == ContainerDataType.WStringArray) {
            WStringArray = new string[count];
            for (int i = 0; i < count; i++) {
                WStringArray[i] = reader.ReadStringPtr();
            }
        }
        else if (type == ContainerDataType.ActorIdentifier) {
            ActorIdentifier = new(reader.ReadStringPtr(), reader.ReadStringPtr());
        }
    }

    public void WriteData(BfevWriter writer)
    {
        if (Argument != null) {
            writer.WriteInlineStringPtrs(2, Argument);
        }
        else if (Items != null) {
            Items.Write(writer); // This writes a RadixTree, not the items array
            foreach (var item in Items.Values) {
                item.Write(writer);
            }
        }
        else if (Int != null) {
            writer.Write((int)Int);
        }
        else if (Bool != null) {
            writer.Write((bool)Bool ? 0x80000001 : 0x00000000);
        }
        else if (Float != null) {
            writer.Write((float)Float);
        }
        else if (String != null) {
            writer.WriteInlineStringPtrs(2, String);
        }
        else if (WString != null) {
            writer.WriteInlineStringPtrs(2, WString);
        }
        else if (IntArray != null) {
            for (int i = 0; i < IntArray.Length; i++) {
                writer.Write(IntArray[i]);
            }
        }
        else if (BoolArray != null) {
            for (int i = 0; i < BoolArray.Length; i++) {
                writer.Write(BoolArray[i] ? 0x80000001 : 0x00000000);
            }
        }
        else if (FloatArray != null) {
            for (int i = 0; i < FloatArray.Length; i++) {
                writer.Write(FloatArray[i]);
            }
        }
        else if (StringArray != null) {
            writer.WriteInlineStringPtrs(8, StringArray);
        }
        else if (WStringArray != null) {
            writer.WriteInlineStringPtrs(8, WStringArray);
        }
        else if (ActorIdentifier != null) {
            writer.WriteInlineStringPtrs(2, ActorIdentifier.Item1, ActorIdentifier.Item2);
        }
    }

    public int GetCount(ContainerDataType type)
    {
        return type switch {
            ContainerDataType.Argument => 1,
            ContainerDataType.Container => Items!.Count,
            ContainerDataType.Int => 1,
            ContainerDataType.Bool => 1,
            ContainerDataType.Float => 1,
            ContainerDataType.String => 1,
            ContainerDataType.WString => 1,
            ContainerDataType.IntArray => IntArray!.Length,
            ContainerDataType.BoolArray => BoolArray!.Length,
            ContainerDataType.FloatArray => FloatArray!.Length,
            ContainerDataType.StringArray => StringArray!.Length,
            ContainerDataType.WStringArray => WStringArray!.Length,
            ContainerDataType.ActorIdentifier => 2,
            _ => throw new NotImplementedException()
        };
    }

    public ContainerDataType GetDataType()
    {
        if (Argument != null) {
            return ContainerDataType.Argument;
        }
        else if (Items != null) {
            return ContainerDataType.Container;
        }
        else if (Int != null) {
            return ContainerDataType.Int;
        }
        else if (Bool != null) {
            return ContainerDataType.Bool;
        }
        else if (Float != null) {
            return ContainerDataType.Float;
        }
        else if (String != null) {
            return ContainerDataType.String;
        }
        else if (WString != null) {
            return ContainerDataType.WString;
        }
        else if (IntArray != null) {
            return ContainerDataType.IntArray;
        }
        else if (BoolArray != null) {
            return ContainerDataType.BoolArray;
        }
        else if (FloatArray != null) {
            return ContainerDataType.FloatArray;
        }
        else if (StringArray != null) {
            return ContainerDataType.StringArray;
        }
        else if (WStringArray != null) {
            return ContainerDataType.WStringArray;
        }
        else if (ActorIdentifier != null) {
            return ContainerDataType.ActorIdentifier;
        }
        else {
            throw new NotImplementedException();
        }
    }
}

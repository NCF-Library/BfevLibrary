using EvflLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvflLibrary.Core
{
    public class ContainerData
    {
        public string? Argument { get; set; }
        public ContainerItem[]? Container { get; set; }
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

        public ContainerData(BinaryReader reader, ushort count, ContainerDataType type)
        {
            if (type == ContainerDataType.Argument) {
                Argument = reader.ReadStringPtr(); // Pascal string or 4-byte long string?? 
            }
            else if (type == ContainerDataType.Container) {
                Container = new ContainerItem[count];
                for (int i = 0; i < count; i++) {
                    Container[i] = reader.TemporarySeek<ContainerItem>(reader.ReadInt64(), SeekOrigin.Begin, () => new(reader));
                }
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
    }
}

using BfevLibrary.Core;
using BfevLibrary.Parsers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BfevLibrary;

public class BfevFile : BfevBase
{
    public BfevFile() { }
    public BfevFile(Stream stream) : base(stream) { }

    public static BfevFile FromBinary(byte[] data)
    {
        using MemoryStream ms = new(data);
        return new(ms);
    }

    public static BfevFile FromBinary(string file)
    {
        using FileStream fs = File.OpenRead(file);
        return new(fs);
    }

    public static BfevFile FromBinary(Stream stream)
    {
        return new(stream);
    }

    public static BfevFile FromJson(string json)
    {
        return JsonSerializer.Deserialize<BfevFile>(json)
            ?? throw new JsonException("Could not deserialize BfevFile from json data (parser returned null)");
    }

    public byte[] ToBinary()
    {
        using MemoryStream ms = new();
        ToBinary(ms);
        return ms.ToArray();
    }

    public void ToBinary(string file)
    {
        using FileStream fs = File.Create(file);
        ToBinary(fs);
    }

    public void ToBinary(Stream stream)
    {
        using BfevWriter writer = new(stream);
        base.Write(writer);
    }

    public string ToJson(bool format = false)
    {
        JsonSerializerOptions options = new() {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = format
        };

        return JsonSerializer.Serialize(this, options);
    }

    [Obsolete("Use BfevFile.ToBinary()", true)]
    public new void Write(BfevWriter writer) { }

    [Obsolete("Use BfevFile.FromBinary()", true)]
    public new void Read(BfevReader reader) { }
}

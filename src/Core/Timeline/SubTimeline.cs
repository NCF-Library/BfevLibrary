using BfevLibrary.Common;
using BfevLibrary.Parsers;
using System.Text.Json.Serialization;

namespace BfevLibrary.Core;

public class SubTimeline : IBfevDataBlock
{
    public string Name { get; set; }

    [JsonConstructor]
    public SubTimeline(string name)
    {
        Name = name;
    }

    public SubTimeline(BfevReader reader)
    {
        Read(reader);
    }

    public IBfevDataBlock Read(BfevReader reader)
    {
        Name = reader.ReadStringPtr();
        return this;
    }

    public void Write(BfevWriter writer)
    {
        writer.WriteStringPtr(Name);
    }
}

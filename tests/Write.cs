using BfevLibrary.Core;
using BfevLibrary.Parsers;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Tests
{
    [TestClass]
    public class Write
    {
        [TestMethod]
        public void WriteEvFl()
        {
            BfevBase evfl = new(".\\Data\\NPC_artist_000.bfevfl");

            using FileStream fs = File.Create(".\\Data\\WRITE_NPC_artist_000.bfevfl");
            using BfevWriter writer = new(fs);
            evfl.Write(writer);
            fs.Dispose();

            BfevBase reEvfl = new(".\\Data\\WRITE_NPC_artist_000.bfevfl");
            string serialized = JsonSerializer.Serialize(reEvfl, new JsonSerializerOptions() {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\WRITE_NPC_artist_000.json", serialized);
        }

        [TestMethod]
        public void WriteEvTm()
        {
            BfevBase evtm = new(".\\Data\\Demo161_0.bfevtm");

            using FileStream fs = File.Create(".\\Data\\WRITE_Demo161_0.bfevtm");
            using BfevWriter writer = new(fs);
            evtm.Write(writer);
            fs.Dispose();

            BfevBase reEvtm = new(".\\Data\\WRITE_Demo161_0.bfevtm");
            string serialized = JsonSerializer.Serialize(reEvtm, new JsonSerializerOptions() {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\WRITE_Demo161_0.json", serialized);
        }
    }
}

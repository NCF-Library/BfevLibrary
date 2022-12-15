using EvflLibrary.Core;
using EvflLibrary.Parsers;
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
            EvflBase evfl = new(".\\Data\\100enemy.bfevfl");

            using FileStream fs = File.Create(".\\Data\\WRITE_100enemy.bfevfl");
            using EvflWriter writer = new(fs);
            evfl.Write(writer);
            fs.Dispose();

            EvflBase reEvfl = new(".\\Data\\WRITE_100enemy.bfevfl");
            string serialized = JsonSerializer.Serialize(reEvfl, new JsonSerializerOptions() {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\WRITE_100enemy.json", serialized);
        }

        [TestMethod]
        public void WriteEvTm()
        {
            EvflBase evtm = new(".\\Data\\Demo161_0.bfevtm");

            using FileStream fs = File.OpenWrite(".\\Data\\WRITE_Demo161_0.bfevtm");
            using EvflWriter writer = new(fs);
            evtm.Write(writer);
            fs.Dispose();

            EvflBase reEvtm = new(".\\Data\\WRITE_Demo161_0.bfevtm");
            string serialized = JsonSerializer.Serialize(reEvtm, new JsonSerializerOptions() {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\WRITE_Demo161_0.json", serialized);
        }
    }
}

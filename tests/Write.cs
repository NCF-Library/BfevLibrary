using BfevLibrary;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class Write
    {
        [TestMethod]
        public void WriteEvFl()
        {
            BfevFile evfl = new(".\\Data\\NPC_artist_000.bfevfl");
            evfl.ToBinary(".\\Data\\WRITE_NPC_artist_000.bfevfl");

            BfevFile reEvfl = new(".\\Data\\WRITE_NPC_artist_000.bfevfl");
            string serialized = reEvfl.ToJson(format: true);
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\WRITE_NPC_artist_000.json", serialized);
        }

        [TestMethod]
        public void WriteEvTm()
        {
            BfevFile evtm = new(".\\Data\\Demo161_0.bfevtm");
            evtm.ToBinary(".\\Data\\WRITE_Demo161_0.bfevtm");

            BfevFile reEvtm = new(".\\Data\\WRITE_Demo161_0.bfevtm");
            string serialized = reEvtm.ToJson(format: true);
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\WRITE_Demo161_0.json", serialized);
        }
    }
}

using BfevLibrary;
using BfevLibrary.Core;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class Write
    {
        [TestMethod]
        public void WriteEvFl()
        {
            BfevFile evfl = BfevFile.FromBinary(".\\Data\\NPC_artist_000.bfevfl");
            evfl.Flowchart!.Events.Add(new ActionEvent("Event56"));
            evfl.Flowchart!.Events.RemoveAt(27);
            evfl.ToBinary(".\\Data\\WRITE_NPC_artist_000.bfevfl");

            BfevFile reEvfl = BfevFile.FromBinary(".\\Data\\WRITE_NPC_artist_000.bfevfl");


            string serialized = reEvfl.ToJson(format: true);
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\WRITE_NPC_artist_000.json", serialized);
        }

        [TestMethod]
        public void WriteEvTm()
        {
            BfevFile evtm = BfevFile.FromBinary(".\\Data\\Demo161_0.bfevtm");
            evtm.ToBinary(".\\Data\\WRITE_Demo161_0.bfevtm");

            BfevFile reEvtm = BfevFile.FromBinary(".\\Data\\WRITE_Demo161_0.bfevtm");
            string serialized = reEvtm.ToJson(format: true);
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\WRITE_Demo161_0.json", serialized);
        }
    }
}

using BfevLibrary;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class Read
    {
        [TestMethod]
        public void ReadEvFl()
        {
            BfevFile evfl = new(".\\Data\\100enemy.bfevfl");
            string serialized = evfl.ToJson(format: true);
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\100enemy.json", serialized);
        }

        [TestMethod]
        public void ReadEvTm()
        {
            BfevFile evtm = new(".\\Data\\Demo161_0.bfevtm");
            string serialized = evtm.ToJson(format: true);
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Demo161_0.json", serialized);
        }
    }
}

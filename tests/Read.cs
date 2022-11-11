using EvflLibrary.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class Read
    {
        [TestMethod]
        public void ReadEvFl()
        {
            EvflBase evfl = new(".\\Data\\100enemy.bfevfl");
            string serialized = JsonConvert.SerializeObject(evfl, new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore });
            // Debug.WriteLine(serialized);
            File.WriteAllText(".\\100enemy.json", serialized);
        }

        [TestMethod]
        public void ReadEvTm()
        {
            EvflBase evfl = new(".\\Data\\Demo161_0.bfevtm");
            string serialized = JsonConvert.SerializeObject(evfl, new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore });
            // Debug.WriteLine(serialized);
            File.WriteAllText(".\\Demo161_0.json", serialized);
        }
    }
}

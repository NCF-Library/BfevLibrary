using EvflLibrary.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            string serialized = JsonSerializer.Serialize(evfl, new JsonSerializerOptions() {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Data\\100enemy.json", serialized);
        }

        [TestMethod]
        public void ReadEvTm()
        {
            EvflBase evtm = new(".\\Data\\Demo161_0.bfevtm");
            string serialized = JsonSerializer.Serialize(evtm, new JsonSerializerOptions() {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            Debug.WriteLine(serialized);
            File.WriteAllText(".\\Demo161_0.json", serialized);
        }
    }
}

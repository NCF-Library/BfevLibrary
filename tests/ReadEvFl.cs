using EvflLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ReadEvFl
    {
        [TestMethod]
        public void Read()
        {
            EvflBase evfl = new();
            evfl.Read(".\\Data\\100enemy.bfevfl");
        }
    }
}

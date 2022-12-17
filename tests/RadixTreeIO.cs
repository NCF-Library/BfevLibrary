using BfevLibrary.Core;
using BfevLibrary.Parsers;
using System.Text;

namespace Tests
{
    [TestClass]
    public class RadixTreeIO
    {
        [TestMethod]
        public void ReadWrite()
        {
            List<Tuple<string, int, ushort, ushort>> sampleTree = new() {
                new("CreateMode", 0, 0x0006, 0x0002),
                new("IsGrounding", 1, 0x0005, 0x0002),
                new("IsWorld", 2, 0x0004, 0x0003),
                new("PosX", 3, 0x0000, 0x0007),
                new("PosY", 2, 0x0008, 0x0001),
                new("PosZ", 1, 0x0003, 0x0009),
                new("RotX", 8, 0x0007, 0x0004),
                new("RotY", 8, 0x0008, 0x0005),
                new("RotZ", 8, 0x0009, 0x0006),
            };

            using MemoryStream stream = new();

            using BfevWriter writer = new(stream);
            string[] keys = sampleTree.Select(x => x.Item1).ToArray();
            writer.WriteRadixTree(keys);

            File.WriteAllBytes("D:\\dic.bin", stream.ToArray());

            stream.Position = 0;
            using BfevReader reader = new(stream);
            RadixTree<object> radixTree = new(reader);

            object[] badLinkedArray = new object[4];
            object[] goodLinkedArray = new object[9];

            bool didPass = false;
            try {
                radixTree.LinkToArray(badLinkedArray);
                didPass = true;
            }
            catch { }

            if (didPass) {
                throw new Exception("Error failed to occur with bad array linking");
            }

            try {
                radixTree.LinkToArray(goodLinkedArray);
            }
            catch (Exception ex) {
                throw new Exception("Failed to link with good array.", ex);
            }
        }
    }
}

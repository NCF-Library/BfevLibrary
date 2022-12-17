using BfevLibrary.Core;
using BfevLibrary.Parsers;
using Nintendo.Sarc;
using Nintendo.Yaz0;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tests.Exceptions;

namespace Tests
{
    [TestClass]
    public class Batch
    {
        [TestMethod]
        public void CheckFileIOAgainstVanilla()
        {
            string events = "D:\\Botw\\Cemu (Stable)\\mlc01\\usr\\title\\0005000e\\101c9500\\content\\Event";
            Dictionary<string, Dictionary<string, long>> benchmark = new();
            int count = 0;

            foreach (var file in Directory.EnumerateFiles(events)) {
                SarcFile sarc = new(Yaz0.Decompress(file));
                foreach ((var name, var data) in sarc.Files) {
                    string ext = Path.GetExtension(name);
                    if (ext == ".bfevtm" || ext == ".bfevfl") {
                        var timestamps = CheckEventFile(data);
                        benchmark.Add($"{Path.GetFileNameWithoutExtension(file)}/{name}", timestamps);
                        count++;
                    }
                }
            }

            Debug.WriteLine(count);
            File.WriteAllText("D:\\Bin\\BfevBenchmarks.json", JsonSerializer.Serialize(benchmark, new JsonSerializerOptions() { WriteIndented = true }));
        }

        public static Dictionary<string, long> CheckEventFile(byte[] data)
        {
            JsonSerializerOptions options = new() {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            Dictionary<string, long> timestamps = new();
            long mark;

            Stopwatch watch = Stopwatch.StartNew();

            BfevBase bfev = new(data);
            mark = watch.ElapsedMilliseconds;
            timestamps.Add($"Read {data.Length}", mark);
            watch.Restart();

            string serialized = JsonSerializer.Serialize(bfev, options);
            mark = watch.ElapsedMilliseconds;
            timestamps.Add($"Serialize {data.Length}", mark);
            watch.Restart();

            BfevBase deserialized = JsonSerializer.Deserialize<BfevBase>(serialized, options)!;
            mark = watch.ElapsedMilliseconds;
            timestamps.Add($"Deserialized {data.Length}", mark);
            watch.Restart();

            using MemoryStream ms = new();
            using BfevWriter writer = new(ms);
            deserialized.Write(writer);
            mark = watch.ElapsedMilliseconds;
            timestamps.Add($"Write {data.Length}", mark);
            watch.Restart();

            mark = watch.ElapsedMilliseconds;
            timestamps.Add($"Completed {data.Length}", timestamps.Values.Sum());

            byte[] newData = ms.ToArray();
            if (!Enumerable.SequenceEqual(data, newData)) {
                throw new BadBfevException() {
                    GoodBinary = data,
                    GoodMemory = bfev,
                    GoodSerialized = serialized,
                    BadBinary = newData,
                    BadMemory = deserialized,
                    BadSerialized = JsonSerializer.Serialize(deserialized, options)
                };
            }

            return timestamps;
        }
    }
}

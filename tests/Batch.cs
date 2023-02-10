using BfevLibrary;
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
            Stopwatch watch = Stopwatch.StartNew();
            long mark;

            BfevFile bfev = BfevFile.FromBinary(data);
            mark = watch.ElapsedMilliseconds;
            timestamps.Add($"Parse {data.Length}", mark);
            watch.Restart();

            string serialized = bfev.ToJson(true);
            mark = watch.ElapsedMilliseconds;
            timestamps.Add($"Serialize {data.Length}", mark);
            watch.Restart();

            BfevFile deserialized = BfevFile.FromJson(serialized);
            mark = watch.ElapsedMilliseconds;
            timestamps.Add($"Deserialize {data.Length}", mark);
            watch.Restart();

            byte[] newData = deserialized.ToBinary();
            mark = watch.ElapsedMilliseconds;
            timestamps.Add($"Write {data.Length}", mark);
            watch.Restart();

            // Summarize current benchmark
            timestamps.Add($"Completed {data.Length}", timestamps.Values.Sum());

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

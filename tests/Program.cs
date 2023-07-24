using BfevLibrary;

string output = "D:\\Bin\\Totk\\Bfev";

foreach (var file in Directory.GetFiles("D:\\Bin\\Totk\\EventFlow")) {
    string filename = Path.GetFileName(file);
    filename = Path.ChangeExtension(filename, ".json");
    string outFile = Path.Combine(output, filename);

    BfevFile bfev = BfevFile.FromBinary(file);
    File.WriteAllText(outFile, bfev.ToJson());

    Console.WriteLine(filename);
}
using BfevLibrary;

BfevFile evfl = BfevFile.FromBinary(@"D:\Bin\AutoRRG\ShopForRandomGoal.bfevfl");
evfl.Flowchart!.RemoveEntryPoint("Yorozuya_Kaitori");

byte[] data = evfl.ToBinary();
File.WriteAllBytes("D:\\Bin\\AutoRRG\\new.bfevfl", data);
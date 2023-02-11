using BfevLibrary;
using BfevLibrary.Core;
using BfevLibrary.Core.Collections;

BfevFile evfl = BfevFile.FromBinary(@"D:\Bin\AutoRRG\Randomizer_GoalMode.bfevfl");

Flowchart flowchart = evfl.Flowchart!;
EventList events = flowchart.Events;

events.RemoveAt(flowchart.EntryPoints["RandomGoal_Choose_no"].EventIndex, recursive: true);
evfl.Flowchart!.EntryPoints.Remove("RandomGoal_Choose_no");

byte[] data = evfl.ToBinary();
File.WriteAllBytes("D:\\Bin\\AutoRRG\\new.bfevfl", data);
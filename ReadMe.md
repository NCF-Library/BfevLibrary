# Bfev Library

Nintendo **B**inary ca**f**e **Ev**ent **Fl**ow Libray | Based on [evfl](https://github.com/zeldamods/evfl) by [Léo Lam](https://github.com/leoetlino)

*BfevLibrary has been tested to parse, serialize (json), deserialize (json), and write every BFEV file found in **Breath of the Wild** byte-perfectly.*

### Usage

### Reading a Bfev File

```cs
// Read from a File Path
BfevFile bfev = BfevFile.FromBinary("path/to/file.bfevfl");
```

```cs
// Read from a byte[]
// Do not use with File.ReadAllBytes(), use
// a Stream instead.
SarcFile sarc = SarcFile.FromBinary("D:/Botw/Update/content/Events/100enemy.sbeventpack");
BfevFile bfev = BfevFile.FromBinary(sarc["EventFlow/100enemy.bfevfl"]);
```

```cs
// Read from a Stream
using FileStream fs = File.OpenRead("path/to/file.bfevfl");
BfevFile bfev = new(fs);
```

```cs
// Read from JSON
string json = File.ReadAllText("path/to/file.bfevfl.json");
BfevFile bfev = BfevFile.FromJson(json);
```

### Writing a Bfev File

```cs
// Write to a File Path
bfev.ToBinary("path/to/file_out.bfevfl");
```

```cs
// Write to a byte[]
byte[] data = bfev.ToBinary();
```

```cs
// Write to a Stream
using FileStream fs = File.Create("path/to/file_out.bfevfl");
bfev.ToBinary(fs);
```

```cs
// Write to Json
string json = bfev.ToJson(format: true);
```

### Install

[![NuGet](https://img.shields.io/nuget/v/BfevLibrary.svg)](https://www.nuget.org/packages/BfevLibrary) [![NuGet](https://img.shields.io/nuget/dt/BfevLibrary.svg)](https://www.nuget.org/packages/BfevLibrary)

#### NuGet
```powershell
Install-Package BfevLibrary
```

#### Build From Source
```batch
git clone https://github.com/NCF-Library/BfevLibrary.git
dotnet build BfevLibrary
```

### Credits

- **[Arch Leaders](https://github.com/ArchLeaders):** C# Re-Implementation & JSON SerDe
- **[Léo Lam](https://github.com/leoetlino):** Original Python Implementation & General Help

# Bfev Library

Nintendo **B**inary ca**f**e **Ev**ent **Fl**ow Libray | Based on [evfl](https://github.com/zeldamods/evfl) by [Léo Lam](https://github.com/leoetlino)

*BfevLibrary has been tested to parse, serialize (json), deserialize (json), and write every BFEV file found in **Breath of the Wild** byte-perfectly.*

### Usage

**Read from a File Path**
```cs
BfevFile bfev = new("path/to/file.bfevfl");
```

**Read from a byte[]**
```cs
SarcFile sarc = new("D:/Botw/Update/content/Events/100enemy.sbeventpack");
BfevFile bfev = new(sarc.Files["EventFlow/100enemy.bfevfl"]);
```

**Read from a Stream**
```cs
using FileStream fs = File.OpenRead("path/to/file.bfevfl");
BfevFile bfev = new(fs);
```

**Read from JSON**
```cs
string json = File.ReadAllText("path/to/file.bfevfl.json");
BfevFile bfev = BfevFile.FromJson(json);
```

**Write to a File Path**
```cs
BfevFile bfev = new("path/to/file.bfevfl");
bfev.ToBinary("path/to/file_out.bfevfl");
```

**Write to a byte[]**
```cs
BfevFile bfev = new("path/to/file.bfevfl");
byte[] data = bfev.ToBinary();
```

**Write to a Stream**
```cs
BfevFile bfev = new("path/to/file.bfevfl");
using FileStream fs = File.Create("path/to/file_out.bfevfl");
bfev.ToBinary(fs);
```

**Write to Json**
```cs
BfevFile bfev = new("path/to/file.bfevfl");
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

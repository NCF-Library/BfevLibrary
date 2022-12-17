using EvflLibrary.Core;

namespace Tests.Exceptions
{
    public class BadEvflException : Exception
    {
        public string GoodSerialized { get; set; } = string.Empty;
        public string BadSerialized { get; set; } = string.Empty;
        public byte[] GoodBinary { get; set; } = Array.Empty<byte>();
        public byte[] BadBinary { get; set; } = Array.Empty<byte>();
        public EvflBase GoodMemory { get; set; } = new();
        public EvflBase BadMemory { get; set; } = new();
    }
}

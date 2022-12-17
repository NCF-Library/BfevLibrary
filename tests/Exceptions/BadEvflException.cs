using BfevLibrary.Core;

namespace Tests.Exceptions
{
    public class BadBfevException : Exception
    {
        public string GoodSerialized { get; set; } = string.Empty;
        public string BadSerialized { get; set; } = string.Empty;
        public byte[] GoodBinary { get; set; } = Array.Empty<byte>();
        public byte[] BadBinary { get; set; } = Array.Empty<byte>();
        public BfevBase GoodMemory { get; set; } = new();
        public BfevBase BadMemory { get; set; } = new();
    }
}

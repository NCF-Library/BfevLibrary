using System.Runtime.Serialization;

namespace BfevLibrary.Core.Exceptions;

public class BfevException : Exception
{
    public BfevException() { }
    public BfevException(string? message) : base(message) { }
    public BfevException(string? message, Exception? innerException) : base(message, innerException) { }
    protected BfevException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

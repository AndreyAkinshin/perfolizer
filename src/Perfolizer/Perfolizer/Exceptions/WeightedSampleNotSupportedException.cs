using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Perfolizer.Exceptions;

public class WeightedSampleNotSupportedException : ArgumentException
{
    [PublicAPI]
    public WeightedSampleNotSupportedException()
    {
    }

    [PublicAPI]
    protected WeightedSampleNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    [PublicAPI]
    public WeightedSampleNotSupportedException(string message) : base(message)
    {
    }

    [PublicAPI]
    public WeightedSampleNotSupportedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    [PublicAPI]
    public WeightedSampleNotSupportedException(string message, string paramName) : base(message, paramName)
    {
    }

    [PublicAPI]
    public WeightedSampleNotSupportedException(string message, string paramName, Exception innerException)
        : base(message, paramName, innerException)
    {
    }
}
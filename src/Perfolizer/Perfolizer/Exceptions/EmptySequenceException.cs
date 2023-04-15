namespace Perfolizer.Exceptions;

public class EmptySequenceException : InvalidOperationException
{
    public EmptySequenceException() : base("Sequence contains no elements")
    {
    }
}
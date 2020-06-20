using System.Collections.Generic;

namespace Perfolizer.Collections
{
    /// <summary>Represents a sorted read-only collection of elements that can be accessed by index.</summary>
    /// <typeparam name="T">The type of elements in the read-only list.</typeparam>
    public interface ISortedReadOnlyList<out T> : IReadOnlyList<T>
    {
    }
}
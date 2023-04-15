using System.Collections;

namespace Perfolizer.Collections;

internal class IdenticalReadOnlyList<T> : IReadOnlyList<T>
{
    public int Count { get; }
    public T Value { get; }

    public IdenticalReadOnlyList(int count, T value)
    {
        Count = count;
        Value = value;
    }

    public IEnumerator<T> GetEnumerator() => new IdenticalEnumerator(Count, Value);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public T this[int index] => Value;

    private class IdenticalEnumerator : IEnumerator<T>
    {
        private readonly int size;
        private int counter;

        public IdenticalEnumerator(int size, T value)
        {
            this.size = size;
            Current = value;
        }

        public bool MoveNext() => counter++ < size;
        public void Reset() => counter = 0;
        public T Current { get; }
        object? IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}
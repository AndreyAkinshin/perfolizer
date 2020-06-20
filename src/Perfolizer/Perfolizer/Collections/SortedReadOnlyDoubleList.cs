using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Collections
{
    public class SortedReadOnlyDoubleList : ISortedReadOnlyList<double>
    {
        [NotNull] private readonly IReadOnlyList<double> list;

        private SortedReadOnlyDoubleList([NotNull] IReadOnlyList<double> list)
        {
            this.list = list;
        }

        public static ISortedReadOnlyList<double> Create([NotNull] IReadOnlyList<double> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            
            for (int i = 0; i < list.Count - 1; i++)
                if (list[i] > list[i + 1])
                    return new SortedReadOnlyDoubleList(list.CopyToArrayAndSort());
            
            return new SortedReadOnlyDoubleList(list.CopyToArray());
        }

        public IEnumerator<double> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        public int Count => list.Count;

        public double this[int index] => list[index];
    }
}
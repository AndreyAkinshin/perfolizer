using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.Selectors
{
    /// <summary>
    /// Range Quantile Queries
    /// <remarks>
    /// Based on https://arxiv.org/pdf/0903.4726.pdf
    /// </remarks>
    /// </summary>
    public class Rqq
    {
        private enum NodeKind
        {
            Parent,
            Leaf,
            Fake
        }

        private enum BinaryFlag
        {
            LessThanMedian = 0,
            MoreThanMedian = 1,
            NotDefined = 2
        }

        private readonly double[] values;
        private readonly int[] rangeLeft, rangeRight;
        private readonly BinaryFlag[] b;
        private readonly int[] bSum;
        private readonly NodeKind[] kinds;
        private readonly int n, nodeCount, usedValues;

        public Rqq(double[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            n = data.Length;
            int logN = (int) Math.Ceiling(Math.Log(n));
            int valuesN = n * logN * 2; // TODO: Optimize
            values = new double[valuesN];
            b = new BinaryFlag[valuesN];
            bSum = new int[valuesN];

            // TODO: Optimize using bit hacks
            nodeCount = 1;
            while (nodeCount < 2 * n)
                nodeCount *= 2;
            nodeCount -= 1;

            rangeLeft = new int[nodeCount];
            rangeRight = new int[nodeCount];
            for (int i = 0; i < n; i++)
                values[i] = data[i];

            kinds = new NodeKind[nodeCount];
            for (int i = 0; i < nodeCount; i++)
                kinds[i] = NodeKind.Fake;

            rangeLeft[0] = 0;
            rangeRight[0] = n - 1;
            kinds[0] = NodeKind.Leaf;
            int valueCount = n;
            var selector = new QuickSelectAdaptive();
            for (int node = 0; node < nodeCount; node++)
            {
                if (kinds[node] == NodeKind.Fake)
                    continue;

                int leftIndex = rangeLeft[node];
                int rightIndex = rangeRight[node];
                if (leftIndex == rightIndex)
                {
                    kinds[node] = NodeKind.Leaf;
                    b[leftIndex] = BinaryFlag.NotDefined;
                    continue;
                }

                double median = selector.Select(values, (rightIndex - leftIndex) / 2, leftIndex, rightIndex);

                int child1 = node * 2 + 1;
                int child2 = node * 2 + 2;
                kinds[node] = NodeKind.Parent;
                kinds[child1] = NodeKind.Leaf;
                kinds[child2] = NodeKind.Leaf;

                int child1Size = (rightIndex - leftIndex + 2) / 2;
                int child2Size = rightIndex - leftIndex + 1 - child1Size;
                rangeLeft[child1] = valueCount;
                rangeRight[child1] = valueCount + child1Size - 1;
                rangeLeft[child2] = valueCount + child1Size;
                rangeRight[child2] = valueCount + child1Size + child2Size - 1;
                valueCount += child1Size + child2Size;
                int child1Counter = 0;
                int child2Counter = 0;

                // Preprocessing values that are less than median (filling b[i])
                for (int i = leftIndex; i <= rightIndex; i++)
                {
                    if (values[i] < median && child1Counter < child1Size)
                    {
                        b[i] = BinaryFlag.LessThanMedian;
                        child1Counter++;
                    }
                    else
                        b[i] = BinaryFlag.NotDefined;
                }

                // Preprocessing values that are equal to median (filling b[i])
                for (int i = leftIndex; i <= rightIndex && child1Counter < child1Size; i++)
                {
                    if (values[i] == median && b[i] == BinaryFlag.NotDefined)
                    {
                        b[i] = BinaryFlag.LessThanMedian;
                        child1Counter++;
                    }
                }

                // Processing all values (moving them to child nodes)
                child1Counter = 0;
                for (int i = leftIndex; i <= rightIndex; i++)
                {
                    switch (b[i])
                    {
                        case BinaryFlag.LessThanMedian:
                        {
                            // Copy value to the left child
                            values[rangeLeft[child1] + child1Counter++] = values[i];
                        }
                            break;
                        case BinaryFlag.NotDefined:
                        {
                            b[i] = BinaryFlag.MoreThanMedian;
                            // Copy value to the right child
                            values[rangeLeft[child2] + child2Counter++] = values[i];
                        }
                            break;
                    }

                    bSum[i] = i == leftIndex
                        ? (int) b[i]
                        : bSum[i - 1] + (int) b[i];
                }
            }

            usedValues = valueCount;
        }

        /// <summary>
        /// Returns p-th quantile in the [l;r] range
        /// </summary>
        public double GetQuantile(int l, int r, Probability p) => Select(l, r, (int) Math.Truncate((r - l) * p));

        /// <summary>
        /// Returns k-th element in a sorted array based on the [l;r] range
        /// </summary>
        public double Select(int l, int r, int k)
        {
            if (k < 0 || k > r - l)
                throw new ArgumentException(nameof(k), $"k ({k}) should be between 0 and r-l ({r - l})");
            if (l < 0 || l >= n)
                throw new ArgumentException(nameof(l), $"l ({l}) should be between 0 and n-1 ({n - 1})");
            if (r < 0 || r >= n)
                throw new ArgumentException(nameof(r), $"r ({r}) should be between 0 and n-1 ({n - 1})");

            int node = 0;
            while (true)
            {
                int shift = rangeLeft[node];
                int GetOneCount(int x, int y) => y >= 0 ? bSum[shift + y] - (x == 0 ? 0 : bSum[shift + x - 1]) : 0;
                int GetZeroCount(int x, int y) => y >= x ? y - x + 1 - GetOneCount(x, y) : 0;
                switch (kinds[node])
                {
                    case NodeKind.Parent:
                    {
                        int zkl = GetZeroCount(l, r);
                        int nextNode, nextL, nextR, nextK;
                        if (zkl > k)
                        {
                            nextNode = node * 2 + 1; // Left child
                            nextL = GetZeroCount(0, l - 1);
                            nextR = GetZeroCount(0, r) - 1;
                            nextK = k;
                        }
                        else
                        {
                            nextNode = node * 2 + 2; // Right child
                            nextL = GetOneCount(0, l - 1);
                            nextR = GetOneCount(0, r) - 1;
                            nextK = k - zkl;
                        }

                        node = nextNode;
                        l = nextL;
                        r = nextR;
                        k = nextK;
                    }
                        break;
                    case NodeKind.Leaf:
                        return values[rangeLeft[node]];
                    case NodeKind.Fake:
                        throw new InvalidOperationException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string DumpTreeAscii(bool details = false)
        {
            using (var memoryStream = new MemoryStream())
            using (var sw = new StreamWriter(memoryStream))
            {
                DumpTreeAscii(sw, details);
                return Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int) memoryStream.Length).TrimEnd();
            }
        }

        public void DumpTreeAscii(StreamWriter writer, bool details = false)
        {
            var valuesStr = values.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray(); // TODO: precision

            // Calculate width for each node
            var width = new int[nodeCount];
            for (int node = 0; node < nodeCount; node++)
            {
                width[node] += rangeRight[node] - rangeLeft[node] + 2; // result += <widths of spaces>
                if (kinds[node] != NodeKind.Fake)
                    for (int i = rangeLeft[node]; i <= rangeRight[node]; i++)
                        width[node] += valuesStr[i].Length; // result += <widths of values>
                width[node] += 2; // result += <widths of borders>
            }

            // Calculate padding for each node and connector-with-child positions 
            var paddingLeft = new int[nodeCount];
            var paddingRight = new int[nodeCount];
            var connectorDownLeft = new int[nodeCount];
            var connectorDownRight = new int[nodeCount];
            var connectorUp = new int[nodeCount];
            for (int node = nodeCount - 1; node >= 0; node--)
            {
                if (kinds[node] == NodeKind.Parent)
                {
                    int child1 = node * 2 + 1;
                    int child2 = node * 2 + 2;
                    int totalWidth1 = paddingLeft[child1] + width[child1] + paddingRight[child1];
                    int totalWidth2 = paddingLeft[child2] + width[child2] + paddingRight[child2];
                    int childrenWidth = totalWidth1 + 1 + totalWidth2;

                    int padding = Math.Max(0, childrenWidth - width[node]);
                    paddingLeft[node] = paddingLeft[child1] + width[child1] +
                                        (paddingRight[child1] + 1 + paddingLeft[child2]) / 2 -
                                        width[node] / 2;
                    paddingRight[node] = padding - paddingLeft[node];

                    connectorDownLeft[node] = 1;
                    connectorDownRight[node] = width[node] - 2;
                    connectorUp[child1] = paddingLeft[node] + connectorDownLeft[node] - paddingLeft[child1];
                    connectorUp[child2] = paddingLeft[node] + connectorDownRight[node] -
                                          (totalWidth1 + 1) - paddingLeft[child2];
                }
            }

            int layer = -1;
            while (true)
            {
                layer++;
                int node2 = (1 << (layer + 1)) - 2;
                int node1 = node2 - (1 << layer) + 1;
                if (node1 >= nodeCount)
                    break;

                void DumpLine(char leftBorder, char separator, char rightBorder, Func<int, string> element,
                    char? down = null, char? up = null)
                {
                    for (int node = node1; node <= node2; node++)
                    {
                        if (kinds[node] == NodeKind.Fake)
                        {
                            if (node % 2 == 1) // It's a left child
                            {
                                int parentIndex = (node - 1) / 2;
                                int parentWidth = 1 + paddingLeft[parentIndex] + width[parentIndex] +
                                                  paddingRight[parentIndex];
                                for (int j = 0; j < parentWidth; j++)
                                    writer.Write(' ');
                            }

                            continue;
                        }

                        int position = -paddingLeft[node];

                        void PrintChar(char c)
                        {
                            if (kinds[node] == NodeKind.Parent && down.HasValue &&
                                (position == connectorDownLeft[node] ||
                                 position == connectorDownRight[node]))
                                writer.Write(down.Value);
                            else if (position == connectorUp[node] && node > 0 && up.HasValue)
                                writer.Write(up.Value);
                            else
                                writer.Write(c);
                            position++;
                        }

                        void PrintString(string s)
                        {
                            foreach (var c in s)
                                PrintChar(c);
                        }

                        for (int j = 0; j < paddingLeft[node]; j++)
                            PrintChar(' ');
                        PrintChar(leftBorder);
                        PrintChar(separator);

                        for (int i = rangeLeft[node]; i <= rangeRight[node]; i++)
                        {
                            string elementStr = element(i);
                            int elementPadding = Math.Max(0, valuesStr[i].Length - elementStr.Length);
                            for (int j = 0; j < elementPadding; j++)
                                PrintChar(separator);
                            PrintString(elementStr);
                            if (i != rangeRight[node])
                                PrintChar(separator);
                        }

                        PrintChar(separator);
                        PrintChar(rightBorder);
                        for (int j = 0; j < paddingRight[node]; j++)
                            PrintChar(' ');
                        if (node != node2)
                            PrintChar(' ');
                    }

                    writer.WriteLine();
                }

                DumpLine('┌', '─', '┐', i => "─", up: '┴');
                DumpLine('│', ' ', '│', i => valuesStr[i]);
                DumpLine('│', ' ', '│', i => b[i] == BinaryFlag.NotDefined ? " " : ((int) b[i]).ToString());
                DumpLine('└', '─', '┘', i => "─", down: '┬');
                DumpLine(' ', ' ', ' ', i => "", down: '│');
            }

            writer.WriteLine();

            if (details)
            {
                for (int node = 0; node < nodeCount; node++)
                {
                    writer.Write('#');
                    writer.Write(node.ToString());
                    writer.Write(": ");

                    switch (kinds[node])
                    {
                        case NodeKind.Parent:
                            writer.Write("NODE(");
                            for (int i = rangeLeft[node]; i <= rangeRight[node]; i++)
                            {
                                writer.Write(valuesStr[i]);
                                if (i != rangeRight[node])
                                    writer.Write(',');
                            }

                            writer.Write("); Children = { #");
                            writer.Write(node * 2 + 1);
                            writer.Write(", #");
                            writer.Write(node * 2 + 2);
                            writer.WriteLine(" }");
                            break;
                        case NodeKind.Leaf:
                            writer.Write("LEAF(");
                            writer.Write(valuesStr[rangeLeft[node]]);
                            writer.WriteLine(")");
                            break;
                        case NodeKind.Fake:
                            writer.WriteLine("FAKE");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                writer.WriteLine();
                writer.WriteLine("Allocated values : " + values.Length);
                writer.WriteLine("Used values      : " + usedValues);
            }

            writer.Flush();
        }
    }
}
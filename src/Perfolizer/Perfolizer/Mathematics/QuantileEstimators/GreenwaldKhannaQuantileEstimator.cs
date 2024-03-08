using System.Text;
using Perfolizer.Common;
using Perfolizer.Exceptions;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators;

/// <summary>
/// <remarks>
/// Greenwald, Michael, and Sanjeev Khanna. “Space-efficient online computation of quantile summaries.”
/// ACM SIGMOD Record 30, no. 2 (2001): 58-66.
/// https://doi.org/10.1145/375663.375670
/// </remarks>
/// </summary>
public class GreenwaldKhannaQuantileEstimator : ISequentialQuantileEstimator
{
    private class Tuple
    {
        public static readonly IComparer<Tuple> Comparer = Comparer<Tuple>.Create((a, b) => a.Value.CompareTo(b.Value));
        public double Value { get; set; } // Observation v[i]
        public int Gap { get; set; } // rMin(v[i]) - rMin(v[i - 1])
        public int Delta { get; set; } // rMax(v[i]) - rMin(v[i])
    }

    private readonly List<Tuple> tuples = new();
    private readonly int compressingInterval;
    private int n;

    public double Epsilon { get; }
    public int Count => n;
    public int TupleCount => tuples.Count;

    public GreenwaldKhannaQuantileEstimator(double epsilon)
    {
        Assertion.Positive(nameof(epsilon), epsilon);
        Assertion.InRangeInclusive(nameof(epsilon), epsilon, 0, 0.5);
        Epsilon = epsilon;
        compressingInterval = (int)Math.Floor(1.0 / (2.0 * Epsilon));
    }

    public void Add(double v)
    {
        var t = new Tuple { Value = v, Gap = 1, Delta = (int)Math.Floor(2.0 * Epsilon * n) };
        int i = GetInsertIndex(t);
        if (i == 0 || i == tuples.Count)
            t.Delta = 0;
            
        tuples.Insert(i, t);
        n++;

        if (n % compressingInterval == 0)
            Compress();
    }

    private int GetInsertIndex(Tuple v)
    {
        int index = tuples.BinarySearch(v, Tuple.Comparer);
        return index >= 0 ? index : ~index;
    }

    public double Quantile(Probability p)
    {
        if (tuples.Count == 0)
            throw new EmptySequenceException();

        double rank = p * (n - 1) + 1;
        int margin = (int)Math.Ceiling(Epsilon * n);

        int bestIndex = -1;
        double bestDist = double.MaxValue;
        int rMin = 0;
        for (int i = 0; i < tuples.Count; i++)
        {
            var t = tuples[i];
            rMin += t.Gap;
            int rMax = rMin + t.Delta;
            if (rank - margin <= rMin && rMax <= rank + margin)
            {
                double currentDist = Math.Abs(rank - (rMin + rMax) / 2.0);
                if (currentDist < bestDist)
                {
                    bestDist = currentDist;
                    bestIndex = i;
                }
            }
        }
        if (bestIndex == -1)
            throw new InvalidOperationException("Failed to find the requested quantile");
        return tuples[bestIndex].Value;
    }

    public void Compress()
    {
        for (int i = tuples.Count - 2; i >= 1; i--)
            while (i < tuples.Count - 1 && DeleteIfNeeded(i))
            {
            }
    }

    private bool DeleteIfNeeded(int i)
    {
        Tuple t1 = tuples[i], t2 = tuples[i + 1];
        int threshold = (int)Math.Floor(2.0 * Epsilon * n);
        if (t1.Delta >= t2.Delta && t1.Gap + t2.Gap + t2.Delta < threshold)
        {
            tuples.RemoveAt(i);
            t2.Gap += t1.Gap;
            return true;
        }
        return false;
    }

    public override string ToString() => $"GreenwaldKhannaQuantileEstimator(eps={Epsilon})";

    #region Helpers for unit tests

    internal string DumpToString(string format = "N2")
    {
        if (tuples.Count == 0)
            return "";

        var rMaxBuilder = new StringBuilder("rMax  :");
        var valueBuilder = new StringBuilder("value :");
        var rMinBuilder = new StringBuilder("rMin  :");
        var detailBuilder = new StringBuilder("Tuples:");
        int indexW = (tuples.Count - 1).ToString().Length;
        int valueW = tuples.Max(t => t.Value.ToStringInvariant(format).Length);
        int gapW = tuples.Max(t => t.Gap.ToString().Length);
        int deltaW = tuples.Max(t => t.Delta.ToString().Length);
        int rMin = 0;
        for (int i = 0; i < tuples.Count; i++)
        {
            rMin += tuples[i].Gap;
            int rMax = rMin + tuples[i].Delta;
            string rMaxStr = rMax.ToString();
            string valueStr = tuples[i].Value.ToStringInvariant(format);
            string rMinStr = rMin.ToString();
            int w = new[] { rMaxStr.Length, valueStr.Length, rMinStr.Length }.Max() + 1;
            rMaxBuilder.Append(rMaxStr.PadLeft(w));
            valueBuilder.Append(valueStr.PadLeft(w));
            rMinBuilder.Append(rMinStr.PadLeft(w));
            detailBuilder.AppendLine(
                $"[{i.ToString().PadLeft(indexW)}]: " +
                $"v = {valueStr.PadLeft(valueW)}, " +
                $"g = {tuples[i].Gap.ToString().PadLeft(gapW)}, " +
                $"delta = {tuples[i].Delta.ToString().PadLeft(deltaW)}");
        }
        return string.Join(Environment.NewLine, rMaxBuilder, valueBuilder, rMinBuilder, "", detailBuilder);
    }

    internal void CheckConsistency()
    {
        foreach (var t in tuples)
            if (t.Gap + t.Delta > (int)Math.Ceiling(2 * Epsilon * n))
                throw new InvalidOperationException("Inconsistent state");
    }

    #endregion
}
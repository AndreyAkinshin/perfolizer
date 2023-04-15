using System;
using JetBrains.Annotations;
using Perfolizer.Exceptions;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators;

/// <summary>
/// Extended Piecewise-Parabolic (P^2) quantile estimator 
///
/// <remarks>
/// Based on the following paper:
/// Raatikainen, Kimmo EE. "Simultaneous estimation of several percentiles."
/// Simulation 49, no. 4 (1987): 159-163.
/// https://doi.org/10.1177/003754978704900405 <br /> 
///
/// See also:<br />
/// * https://aakinshin.net/posts/ex-p2-quantile-estimator/
/// </remarks>
/// </summary>
public class ExtendedP2QuantileEstimator : ISequentialQuantileEstimator
{
    internal readonly Probability[] Probabilities;
    private readonly int m, markerCount;
    private readonly int[] n;
    private readonly double[] ns;
    internal readonly double[] Q;

    public int Count { get; private set; }

    public ExtendedP2QuantileEstimator(params Probability[] probabilities)
    {
        this.Probabilities = probabilities;
        m = probabilities.Length;
        markerCount = 2 * m + 3;
        n = new int[markerCount];
        ns = new double[markerCount];
        Q = new double[markerCount];
    }

    private void UpdateNs(int maxIndex)
    {
        // Principal markers
        ns[0] = 0;
        for (int i = 0; i < m; i++)
            ns[i * 2 + 2] = maxIndex * Probabilities[i];
        ns[markerCount - 1] = maxIndex;

        // Middle markers
        ns[1] = maxIndex * Probabilities[0] / 2;
        for (int i = 1; i < m; i++)
            ns[2 * i + 1] = maxIndex * (Probabilities[i - 1] + Probabilities[i]) / 2;
        ns[markerCount - 2] = maxIndex * (1 + Probabilities[m - 1]) / 2;
    }

    public void Add(double value)
    {
        if (Count < markerCount)
        {
            Q[Count++] = value;
            if (Count == markerCount)
            {
                Array.Sort(Q);

                UpdateNs(markerCount - 1);
                for (int i = 0; i < markerCount; i++)
                    n[i] = (int)Math.Round(ns[i]);

                Array.Copy(Q, ns, markerCount);
                for (int i = 0; i < markerCount; i++)
                    Q[i] = ns[n[i]];
                UpdateNs(markerCount - 1);
            }

            return;
        }

        int k = -1;
        if (value < Q[0])
        {
            Q[0] = value;
            k = 0;
        }
        else
        {
            for (int i = 1; i < markerCount; i++)
                if (value < Q[i])
                {
                    k = i - 1;
                    break;
                }
            if (k == -1)
            {
                Q[markerCount - 1] = value;
                k = markerCount - 2;
            }
        }

        for (int i = k + 1; i < markerCount; i++)
            n[i]++;
        UpdateNs(Count);

        int leftI = 1, rightI = markerCount - 2;
        while (leftI <= rightI)
        {
            int i;
            if (Math.Abs(ns[leftI] / Count - 0.5) <= Math.Abs(ns[rightI] / Count - 0.5))
                i = leftI++;
            else
                i = rightI--;
            Adjust(i);
        }

        Count++;
    }

    private void Adjust(int i)
    {
        double d = ns[i] - n[i];
        if (d >= 1 && n[i + 1] - n[i] > 1 || d <= -1 && n[i - 1] - n[i] < -1)
        {
            int dInt = Math.Sign(d);
            double qs = Parabolic(i, dInt);
            if (Q[i - 1] < qs && qs < Q[i + 1])
                Q[i] = qs;
            else
                Q[i] = Linear(i, dInt);
            n[i] += dInt;
        }
    }

    private double Parabolic(int i, double d)
    {
        return Q[i] + d / (n[i + 1] - n[i - 1]) * (
            (n[i] - n[i - 1] + d) * (Q[i + 1] - Q[i]) / (n[i + 1] - n[i]) +
            (n[i + 1] - n[i] - d) * (Q[i] - Q[i - 1]) / (n[i] - n[i - 1])
        );
    }

    private double Linear(int i, int d)
    {
        return Q[i] + d * (Q[i + d] - Q[i]) / (n[i + d] - n[i]);
    }

    public double Quantile(Probability p)
    {
        if (Count == 0)
            throw new EmptySequenceException();
        if (Count <= markerCount)
        {
            Array.Sort(Q, 0, Count);
            int index = (int)Math.Round((Count - 1) * p);
            return Q[index];
        }

        for (int i = 0; i < m; i++)
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Probabilities[i] == p)
                return Q[2 * i + 2];

        throw new InvalidOperationException($"Target quantile ({p}) wasn't requested in the constructor");
    }

    public void Clear()
    {
        Count = 0;
    }
}
using System;
using Perfolizer.Common;
using Perfolizer.Exceptions;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators;

public class MovingP2QuantileEstimator : ISequentialSpecificQuantileEstimator
{
    private readonly P2QuantileEstimator estimator;
    private readonly int windowSize;
    private int n;
    private double previousWindowEstimation;

    public MovingP2QuantileEstimator(Probability probability, int windowSize)
    {
        Assertion.Positive(nameof(windowSize), windowSize);
        this.windowSize = windowSize;
        estimator = new P2QuantileEstimator(probability);
    }

    public void Add(double value)
    {
        n++;
        if (n % windowSize == 0)
        {
            previousWindowEstimation = estimator.Quantile();
            estimator.Clear();
        }
        estimator.Add(value);
    }

    public double Quantile()
    {
        if (n == 0)
            throw new EmptySequenceException();
        if (n < windowSize)
            return estimator.Quantile();
            
        double estimation1 = previousWindowEstimation;
        double estimation2 = estimator.Quantile();
        double w2 = (n % windowSize + 1) * 1.0 / windowSize;
        double w1 = 1.0 - w2;
        return w1 * estimation1 + w2 * estimation2;
    }
}
using System;
using System.Collections.Generic;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.DispersionEstimators
{
    /// <summary>
    /// The median absolute deviation (MAD).
    /// MAD = ConsistencyFactor * median(abs(x[i] - median(x)))
    /// </summary>
    public abstract class MedianAbsoluteDeviationEstimatorBase : IMedianAbsoluteDeviationEstimator
    {
        protected abstract double GetScaleFactor(Sample sample);

        public abstract IQuantileEstimator QuantileEstimator { get; }

        public double Calc(Sample sample)
        {
            Assertion.NotNull(nameof(sample), sample);
            if (sample.Count == 1)
                return 0;

            double scaleFactor = GetScaleFactor(sample);
            double median = QuantileEstimator.GetMedian(sample);
            double[] deviations = new double[sample.Count];
            for (int i = 0; i < sample.Count; i++)
                deviations[i] = Math.Abs(sample.Values[i] - median);
            return scaleFactor * QuantileEstimator.GetMedian(new Sample(deviations));
        }

        public double CalcLower(Sample sample)
        {
            Assertion.NotNull(nameof(sample), sample);
            if (sample.Count == 1)
                return 0;

            double scaleFactor = GetScaleFactor(sample);
            double median = QuantileEstimator.GetMedian(sample);
            var deviations = new List<double>(sample.Count);
            for (int i = 0; i < sample.Count; i++)
                if (sample.Values[i] <= median)
                    deviations.Add(Math.Abs(sample.Values[i] - median));
            return scaleFactor * QuantileEstimator.GetMedian(new Sample(deviations));
        }

        public double CalcUpper(Sample sample)
        {
            Assertion.NotNull(nameof(sample), sample);
            if (sample.Count == 1)
                return 0;

            double scaleFactor = GetScaleFactor(sample);
            double median = QuantileEstimator.GetMedian(sample);
            var deviations = new List<double>(sample.Count);
            for (int i = 0; i < sample.Count; i++)
                if (sample.Values[i] >= median)
                    deviations.Add(Math.Abs(sample.Values[i] - median));
            return scaleFactor * QuantileEstimator.GetMedian(new Sample(deviations));
        }
    }
}
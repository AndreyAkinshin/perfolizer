using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.DispersionEstimators;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Sequences;

namespace Perfolizer.Mathematics.Functions
{
    public class GammaEffectSizeFunction : QuantileCompareFunction
    {
        [NotNull] private static readonly IMedianAbsoluteDeviationEstimator DefaultMedianAbsoluteDeviationEstimator =
            new SimpleNormalizedMedianAbsoluteDeviationEstimator();

        public static readonly GammaEffectSizeFunction Instance = new GammaEffectSizeFunction();

        private const double Eps = 1e-9;

        [NotNull] private readonly IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator;

        public GammaEffectSizeFunction([CanBeNull] IMedianAbsoluteDeviationEstimator medianAbsoluteDeviationEstimator = null) :
            base((medianAbsoluteDeviationEstimator ?? DefaultMedianAbsoluteDeviationEstimator).QuantileEstimator)
        {
            this.medianAbsoluteDeviationEstimator = medianAbsoluteDeviationEstimator ?? DefaultMedianAbsoluteDeviationEstimator;
        }

        public override double GetValue(Sample a, Sample b, Probability probability)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);

            try
            {
                double aMad = medianAbsoluteDeviationEstimator.Calc(a);
                double bMad = medianAbsoluteDeviationEstimator.Calc(b);
                if (aMad < Eps && bMad < Eps)
                {
                    double aMedian = QuantileEstimator.GetMedian(a);
                    double bMedian = QuantileEstimator.GetMedian(b);
                    if (Math.Abs(aMedian - bMedian) < Eps)
                        return 0;
                    return aMedian < bMedian
                        ? double.PositiveInfinity
                        : double.NegativeInfinity;
                }

                double aQuantile = QuantileEstimator.GetQuantile(a, probability);
                double bQuantile = QuantileEstimator.GetQuantile(b, probability);
                double pooledMad = PooledMad(a.Count, b.Count, aMad, bMad);

                return (bQuantile - aQuantile) / pooledMad;
            }
            catch (Exception)
            {
                return double.NaN;
            }
        }

        public override double[] GetValues(Sample a, Sample b, IReadOnlyList<Probability> probabilities)
        {
            Assertion.NotNull(nameof(a), a);
            Assertion.NotNull(nameof(b), b);
            Assertion.NotNullOrEmpty(nameof(probabilities), probabilities);

            int k = probabilities.Count;
            try
            {
                double aMad = medianAbsoluteDeviationEstimator.Calc(a);
                double bMad = medianAbsoluteDeviationEstimator.Calc(b);
                if (aMad < Eps && bMad < Eps)
                {
                    double aMedian = QuantileEstimator.GetMedian(a);
                    double bMedian = QuantileEstimator.GetMedian(b);
                    if (Math.Abs(aMedian - bMedian) < Eps)
                        return ConstantSequence.Zero.GenerateArray(k);
                    return aMedian < bMedian
                        ? ConstantSequence.PositiveInfinity.GenerateArray(k)
                        : ConstantSequence.NegativeInfinity.GenerateArray(k);
                }

                double[] aQuantile = QuantileEstimator.GetQuantiles(a, probabilities);
                double[] bQuantile = QuantileEstimator.GetQuantiles(b, probabilities);

                double pooledMad = PooledMad(a.Count, b.Count, aMad, bMad);

                double[] values = new double[k];
                for (int i = 0; i < k; i++)
                    values[i] = (bQuantile[i] - aQuantile[i]) / pooledMad;

                return values;
            }
            catch (Exception)
            {
                return ConstantSequence.NaN.GenerateArray(k);
            }
        }

        public static double PooledMad(int n1, int n2, double mad1, double mad2)
        {
            return Math.Sqrt(((n1 - 1) * mad1.Sqr() + (n2 - 1) * mad2.Sqr()) / (n1 + n2 - 2));
        }

        protected override double CalculateValue(double quantileA, double quantileB)
        {
            throw new NotSupportedException();
        }
    }
}
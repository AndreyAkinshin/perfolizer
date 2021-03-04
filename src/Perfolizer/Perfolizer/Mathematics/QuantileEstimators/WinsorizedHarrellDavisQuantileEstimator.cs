using System;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public class WinsorizedHarrellDavisQuantileEstimator : IQuantileEstimator, IQuantileConfidenceIntervalEstimator
    {
        private readonly Probability trimPercent;

        public WinsorizedHarrellDavisQuantileEstimator(Probability trimPercent)
        {
            this.trimPercent = trimPercent;
        }

        public double GetQuantile(Sample sample, Probability probability)
        {
            return GetMoments(sample, probability).C1;
        }

        [NotNull]
        public ConfidenceIntervalEstimator GetQuantileConfidenceIntervalEstimator([NotNull] Sample sample, Probability probability)
        {
            (double c1, double c2) = GetMoments(sample, probability);
            double median = c1;
            double standardError = Math.Sqrt(c2 - c1.Sqr());
            double weightedCount = sample.WeightedCount;
            return new ConfidenceIntervalEstimator(weightedCount, median, standardError);
        }

        public (int left, int right) GetWinsorizedInterval(int n, Probability probability)
        {
            var moments = GetMoments(new Sample(new double[n]), probability);
            return (moments.LeftIndex, moments.RightIndex);
        }

        public bool SupportsWeightedSamples => true;

        private readonly struct Moments
        {
            public readonly double C1;
            public readonly double C2;
            public readonly int LeftIndex;
            public readonly int RightIndex;

            public Moments(double c1, double c2, int leftIndex, int rightIndex)
            {
                C1 = c1;
                C2 = c2;
                LeftIndex = leftIndex;
                RightIndex = rightIndex;
            }

            public void Deconstruct(out double c1, out double c2)
            {
                c1 = C1;
                c2 = C2;
            }
        }

        private Moments GetMoments([NotNull] Sample sample, Probability probability)
        {
            Assertion.NotNull(nameof(sample), sample);

            double n = sample.WeightedCount;
            double a = (n + 1) * probability, b = (n + 1) * (1 - probability);
            var distribution = new BetaDistribution(a, b);
            double targetPercent = 1.0 - trimPercent;
            bool symmetricMode = Math.Abs(probability - 0.5) < 1e-9;

            double c1 = 0;
            double c2 = 0;

            void Process(int j, double w)
            {
                c1 += w * sample.SortedValues[j];
                c2 += w * sample.SortedValues[j].Sqr();
            }

            double GetElementProbability(int j) => sample.IsWeighted
                ? sample.SortedWeights[j] / sample.TotalWeight
                : 1.0 / sample.Count;

            // Preparation
            double probabilityLeft = 0, probabilityRight = 0;
            int indexLeft;
            for (indexLeft = 0; indexLeft < sample.Count; indexLeft++)
            {
                double elementProbability = GetElementProbability(indexLeft);
                probabilityLeft = probabilityRight;
                probabilityRight = probabilityLeft + elementProbability;
                if (probabilityRight >= probability || indexLeft == sample.Count - 1)
                    break;
            }
            int indexRight = indexLeft;
            double betaCdfLeft = distribution.Cdf(probabilityLeft);
            double betaCdfRight = distribution.Cdf(probabilityRight);
            Process(indexLeft, betaCdfRight - betaCdfLeft);
            double betaPdfLeft = distribution.Pdf(probabilityLeft);
            double betaPdfRight = distribution.Pdf(probabilityRight);
            while ((betaCdfRight - betaCdfLeft < targetPercent ||
                    symmetricMode && (indexLeft != sample.Count - 1 - indexRight)) &&
                   (indexLeft > 0 || indexRight < sample.Count - 1))
            {
                if (indexLeft > 0 && (betaPdfLeft > betaPdfRight || indexRight == sample.Count - 1))
                {
                    // Expand to the left
                    int indexNext = indexLeft - 1;
                    double probabilityNext = probabilityLeft - GetElementProbability(indexNext);
                    double betaCdfNext = distribution.Cdf(probabilityNext);
                    double betaPdfNext = distribution.Pdf(probabilityNext);
                    Process(indexNext, betaCdfLeft - betaCdfNext);

                    indexLeft = indexNext;
                    probabilityLeft = probabilityNext;
                    betaCdfLeft = betaCdfNext;
                    betaPdfLeft = betaPdfNext;
                }
                else
                {
                    // Expand to the right
                    int indexNext = indexRight + 1;
                    double probabilityNext = probabilityRight + GetElementProbability(indexNext);
                    double betaCdfNext = distribution.Cdf(probabilityNext);
                    double betaPdfNext = distribution.Pdf(probabilityNext);
                    Process(indexNext, betaCdfNext - betaCdfRight);

                    indexRight = indexNext;
                    probabilityRight = probabilityNext;
                    betaCdfRight = betaCdfNext;
                    betaPdfRight = betaPdfNext;
                }
            }
            
            // Processing winsorized elements
            Process(indexLeft, betaCdfLeft);
            Process(indexRight, 1 - betaCdfRight);

            return new Moments(c1, c2, indexLeft, indexRight);
        }
    }
}
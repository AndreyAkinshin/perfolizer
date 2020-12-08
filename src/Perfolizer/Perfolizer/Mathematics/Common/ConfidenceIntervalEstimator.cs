using Perfolizer.Common;
using Perfolizer.Mathematics.Distributions;

namespace Perfolizer.Mathematics.Common
{
    public class ConfidenceIntervalEstimator
    {
        public double SampleSize { get; }
        public double Estimation { get; }
        public double StandardError { get; }
        private double DegreeOfFreedom => SampleSize - 1;

        public ConfidenceIntervalEstimator(double sampleSize, double estimation, double standardError)
        {
            Assertion.MoreThan(nameof(sampleSize), sampleSize, 1);

            SampleSize = sampleSize;
            Estimation = estimation;
            StandardError = standardError;
        }

        public ConfidenceInterval GetConfidenceInterval(ConfidenceLevel confidenceLevel)
        {
            double margin = StandardError * GetZLevel(confidenceLevel);
            return new ConfidenceInterval(Estimation, Estimation - margin, Estimation + margin, confidenceLevel);
        }

        private double GetZLevel(ConfidenceLevel confidenceLevel)
        {
            double x = 1 - (1 - confidenceLevel) / 2;
            return new StudentDistribution(DegreeOfFreedom).Quantile(x);
        }
    }
}
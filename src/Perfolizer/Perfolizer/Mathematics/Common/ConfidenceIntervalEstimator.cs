using Perfolizer.Common;
using Perfolizer.Mathematics.Distributions;

namespace Perfolizer.Mathematics.Common
{
    public class ConfidenceIntervalEstimator
    {
        public int SampleSize { get; }
        public double Estimation { get; }
        public double StandardError { get; }
        private int DegreeOfFreedom => SampleSize - 1;

        public ConfidenceIntervalEstimator(int sampleSize, double estimation, double standardError)
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

        private double GetZLevel(double confidenceLevel) => StudentDistribution.InverseTwoTailStudent(1 - confidenceLevel, DegreeOfFreedom);
    }
}
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.LocationShiftEstimators
{
    /// <summary>
    /// Based on:
    /// Hodges, J. L., and E. L. Lehmann. 1963. Estimates of location based on rank tests.
    /// The Annals of Mathematical Statistics 34 (2):598–611.  
    /// DOI: 10.1214/aoms/1177704172
    /// </summary>
    public class HodgesLehmannLocationShiftEstimator : ILocationShiftEstimator
    {
        public static readonly ILocationShiftEstimator Instance = new HodgesLehmannLocationShiftEstimator();
        
        public double LocationShift(Sample a, Sample b)
        {
            double[] shifts = new double[a.Count * b.Count];
            int k = 0;
            for (int i = 0; i < a.Count; i++)
            for (int j = 0; j < b.Count; j++)
                shifts[k++] = b.Values[j] - a.Values[i];
            return SimpleQuantileEstimator.Instance.Median(new Sample(shifts));
        }
    }
}
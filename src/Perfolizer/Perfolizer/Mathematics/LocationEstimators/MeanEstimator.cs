using Perfolizer.Metrology;
using Pragmastat;
using Pragmastat.Estimators;
using Pragmastat.Metrology;

namespace Perfolizer.Mathematics.LocationEstimators;

public class MeanEstimator : IOneSampleEstimator
{
    public static readonly MeanEstimator Instance = new();
    public Measurement Estimate(Sample x) => x.Values.Average().WithUnitOf(x);
}
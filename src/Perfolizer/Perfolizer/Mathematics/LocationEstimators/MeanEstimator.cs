using Perfolizer.Mathematics.GenericEstimators;

namespace Perfolizer.Mathematics.LocationEstimators;

public class MeanEstimator : ILocationEstimator
{
    public static readonly MeanEstimator Instance = new();
    public double Location(Sample x) => x.Values.Average();
}
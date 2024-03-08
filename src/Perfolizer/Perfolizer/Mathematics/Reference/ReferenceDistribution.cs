using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.Mathematics.Reference;

public class ReferenceDistribution(string key, string description, IContinuousDistribution distribution)
{
    public string Key { get; } = key;
    public string Description { get; } = description;
    public IContinuousDistribution Distribution { get; } = distribution;
}
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.Mathematics.Reference
{
    public class ReferenceDistribution
    {
        public string Key { get; }
        public string Description { get; }
        public IContinuousDistribution Distribution { get; }

        public ReferenceDistribution(IContinuousDistribution distribution)
        {
            Key = Description = distribution.ToString();
            Distribution = distribution;
        }

        public ReferenceDistribution(string key, IContinuousDistribution distribution)
        {
            Key = key;
            Description = distribution.ToString();
            Distribution = distribution;
        }


        public ReferenceDistribution(string key, string description, IContinuousDistribution distribution)
        {
            Key = key;
            Description = description;
            Distribution = distribution;
        }
    }
}
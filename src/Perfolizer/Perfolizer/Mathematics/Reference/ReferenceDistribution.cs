using Perfolizer.Mathematics.Distributions;

namespace Perfolizer.Mathematics.Reference
{
    public class ReferenceDistribution
    {
        public string Key { get; }
        public string Description { get; }
        public IDistribution Distribution { get; }

        public ReferenceDistribution(IDistribution distribution)
        {
            Key = Description = distribution.ToString();
            Distribution = distribution;
        }

        public ReferenceDistribution(string key, IDistribution distribution)
        {
            Key = key;
            Description = distribution.ToString();
            Distribution = distribution;
        }


        public ReferenceDistribution(string key, string description, IDistribution distribution)
        {
            Key = key;
            Description = description;
            Distribution = distribution;
        }
    }
}
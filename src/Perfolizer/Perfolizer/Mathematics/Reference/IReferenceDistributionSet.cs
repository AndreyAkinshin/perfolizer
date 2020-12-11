namespace Perfolizer.Mathematics.Reference
{
    public interface IReferenceDistributionSet
    {
        public string Key { get; }
        public string Description { get; }
        public ReferenceDistribution[] Distributions { get; }
    }
}
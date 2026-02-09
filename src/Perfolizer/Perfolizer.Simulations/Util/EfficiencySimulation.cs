using Perfolizer.Collections;
using Perfolizer.Common;
using Perfolizer.Mathematics.Randomization;
using Pragmastat;

namespace Perfolizer.Simulations.Util;

public class EfficiencySimulation<TEstimator>(
    Func<TEstimator, Sample, double> estimate,
    int sampleCount = EfficiencySimulation<TEstimator>.DefaultSampleCount)
    where TEstimator : notnull
{
    private const int DefaultSampleCount = 100_000;

    private readonly Dictionary<string, TEstimator> estimators = new();
    private readonly Dictionary<string, RandomGenerator> distributions = new();
    private readonly HashSet<int> sampleSizes = new();

    public EfficiencySimulation<TEstimator> AddEstimator(string name, TEstimator estimator)
    {
        if (!estimators.TryAdd(name, estimator))
            throw new ArgumentException($"Estimator '{name}' is already registered");
        return this;
    }

    public EfficiencySimulation<TEstimator> AddDistribution(string name, RandomGenerator randomGenerator)
    {
        if (!distributions.TryAdd(name, randomGenerator))
            throw new ArgumentException($"Distribution '{name}' is already registered");
        return this;
    }

    public EfficiencySimulation<TEstimator> AddSampleSizes(params int[] sizes)
    {
        foreach (int sampleSize in sizes)
        {
            Assertion.Positive(nameof(sizes), sampleSize);
            sampleSizes.Add(sampleSize);
        }
        return this;
    }

    public class SimulationRow(
        string distribution,
        int sampleSize,
        IReadOnlyDictionary<string, double> relativeEfficiency)
    {
        public string Distribution { get; init; } = distribution;
        public int SampleSize { get; init; } = sampleSize;
        public IReadOnlyDictionary<string, double> RelativeEfficiency { get; init; } = relativeEfficiency;
    }

    public IEnumerable<SimulationRow> Simulate()
    {
        if (distributions.IsEmpty())
            throw new InvalidOperationException("No distributions provided");
        if (estimators.IsEmpty())
            throw new InvalidOperationException("No estimators provided");
        if (sampleSizes.IsEmpty())
            throw new InvalidOperationException("No sample sizes provided");

        foreach ((string distributionName, var randomGenerator) in distributions)
            foreach (int sampleSize in sampleSizes)
            {
                var samplingDistributions = new Dictionary<string, double[]>();
                foreach (string estimatorName in estimators.Keys)
                    samplingDistributions[estimatorName] = new double[sampleCount];

                for (int i = 0; i < sampleCount; i++)
                {
                    var sample = new Sample(randomGenerator.Next(sampleSize));
                    foreach ((string estimatorName, var estimator) in estimators)
                        samplingDistributions[estimatorName][i] = estimate(estimator, sample);
                }

                var mses = new Dictionary<string, double>();
                foreach (string estimatorName in estimators.Keys)
                    mses[estimatorName] = Mse(samplingDistributions[estimatorName]);
                double minMse = mses.Values.Min();
                var relativeEfficiency = new Dictionary<string, double>();
                foreach (string estimatorName in estimators.Keys)
                    relativeEfficiency[estimatorName] = minMse / mses[estimatorName];

                var row = new SimulationRow(distributionName, sampleSize, relativeEfficiency);
                yield return row;
            }
    }

    // The mean squared error (MSE)
    private static double Mse(double[] values)
    {
        double mean = values.Average();
        return values.Sum(x => (x - mean) * (x - mean)) / values.Length;
    }
}
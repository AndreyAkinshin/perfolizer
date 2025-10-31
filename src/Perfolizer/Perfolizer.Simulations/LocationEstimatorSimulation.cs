using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.LocationEstimators;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Simulations.Util;
using Pragmastat.Estimators;

namespace Perfolizer.Simulations;

public class LocationEstimatorSimulation
{
    public void Run(string[] args)
    {
        var simulation =
            new EfficiencySimulation<IOneSampleEstimator>((estimator, sample) => estimator.Estimate(sample))
                .AddEstimator("Mean", MeanEstimator.Instance)
                .AddEstimator("Median", SimpleQuantileEstimator.Instance.ToLocationEstimator())
                .AddEstimator("Center", CenterEstimator.Instance)
                .AddDistribution("Normal", NormalDistribution.Standard.Random(1729))
                .AddSampleSizes(Enumerable.Range(2, 99).ToArray());

        var rows = simulation.Simulate();

        foreach (var row in rows)
        {
            Console.WriteLine(row.Distribution + " / N = " + row.SampleSize);
            foreach ((string key, double value) in row.RelativeEfficiency)
                Console.WriteLine("  " + key.PadRight(6) + " : " + Math.Round(value * 100, 1) + "%");
            Console.WriteLine();
        }
    }
}
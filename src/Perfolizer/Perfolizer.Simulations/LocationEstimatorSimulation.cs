using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Mathematics.LocationEstimators;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Simulations.Util;

namespace Perfolizer.Simulations;

public class LocationEstimatorSimulation
{
    public void Run(string[] args)
    {
        var simulation =
            new EfficiencySimulation<ILocationEstimator>((estimator, sample) => estimator.Location(sample))
                .AddEstimator("Mean", MeanEstimator.Instance)
                .AddEstimator("Median", SimpleQuantileEstimator.Instance.ToLocationEstimator())
                .AddEstimator("HL", HodgesLehmannEstimator.Instance)
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
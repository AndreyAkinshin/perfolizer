using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Pragmastat;

namespace Perfolizer.Demo;

public class QuantileEstimatorDemo : IDemo
{
    public void Run()
    {
        // There are multiple ways to estimate quantile values
        // Let's consider several popular quantile estimators
        var estimators = new[]
        {
            // The classic traditional quantile estimator also known as Hydman-Fan Type 7 quantile estimator
            // It uses 1 or 2 sample elements
            // * Hyndman, Rob J., and Yanan Fan. "Sample quantiles in statistical packages."
            //   The American Statistician 50, no. 4 (1996): 361-365.
            //   https://doi.org/10.2307/2684934
            SimpleQuantileEstimator.Instance,
            // The Harrell-Davis quantile estimator
            // It uses a weighted sum of all sample elements, weights are assigned according to the Beta distribution
            // * Harrell, Frank E., and C. E. Davis. "A new distribution-free quantile estimator." Biometrika 69, no. 3 (1982): 635-640.
            //   https://doi.org/10.1093/biomet/69.3.635
            HarrellDavisQuantileEstimator.Instance,
            // The Trimmed Harrell-Davis quantile estimator based on the highest density interval of size 1/sqrt(sampleSize)
            // It uses a weighted sum of sqrt(sampleSize) sample elements, weights are assigned according to the Beta distribution
            // * Akinshin, Andrey. "Trimmed Harrell-Davis quantile estimator based on the highest density interval of the given width."
            //   arXiv preprint arXiv:2111.11776 (2021).
            //   https://arxiv.org/abs/2111.11776
            TrimmedHarrellDavisQuantileEstimator.Sqrt,
            // The Sfakianakis-Verginis quantile estimators
            // They use a weighted sum of all sample elements, weights are assigned according to the Binomial distribution
            // * Sfakianakis, Michael E., and Dimitris G. Verginis. "A new family of nonparametric quantile estimators."
            //   Communications in Statistics—Simulation and Computation® 37, no. 2 (2008): 337-345.
            //   https://doi.org/10.1080/03610910701790491
            // * Akinshin, Andrey. "Sfakianakis-Verginis quantile estimator."
            //   https://aakinshin.net/posts/sfakianakis-verginis-quantile-estimator/
            SfakianakisVerginis1QuantileEstimator.Instance,
            SfakianakisVerginis2QuantileEstimator.Instance,
            SfakianakisVerginis3QuantileEstimator.Instance,
            // The Navruz-Özdemir quantile estimator
            // It uses a weighted sum of all sample elements, weights are assigned according to the Binomial distribution
            // * Navruz, Gözde, and A. Fırat Özdemir. "A new quantile estimator with weights based on a subsampling approach."
            //   British Journal of Mathematical and Statistical Psychology 73, no. 3 (2020): 506-521.
            //   https://doi.org/10.1111/bmsp.12198
            // * Akinshin, Andrey. "Navruz-Özdemir quantile estimator."
            //   https://aakinshin.net/posts/navruz-ozdemir-quantile-estimator/
            NavruzOzdemirQuantileEstimator.Instance
        };

        // ********************************************************************************
        Console.WriteLine("*** Statistical efficiency ***");
        // First of all, let's consider their statistical efficiency
        // In order to do it, we generate 10_000 random samples of size 10 from the standard normal distribution
        // For each sample, we estimate the median using different quantile estimators
        var randomGenerator = NormalDistribution.Standard.Random(new Random(42));
        var medians = new Dictionary<IQuantileEstimator, List<double>>();
        foreach (var estimator in estimators)
            medians[estimator] = new List<double>();
        for (int i = 0; i < 10_000; i++)
        {
            var sample = new Sample(randomGenerator.Next(10));
            foreach (var estimator in estimators)
                medians[estimator].Add(estimator.Median(sample));
        }

        // Next, we calculate the mean squared error (MSE) for each estimator
        // The lower values of MSE are better
        double Mse(List<double> values)
        {
            var mean = values.Average();
            return values.Sum(x => (x - mean) * (x - mean)) / values.Count;
        }

        foreach (var estimator in estimators)
            Console.WriteLine($"{estimator.Alias,-12}: MSE = {Mse(medians[estimator])}");
        Console.WriteLine("As we can see, HD, THD-SQRT, SV1, SV2, SV3, NO are more statistically efficient than the classic HF7");
        Console.WriteLine();

        // ********************************************************************************
        Console.WriteLine("*** Robustness ***");
        // Now let's look at the estimator robustness
        // We take a sample with 8 small elements and a single outlier
        var sampleWithOutlier = new Sample(new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1_000_000 });
        // Now we estimate the median using each estimator
        foreach (var estimator in estimators)
            Console.WriteLine($"{estimator.Alias,-8}: Median = {estimator.Median(sampleWithOutlier)}");
        Console.WriteLine("As we can see, only HF7 and THD-SQRT are robust enough");
    }
}
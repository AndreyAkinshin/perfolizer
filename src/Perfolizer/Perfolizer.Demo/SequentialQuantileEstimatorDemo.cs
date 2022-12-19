using System;
using System.Collections.Generic;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Demo
{
    public class SequentialQuantileEstimatorDemo : IDemo
    {
        public void Run()
        {
            // Sequential quantile estimators allow estimating request quantiles without storing all the elements
            // For this demo, we are going to estimate the median (p = 0.5)
            Probability probability = 0.5;
            // Here is the list of the used sequential quantile estimators
            var estimators = new ISequentialSpecificQuantileEstimator[]
            {
                // The P2 quantile estimator
                // * Jain, Raj, and Imrich Chlamtac. "The P2 algorithm for dynamic calculation of quantiles and histograms without storing observations."
                //   Communications of the ACM 28, no. 10 (1985): 1076-1085.
                //   https://doi.org/10.1145/4372.4378
                // * Akinshin, Andrey. "P² quantile estimator: estimating the median without storing values."
                //   https://aakinshin.net/posts/p2-quantile-estimator/
                new P2QuantileEstimator(probability),
                // The Greenwald-Khanna quantile estimator
                // * Greenwald, Michael, and Sanjeev Khanna. “Space-efficient online computation of quantile summaries.”
                //   ACM SIGMOD Record 30, no. 2 (2001): 58-66.
                //   https://doi.org/10.1145/375663.375670
                // * Akinshin, Andrey. "Greenwald-Khanna quantile estimator."
                //   https://aakinshin.net/posts/greenwald-khanna-quantile-estimator/
                new GreenwaldKhannaSpecificQuantileEstimator(probability, 0.05),
                // The partitioning heaps moving quantile estimator based on the Hardle-Steiger method with windowSize = 100
                // * Hardle, W., and William Steiger. “Algorithm AS 296: Optimal median smoothing.”
                //   Journal of the Royal Statistical Society. Series C (Applied Statistics) 44, no. 2 (1995): 258-264.
                //   https://doi.org/10.2307/2986349
                // * Akinshin, Andrey. "Fast implementation of the moving quantile based on the partitioning heaps."
                //   https://aakinshin.net/posts/partitioning-heaps-quantile-estimator/
                // * Akinshin, Andrey. "Better moving quantile estimations using the partitioning heaps."
                //   https://aakinshin.net/posts/partitioning-heaps-quantile-estimator2/
                new PartitioningHeapsMovingQuantileEstimator(probability, 100),
                // The Moving P2 quantile estimator with windowSize = 100
                // * Akinshin, Andrey. "MP² quantile estimator: estimating the moving median without storing values."
                //   https://aakinshin.net/posts/mp2-quantile-estimator/
                new MovingP2QuantileEstimator(probability, 100)
            };

            void AddValues(IEnumerable<double> values)
            {
                foreach (double value in values)
                foreach (var estimator in estimators)
                    estimator.Add(value);
            }

            void PrintEstimations()
            {
                foreach (var estimator in estimators)
                {
                    string title = estimator.GetType().Name.Replace("QuantileEstimator", "");
                    Console.WriteLine($"{title,-24}: {estimator.Quantile()}");
                }
            }

            var random = new Random(42);

            Console.WriteLine("We add 100 random numbers from the Normal distribution with mean = 10");
            AddValues(new NormalDistribution(10).Random(random).Next(100));
            PrintEstimations();
            Console.WriteLine("As we can see, all estimators return a value around 10");
            Console.WriteLine();

            Console.WriteLine("Next, we add 100 random numbers from the Normal distribution with mean = 30");
            AddValues(new NormalDistribution(30).Random(random).Next(100));
            PrintEstimations();
            Console.WriteLine("P2 and GreenwaldKhanna return values around 15 because");
            Console.WriteLine("  they are based on the whole set of numbers");
            Console.WriteLine("PartitioningHeapsMoving and MovingP2 return values around 30 because");
            Console.WriteLine("  they are based only on the last 100 numbers");
        }
    }
}
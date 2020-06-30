using System.Collections.Generic;
using System.Linq;
using Perfolizer.Collections;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public class SimpleQuantileEstimator : IQuantileEstimator, IWeightedQuantileEstimator
    {
        public static readonly IQuantileEstimator Instance = new SimpleQuantileEstimator();

        /// <summary>
        /// Calculates the requested quantile from the set of values
        /// </summary>
        /// <remarks>
        /// The implementation is expected to be consistent with the one from Excel.
        /// It's a quite common to export bench output into .csv for further analysis
        /// And it's a good idea to have same results from all tools being used.
        /// </remarks>
        /// <param name="data">Sequence of the values to be calculated</param>
        /// <param name="probability">Value in range [0;1]</param>
        /// <returns>Quantile from the set of values</returns>
        // Based on: http://stackoverflow.com/a/8137526
        public double GetQuantile(ISortedReadOnlyList<double> data, double probability)
        {
            QuantileEstimatorHelper.CheckArguments(data, probability);

            // DONTTOUCH: the following code was taken from http://stackoverflow.com/a/8137526 and it is proven
            // to work in the same way the excel's counterpart does.
            // So it's better to leave it as it is unless you do not want to reimplement it from scratch:)
            double realIndex = probability * (data.Count - 1);
            int index = (int) realIndex;
            double frac = realIndex - index;
            if (index + 1 < data.Count)
                return data[index] * (1 - frac) + data[index + 1] * frac;
            return data[index];
        }

        public double GetWeightedQuantile(ISortedReadOnlyList<double> data, IReadOnlyList<double> weights, double probability)
        {
            QuantileEstimatorHelper.CheckWeightedArguments(data, weights, probability);

            int n = data.Count;
            double leftBorder = probability * (n - 1) / n;
            double rightBorder = leftBorder + 1.0 / n;

            double Cdf(double x)
            {
                if (x <= leftBorder)
                    return 0;
                if (x >= rightBorder)
                    return 1;
                return (x - leftBorder) / (rightBorder - leftBorder);
            }
            
            double totalWeight = weights.Sum();
            double result = 0;
            double current = 0;
            for (int i = 0; i < n; i++)
            {
                double next = current + weights[i] / totalWeight;
                result += data[i] * (Cdf(next) - Cdf(current));
                current = next;
            }

            return result;
        }
    }
}
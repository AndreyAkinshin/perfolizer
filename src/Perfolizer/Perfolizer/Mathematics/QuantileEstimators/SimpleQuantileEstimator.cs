using Perfolizer.Collections;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public class SimpleQuantileEstimator : IQuantileEstimator
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
            int index = (int)realIndex;
            double frac = realIndex - index;
            if (index + 1 < data.Count)
                return data[index] * (1 - frac) + data[index + 1] * frac;
            return data[index];
        }
    }
}
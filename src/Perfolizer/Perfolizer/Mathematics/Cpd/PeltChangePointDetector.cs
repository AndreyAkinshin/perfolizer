using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Cpd
{
    public abstract class PeltChangePointDetector
    {
        public interface ICostCalculator
        {
            double Penalty { get; }

            /// <summary>
            /// Calculates the cost of the (tau1; tau2] segment.
            /// Remember that tau are one-based indexes.
            /// </summary>
            double GetCost(int tau0, int tau1, int tau2);
        }

        [NotNull]
        public abstract ICostCalculator CreateCostCalculator([NotNull] double[] data);

        /// <summary>
        /// For given array of `double` values, detects locations of changepoints that
        /// splits original series of values into "statistically homogeneous" segments.
        /// Such points correspond to moments when statistical properties of the distribution are changing.
        ///
        /// This method supports nonparametric distributions and has O(N*log(N)) algorithmic complexity.
        /// </summary>
        /// <param name="data">An array of double values</param>
        /// <param name="minDistance">Minimum distance between changepoints</param>
        /// <returns>
        /// Returns an `int[]` array with 0-based indexes of changepoint.
        /// Changepoints correspond to the end of the detected segments.
        /// For example, changepoints for { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2 } are { 5, 11 }.
        /// </returns>
        [NotNull]
        public int[] GetChangePointIndexes([NotNull] double[] data, int minDistance = 20)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // We will use `n` as the number of elements in the `data` array
            int n = data.Length;

            // Checking corner cases
            if (n <= 2 || 2 * minDistance > n)
                return new int[0];
            if (minDistance < 1)
                throw new ArgumentOutOfRangeException(nameof(minDistance), $"{nameof(minDistance)} ({minDistance}) should be positive");

            var costCalculator = CreateCostCalculator(data);
            double penalty = costCalculator.Penalty;
            double Cost(int tau0, int tau1, int tau2) => costCalculator.GetCost(tau0, tau1, tau2);

            // We will use dynamic programming to find the best solution; `bestCost` is the cost array.
            // `bestCost[i]` is the cost for subarray `data[0..i-1]`.
            // It's a 1-based array (`data[0]`..`data[n-1]` correspond to `bestCost[1]`..`bestCost[n]`)
            var bestCost = new double[n + 1];
            bestCost[0] = -penalty;
            for (int currentTau = minDistance; currentTau < 2 * minDistance; currentTau++)
                bestCost[currentTau] = Cost(0, 0, currentTau);

            // `previousChangePointIndex` is an array of references to previous changepoints. If the current segment ends at
            // the position `i`, the previous segment ends at the position `previousChangePointIndex[i]`. It's a 1-based
            // array (`data[0]`..`data[n-1]` correspond to the `previousChangePointIndex[1]`..`previousChangePointIndex[n]`)
            var previousChangePointIndex = new int[n + 1];

            // We use PELT (Pruned Exact Linear Time) approach which means that instead of enumerating all possible previous
            // tau values, we use a whitelist of "good" tau values that can be used in the optimal solution. If we are 100%
            // sure that some of the tau values will not help us to form the optimal solution, such values should be
            // removed. See [Killick2012] for details.
            var previousTaus = new int[n + 1]; // The maximum number of the previous tau values is n + 1
            previousTaus[0] = 0;
            previousTaus[1] = minDistance;
            var costForPreviousTau = new double[n + 1];
            int previousTausCount = 2; // The counter of previous tau values. Defines the size of `previousTaus` and `costForPreviousTau`.

            // Following the dynamic programming approach, we enumerate all tau positions. For each `currentTau`, we pretend
            // that it's the end of the last segment and trying to find the end of the previous segment.
            for (int currentTau = 2 * minDistance; currentTau < n + 1; currentTau++)
            {
                // For each previous tau, we should calculate the cost of taking this tau as the end of the previous
                // segment. This cost equals the cost for the `previousTau` plus cost of the new segment (from `previousTau`
                // to `currentTau`) plus penalty for the new changepoint.
                for (int i = 0; i < previousTausCount; i++)
                {
                    int previousTau = previousTaus[i];
                    costForPreviousTau[i] = bestCost[previousTau] +
                                            Cost(previousChangePointIndex[previousTau], previousTau, currentTau) +
                                            penalty;
                }

                // Now we should choose the tau that provides the minimum possible cost.
                int bestPreviousTauIndex = WhichMin(costForPreviousTau, previousTausCount);
                bestCost[currentTau] = costForPreviousTau[bestPreviousTauIndex];
                previousChangePointIndex[currentTau] = previousTaus[bestPreviousTauIndex];

                // Prune phase: we remove "useless" tau values that will not help to achieve minimum cost in the future
                double currentBestCost = bestCost[currentTau];
                int newPreviousTausCount = 0;
                for (int i = 0; i < previousTausCount; i++)
                    if (costForPreviousTau[i] < currentBestCost + penalty)
                        previousTaus[newPreviousTausCount++] = previousTaus[i];

                // We add a new tau value that is located on the `minDistance` distance from the next `currentTau` value
                previousTaus[newPreviousTausCount] = currentTau - minDistance + 1;
                previousTausCount = newPreviousTausCount + 1;
            }

            // Here we collect the result list of changepoint indexes `changePointIndexes` using `previousChangePointIndex`
            var changePointIndexes = new List<int>();
            int currentIndex = previousChangePointIndex[n]; // The index of the end of the last segment is `n`
            while (currentIndex != 0)
            {
                changePointIndexes.Add(currentIndex - 1); // 1-based indexes should be be transformed to 0-based indexes
                currentIndex = previousChangePointIndex[currentIndex];
            }

            changePointIndexes.Reverse(); // The result changepoints should be sorted in ascending order.
            return changePointIndexes.ToArray();
        }

        /// <summary>
        /// Returns the index of the minimum element in the given range.
        /// </summary>
        /// <param name="source">An array of <see cref="T:System.Double"></see> values to determine the minimum element of</param>
        /// <param name="length">The actual number of values that will be used for search
        /// (only values form the 0..(length-1) will be used)</param>
        /// <returns>The index of the minimum element in range 0..(length-1)</returns>
        /// <exception cref="InvalidOperationException"><paramref name="source">source</paramref> contains no elements</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length">length</paramref> is not positive or less than
        /// <paramref name="source">source</paramref>.Length</exception>
        private static int WhichMin([NotNull] double[] source, int length)
        {
            if (source.Length == 0)
                throw new InvalidOperationException($"{nameof(source)} should contain elements");
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, $"{nameof(length)} should be positive");
            if (length > source.Length)
                throw new ArgumentOutOfRangeException(nameof(length), length,
                    $"{nameof(length)} should be greater or equal to {nameof(source)}.Length");

            double minValue = source[0];
            int minIndex = 0;
            for (int i = 1; i < length; i++)
                if (source[i] < minValue)
                {
                    minValue = source[i];
                    minIndex = i;
                }

            return minIndex;
        }
    }
}
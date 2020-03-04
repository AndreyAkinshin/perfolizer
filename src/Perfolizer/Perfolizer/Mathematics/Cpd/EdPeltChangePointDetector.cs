using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Cpd
{
    /// <summary>
    /// The ED-PELT algorithm for changepoint detection.
    ///
    /// <remarks>
    /// The implementation is based on the following papers:
    /// <list type="bullet">
    /// <item>
    /// <b>[Haynes2017]</b> Haynes, Kaylea, Paul Fearnhead, and Idris A. Eckley.
    /// "A computationally efficient nonparametric approach for changepoint detection."
    /// Statistics and Computing 27, no. 5 (2017): 1293-1305.
    /// https://doi.org/10.1007/s11222-016-9687-5
    /// </item>
    /// <item>
    /// <b>[Killick2012]</b> Killick, Rebecca, Paul Fearnhead, and Idris A. Eckley.
    /// "Optimal detection of changepoints with a linear computational cost."
    /// Journal of the American Statistical Association 107, no. 500 (2012): 1590-1598.
    /// https://arxiv.org/pdf/1101.1438.pdf
    /// </item>
    /// </list>
    /// </remarks>
    /// </summary>
    public class EdPeltChangePointDetector : PeltChangePointDetector
    {
        public static readonly EdPeltChangePointDetector Instance = new EdPeltChangePointDetector();

        public class CostCalculator : ICostCalculator
        {
            private readonly int n, k;
            private readonly int[] partialSums;

            public CostCalculator([NotNull] double[] data)
            {
                n = data.Length;

                // The penalty which we add to the final cost for each additional changepoint
                // Here we use the Modified Bayesian Information Criterion
                Penalty = 3 * Math.Log(n);

                // `k` is the number of quantiles that we use to approximate an integral during the segment cost evaluation
                // We use `k=Ceiling(4*log(n))` as suggested in the Section 4.3 "Choice of K in ED-PELT" in [Haynes2017]
                // `k` can't be greater than `n`, so we should always use the `Min` function here (important for n <= 8)
                k = Math.Min(n, (int) Math.Ceiling(4 * Math.Log(n)));

                // We should precalculate sums for empirical CDF, it will allow fast evaluating of the segment cost
                partialSums = GetPartialSums(data, k);
            }

            public double Penalty { get; }

            /// <summary>
            /// Partial sums for empirical CDF (formula (2.1) from Section 2.1 "Model" in [Haynes2017])
            /// <code>
            /// partialSums'[i, tau] = (count(data[j] &lt; t) * 2 + count(data[j] == t) * 1) for j=0..tau-1
            /// where t is the i-th quantile value (see Section 3.1 "Discrete approximation" in [Haynes2017] for details)
            /// </code>
            /// In order to get better performance, we present
            /// a two-dimensional array <c>partialSums'[k, n + 1]</c> as a single-dimensional array <c>partialSums[k * (n + 1)]</c>.
            /// We assume that <c>partialSums'[i, tau] = partialSums[i * (n + 1) + tau]</c>
            /// <remarks>
            /// <list type="bullet">
            /// <item>
            /// We use doubled sum values in order to use <c>int[,]</c> instead of <c>double[,]</c> (it provides noticeable
            /// performance boost). Thus, multipliers for <c>count(data[j] &lt; t)</c> and <c>count(data[j] == t)</c> are
            /// 2 and 1 instead of 1 and 0.5 from the [Haynes2017].
            /// </item>
            /// <item>
            /// Note that these quantiles are not uniformly distributed: tails of the <c>data</c> distribution contain more
            /// quantile values than the center of the distribution
            /// </item>
            /// </list>
            /// </remarks>
            /// </summary>
            [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
            [NotNull]
            private static int[] GetPartialSums([NotNull] double[] data, int k)
            {
                int n = data.Length;
                var partialSums = new int[k * (n + 1)];
                var sortedData = new double[data.Length];
                Array.Copy(data, sortedData, data.Length);
                Array.Sort(sortedData);

                int offset = 0;
                for (int i = 0; i < k; i++)
                {
                    double z = -1 + (2 * i + 1.0) / k; // Values from (-1+1/k) to (1-1/k) with step = 2/k
                    double p = 1.0 / (1 + Math.Pow(2 * n - 1, -z)); // Values from 0.0 to 1.0
                    double t = sortedData[(int) Math.Truncate((n - 1) * p)]; // Quantile value, formula (2.1) in [Haynes2017]

                    for (int tau = 1; tau <= n; tau++)
                    {
                        // `currentPartialSumsValue` is a temp variable to keep the future value of `partialSums[offset + tau]`
                        // (or `partialSums'[i, tau]`)
                        int currentPartialSumsValue = partialSums[offset + tau - 1];
                        if (data[tau - 1] < t)
                            currentPartialSumsValue += 2; // We use doubled value (2) instead of original 1.0
                        if (data[tau - 1] == t)
                            currentPartialSumsValue += 1; // We use doubled value (1) instead of original 0.5

                        partialSums[offset + tau] = currentPartialSumsValue;
                    }

                    offset += n + 1;
                }

                return partialSums;
            }

            public double GetCost(int tau0, int tau1, int tau2)
            {
                double sum = 0;
                int offset = tau1; // offset of partialSums'[i, tau1] in the single-dimenstional `partialSums` array
                int tauDiff = tau2 - tau1;
                for (int i = 0; i < k; i++)
                {
                    // actualSum is (count(data[j] < t) * 2 + count(data[j] == t) * 1) for j=tau1..tau2-1
                    int actualSum =
                        partialSums[offset + tauDiff] -
                        partialSums[offset]; // partialSums'[i, tau2] - partialSums'[i, tau1]

                    // We skip these two cases (correspond to fit = 0 or fit = 1) because of invalid Math.Log values
                    if (actualSum != 0 && actualSum != tauDiff * 2)
                    {
                        // Empirical CDF $\hat{F}_i(t)$ (Section 2.1 "Model" in [Haynes2017])
                        double fit = actualSum * 0.5 / tauDiff;

                        // Segment cost $\mathcal{L}_{np}$ (Section 2.2 "Nonparametric maximum likelihood" in [Haynes2017])
                        double lnp = tauDiff * (fit * Math.Log(fit) + (1 - fit) * Math.Log(1 - fit));
                        sum += lnp;
                    }

                    offset += n + 1;
                }

                double c = -Math.Log(2 * n - 1); // Constant from Lemma 3.1 in [Haynes2017]
                return 2.0 * c / k * sum; // See Section 3.1 "Discrete approximation" in [Haynes2017]
            }
        }

        public override ICostCalculator CreateCostCalculator(double[] data) => new CostCalculator(data);
    }
}
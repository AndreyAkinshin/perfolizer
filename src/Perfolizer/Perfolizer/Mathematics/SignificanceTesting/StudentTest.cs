using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.SignificanceTesting.Base;

namespace Perfolizer.Mathematics.SignificanceTesting;

public class StudentTest : ISignificanceOneSampleTest<StudentTOneSampleResult>
{
    public static readonly StudentTest Instance = new();

    public StudentTOneSampleResult Run(Sample x, double y, AlternativeHypothesis alternativeHypothesis)
    {
        Assertion.NonWeighted(nameof(x), x);

        var moments = Moments.Create(x);
        double mean = moments.Mean;
        double stdDev = moments.StandardDeviation;
        double n = x.Size;
        double df = n - 1;
        double t = (mean - y) / (stdDev / Sqrt(n));
        double cdf = new StudentDistribution(df).Cdf(t);
        double pValue = SignificanceTestHelper.CdfToPValue(cdf, alternativeHypothesis);

        return new StudentTOneSampleResult(x, y, alternativeHypothesis, pValue, t, df);
    }
}
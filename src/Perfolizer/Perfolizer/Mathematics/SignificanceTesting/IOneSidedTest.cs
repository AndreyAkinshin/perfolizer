using JetBrains.Annotations;
using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting
{
    public interface IOneSidedTest<out T> where T : OneSidedTestResult
    {
        [CanBeNull]
        T IsGreater([NotNull] double[] x, [NotNull] double[] y, Threshold threshold = null);
    }
}
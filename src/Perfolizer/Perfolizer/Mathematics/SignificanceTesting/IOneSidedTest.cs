using Perfolizer.Mathematics.Thresholds;

namespace Perfolizer.Mathematics.SignificanceTesting
{
    public interface IOneSidedTest<out T> where T : OneSidedTestResult
    {
        T? IsGreater(double[] x, double[] y, Threshold? threshold = null);
    }
}
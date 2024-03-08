using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Metrology;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public interface ITostEquivalenceTest<T> : IEquivalenceTest where T : SignificanceTwoSampleResult
{
    TostEquivalenceResult<T> Perform(Sample x, Sample y, Threshold threshold, SignificanceLevel alpha);
}
using Perfolizer.Mathematics.Common;
using Perfolizer.Metrology;
using Pragmastat;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public interface IEquivalenceTest
{
    ComparisonResult Perform(Sample x, Sample y, Threshold threshold, SignificanceLevel alpha);
}
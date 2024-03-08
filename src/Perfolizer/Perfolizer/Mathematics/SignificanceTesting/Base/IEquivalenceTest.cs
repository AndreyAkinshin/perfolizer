using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Metrology;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

public interface IEquivalenceTest
{
    bool AreEquivalent(Sample x, Sample y, Threshold threshold, SignificanceLevel alpha);
}
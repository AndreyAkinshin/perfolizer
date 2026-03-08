using Perfolizer.Mathematics.Common;
using Perfolizer.Metrology;
using Pragmastat;
using Threshold = Perfolizer.Metrology.Threshold;

namespace Perfolizer.Mathematics.SignificanceTesting.Base;

[Obsolete("Use Pragmastat.Toolkit.Compare2 instead.")]
public interface IEquivalenceTest
{
    ComparisonResult Perform(Sample x, Sample y, Threshold threshold, SignificanceLevel alpha);
}
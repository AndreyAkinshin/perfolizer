using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.OutlierDetection
{
    public interface IOutlierDetectorFactory
    {
        IOutlierDetector Create([NotNull] IReadOnlyList<double> values);
    }
}
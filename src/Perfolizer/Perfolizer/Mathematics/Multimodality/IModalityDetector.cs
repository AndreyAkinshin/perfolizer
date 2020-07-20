using System.Collections.Generic;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Multimodality
{
    public interface IModalityDetector
    {
        [NotNull, Pure]
        ModalityData DetectModes([NotNull] IReadOnlyList<double> values);
    }
}
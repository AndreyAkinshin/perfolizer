using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Multimodality
{
    public static class ModalityDetectorExtensions
    {
        [NotNull]
        public static ModalityData DetectModes([NotNull] this IModalityDetector modalityDetector, [NotNull] IReadOnlyList<double> values)
        {
            Assertion.NotNull(nameof(modalityDetector), modalityDetector);
            Assertion.NotNull(nameof(values), values);

            return modalityDetector.DetectModes(values.ToSample());
        }
    }
}
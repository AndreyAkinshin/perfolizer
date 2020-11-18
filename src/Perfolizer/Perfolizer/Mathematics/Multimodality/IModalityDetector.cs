using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Multimodality
{
    public interface IModalityDetector
    {
        [NotNull, Pure]
        ModalityData DetectModes([NotNull] Sample sample);
    }
}
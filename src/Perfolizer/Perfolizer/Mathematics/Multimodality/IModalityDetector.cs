using JetBrains.Annotations;
using Pragmastat;

namespace Perfolizer.Mathematics.Multimodality;

public interface IModalityDetector
{
    [Pure]
    ModalityData DetectModes(Sample sample);
}
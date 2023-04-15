using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Multimodality;

public interface IModalityDetector
{
    [Pure]
    ModalityData DetectModes(Sample sample);
}
using System;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Multimodality
{
    public interface IModalityDataFormatter
    {
        string Format([NotNull] ModalityData data, [CanBeNull] string numberFormat = null,
            [CanBeNull] IFormatProvider numberFormatProvider = null);
    }
}
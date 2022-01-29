using System;

namespace Perfolizer.Mathematics.Multimodality
{
    public class AutomaticModalityDataFormatter : IModalityDataFormatter
    {
        public static readonly IModalityDataFormatter Instance = new AutomaticModalityDataFormatter();

        public string Format(ModalityData data, string? numberFormat = null, IFormatProvider? numberFormatProvider = null)
        {
            int modality = data.Modality;
            var formatter = new ManualModalityDataFormatter
            {
                PresentCount = modality <= 2,
                PresentModeLocations = modality <= 1,
                PresentOutliers = true,
                CompactMiddleModes = true
            };
            return formatter.Format(data, numberFormat, numberFormatProvider);
        }
    }
}
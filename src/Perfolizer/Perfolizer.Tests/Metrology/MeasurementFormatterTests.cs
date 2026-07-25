using System.Globalization;
using Perfolizer.Horology;
using Perfolizer.Metrology;

namespace Perfolizer.Tests.Metrology;

public class MeasurementFormatterTests
{
    /// <summary>
    /// Formatting must not depend on the ambient culture: callers that do not pass a format provider
    /// (e.g. TimeInterval.ToString(), Frequency.ToString(), CpuBrandHelper) always produce '.' as the
    /// decimal separator, so that reports stay identical across machines.
    /// </summary>
    [Theory]
    [InlineData("ru-RU")]
    [InlineData("de-DE")]
    [InlineData("en-US")]
    public void FormatIsCultureIndependentTest(string cultureName)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(cultureName);

            var measurement = Frequency.FromGHz(3.1).ToMeasurement(FrequencyUnit.GHz);
            Assert.Equal("3.10GHz", MeasurementFormatter.Default.Format(measurement, "N2"));
            Assert.Equal("1.234us", TimeInterval.FromNanoseconds(1234).ToString());
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }
}

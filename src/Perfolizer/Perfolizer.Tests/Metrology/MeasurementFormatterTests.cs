using System.Globalization;
using Perfolizer.Horology;
using Perfolizer.Metrology;
using Perfolizer.Tests.Infra;
using Pragmastat.Metrology;

namespace Perfolizer.Tests.Metrology;

/// <summary>
/// MeasurementFormatter is the codec behind Threshold parsing, sample parsing, and every
/// "value+unit" string that travels through configs and command lines.
/// These tests pin down the current wire format: they describe what it is, not what it should be.
/// </summary>
public class MeasurementFormatterTests
{
    [Theory]
    // Dimensionless
    [InlineData("1")]
    // Time
    [InlineData("2ns")]
    [InlineData("3.5us")]
    [InlineData("4ms")]
    [InlineData("5s")]
    [InlineData("6m")]
    [InlineData("7h")]
    [InlineData("8d")]
    // Size
    [InlineData("9B")]
    [InlineData("10KB")]
    [InlineData("11MB")]
    [InlineData("12GB")]
    [InlineData("13TB")]
    // Percent
    [InlineData("14%")]
    // Frequency
    [InlineData("17Hz")]
    [InlineData("18KHz")]
    [InlineData("19MHz")]
    [InlineData("20GHz")]
    public void RoundTripTest(string s)
    {
        var formatter = MeasurementFormatter.Default;
        Assert.True(formatter.TryParse(s, out var measurement), $"Failed to parse '{s}'");
        Assert.Equal(s, formatter.Format(measurement));
    }

    /// <summary>
    /// The codec writes the value and the abbreviation with no separator, but accepts both forms on input.
    /// </summary>
    [Theory]
    [InlineData("3.5us")]
    [InlineData("3.5 us")]
    public void ParseAcceptsOptionalGapTest(string s)
    {
        Assert.True(MeasurementFormatter.Default.TryParse(s, out var measurement));
        Assert.Equal(3.5, measurement.NominalValue);
        Assert.Equal(TimeUnit.Microsecond, measurement.Unit);
    }

    /// <summary>
    /// Ratio and Disparity carry no abbreviation, so they are indistinguishable from plain numbers
    /// once serialized. Parsing such a string yields a Number measurement.
    /// </summary>
    [Theory]
    [InlineData("16")]
    public void UnitlessFamiliesCollapseToNumberTest(string s)
    {
        Assert.True(MeasurementFormatter.Default.TryParse(s, out var measurement));
        Assert.Equal(MeasurementUnit.Number, measurement.Unit);
    }

    /// <summary>
    /// Formatting must not depend on the ambient culture: callers that do not pass a format provider
    /// (TimeInterval.ToString(), Frequency.ToString(), Threshold.ToString(), CpuBrandHelper) have to
    /// produce identical output on every machine.
    /// </summary>
    [Theory]
    [InlineData("ru-RU")]
    [InlineData("de-DE")]
    [InlineData("en-US")]
    public void FormatIsCultureIndependentTest(string cultureName)
    {
        CultureScope.Run(cultureName, () =>
        {
            var measurement = Frequency.FromGHz(3.1).ToMeasurement(FrequencyUnit.GHz);
            Assert.Equal("3.10GHz", MeasurementFormatter.Default.Format(measurement, "N2"));
            Assert.Equal("1.234us", TimeInterval.FromNanoseconds(1234).ToString());
            Assert.Equal("1.5KB", SizeValue.FromBytes(1024 + 512).ToString());
        });
    }

    /// <summary>
    /// An explicitly passed provider still wins over the invariant default.
    /// </summary>
    [Fact]
    public void ExplicitFormatProviderIsRespectedTest()
    {
        var measurement = Frequency.FromGHz(3.1).ToMeasurement(FrequencyUnit.GHz);
        string actual = MeasurementFormatter.Default.Format(measurement, "N2", new CultureInfo("ru-RU"));
        Assert.Equal("3,10GHz", actual);
    }
}

using Perfolizer.Extensions;
using Perfolizer.Helpers;
using Perfolizer.Json;
using Perfolizer.Models;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Json;

/// <summary>
/// In the Turkish and Azerbaijani cultures, 'I' lower-cases to the dotless 'ı' and 'i' upper-cases to the dotted 'İ',
/// so JSON keys, enum values and column titles must not follow the ambient culture.
/// </summary>
public class LightJsonSerializerCultureTests
{
    private class SampleInfo : AbstractInfo
    {
        public int IterationIndex { get; set; }
        public SampleKind Kind { get; set; }
    }

    private enum SampleKind
    {
        Unknown,
        Infrastructure
    }

    [Theory]
    [InlineData("tr-TR")]
    [InlineData("az-Latn-AZ")]
    public void KeysAndEnumValuesAreCultureIndependentTest(string cultureName)
    {
        CultureScope.Run(cultureName, () =>
        {
            var info = new SampleInfo { IterationIndex = 1, Kind = SampleKind.Infrastructure };
            string json = LightJsonSerializer.Serialize(info, new LightJsonSettings { Indent = false });
            Assert.Equal("{\"iterationIndex\":1,\"kind\":\"infrastructure\"}", json);
        });
    }

    [Theory]
    [InlineData("tr-TR")]
    [InlineData("az-Latn-AZ")]
    public void CasingHelpersAreCultureIndependentTest(string cultureName)
    {
        CultureScope.Run(cultureName, () =>
        {
            Assert.Equal("iterations", "Iterations".ToCamelCase());
            Assert.Equal("Iterations", "iterations".CapitalizeFirst());
            Assert.Equal("Iterations", StringHelper.FirstUpper("iterations"));
        });
    }
}

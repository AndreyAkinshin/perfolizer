using Perfolizer.Mathematics.Histograms;

namespace Perfolizer.Tests.Mathematics.Histograms;

public class GeneralHistogramTests
{
    [Fact]
    public void EmptyListTest()
    {
        foreach (var builder in HistogramBuilder.AllBuilders)
            Assert.Throws<ArgumentException>(() => builder.Build(Array.Empty<double>(), 1));
    }

    [Fact]
    public void NegativeBinSizeTest()
    {
        foreach (var builder in HistogramBuilder.AllBuilders)
            Assert.Throws<ArgumentException>(() => builder.Build(new double[] { 1, 2 }, -3));
    }
}
﻿using Perfolizer.Mathematics.Histograms;

namespace Perfolizer.Tests.Mathematics.Histograms;

public class HistogramTests
{
    private readonly ITestOutputHelper output;

    public HistogramTests(ITestOutputHelper output) => this.output = output;

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

    [Fact]
    public void SimpleHistogramTest1()
    {
        DoSimpleHistogramTest(new[] { 1.0, 2.0, 3.0, 4.0, 5.0 }, 1,
            new[]
            {
                new[] { 1.0 },
                new[] { 2.0 },
                new[] { 3.0 },
                new[] { 4.0 },
                new[] { 5.0 }
            });
    }

    [Fact]
    public void SimpleHistogramTest2()
    {
        DoSimpleHistogramTest(new[] { 1.0, 2.0, 3.0, 4.0, 5.0 }, 2.5,
            new[]
            {
                new[] { 1.0, 2.0 },
                new[] { 3.0, 4.0 },
                new[] { 5.0 }
            });
    }

    [Fact]
    public void SimpleHistogramTest3()
    {
        DoSimpleHistogramTest(new[] { 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 2.7 }, 2.0,
            new[]
            {
                new[] { 1.0, 1.1, 1.2, 1.3, 1.4, 1.5 },
                new[] { 2.7 }
            });
    }

    private void DoSimpleHistogramTest(double[] values, double binSize, double[][] bins)
    {
        var expectedHistogram = Histogram.BuildManual(binSize, bins);
        var actualHistogram = HistogramBuilder.Simple.Build(values, binSize);
        PrintHistogram("Expected", expectedHistogram);
        PrintHistogram("Actual", actualHistogram);

        Assert.Equal(bins.Length, actualHistogram.Bins.Length);
        for (int i = 0; i < actualHistogram.Bins.Length; i++)
            Assert.Equal(bins[i], actualHistogram.Bins[i].Values);
    }

    private void PrintHistogram(string title, Histogram histogram)
    {
        output.WriteLine($"=== {title}:Short ===");
        output.WriteLine(histogram.ToString(histogram.CreateNanosecondFormatter()));
        output.WriteLine($"=== {title}:Full ===");
        output.WriteLine(histogram.ToString(histogram.CreateNanosecondFormatter()));
    }
}
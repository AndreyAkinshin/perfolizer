using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Horology;
using Perfolizer.Mathematics.Histograms;
using Perfolizer.Mathematics.Multimodality;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Histograms
{
    public static class HistogramTestHelper
    {
        [AssertionMethod]
        public static void DoHistogramTest(ITestOutputHelper output, IHistogramBuilder builder,
            double[] values, double[][] bins)
        {
            var actualHistogram = builder.Build(values);
            Check(output, bins, actualHistogram);
        }

        [AssertionMethod]
        public static void DoHistogramTest(ITestOutputHelper output, IHistogramBuilder builder,
            double[] values, double binSize, double[][] bins)
        {
            var actualHistogram = builder.Build(values, binSize);
            Check(output, bins, actualHistogram);
        }

        [AssertionMethod]
        public static void DoHistogramTest(ITestOutputHelper output, IHistogramBuilder builder,
            double[] values, bool[] states)
        {
            var actualHistogram = builder.Build(values);
            Check(output, states, actualHistogram);
        }

        [AssertionMethod]
        private static void Check(ITestOutputHelper output, double[][] expectedBins, Histogram actualHistogram)
        {
            var expectedHistogram = Histogram.BuildManual(0, expectedBins);
            output.Print("Expected", expectedHistogram);
            output.Print("Actual", actualHistogram);

            Assert.Equal(expectedBins.Length, actualHistogram.Bins.Length);
            for (int i = 0; i < actualHistogram.Bins.Length; i++)
                Assert.Equal(expectedBins[i], actualHistogram.Bins[i].Values);
        }

        [AssertionMethod]
        private static void Check(ITestOutputHelper output, bool[] expectedStates, Histogram actualHistogram)
        {
            output.Print("Actual", actualHistogram);

            Assert.Equal(expectedStates.Length, actualHistogram.Bins.Length);
            for (int i = 0; i < actualHistogram.Bins.Length; i++)
                Assert.Equal(expectedStates[i], actualHistogram.Bins[i].HasAny);
        }

        public static void Print(this ITestOutputHelper output, string title, Histogram histogram)
        {
            var values = histogram.AllValues().CopyToArray();
            double mValue = MValueCalculator.Calculate(values);
            var cultureInfo = TestCultureInfo.Instance;
            var binSizeInterval = TimeInterval.FromNanoseconds(histogram.BinSize);
            output.WriteLine($"=== {title}:Short (BinSize={binSizeInterval.ToString("N4", cultureInfo)}, mValue={mValue.ToString("0.##", cultureInfo)}) ===");
            output.WriteLine(histogram.ToString(histogram.CreateNanosecondFormatter(cultureInfo, "0.0000")));
            output.WriteLine($"=== {title}:Full (BinSize={binSizeInterval.ToString("N4", cultureInfo)}, mValue={mValue.ToString("0.##", cultureInfo)}) ===");
            output.WriteLine(histogram.ToString(histogram.CreateNanosecondFormatter(cultureInfo, "0.0000"), full: true));
        }
    }
}
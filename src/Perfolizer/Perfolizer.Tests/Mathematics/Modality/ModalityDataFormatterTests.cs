using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Collections;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Multimodality;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Modality
{
    public class ModalityDataFormatterTests
    {
        private readonly ITestOutputHelper output;

        public ModalityDataFormatterTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static readonly IReadOnlyList<double> Sample1 = GenerateSample(1, 0, 0, 0);
        private static readonly IReadOnlyList<double> Sample2 = GenerateSample(1, 1, 0, 1);
        private static readonly IReadOnlyList<double> Sample3 = GenerateSample(1, 2, 0, 3);
        private static readonly IReadOnlyList<double> Sample4 = GenerateSample(2, 0, 0, 0);
        private static readonly IReadOnlyList<double> Sample5 = GenerateSample(2, 1, 2, 1);
        private static readonly IReadOnlyList<double> Sample6 = GenerateSample(3, 2, 1, 0);
        private static readonly IReadOnlyList<double> Sample7 = GenerateSample(5, 3, 3, 3);

        private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
        {
            {"Default1", TestData.Default(Sample1, "[7 | 10 | 13]_50")},
            {"Default2", TestData.Default(Sample2, "{0} + [7 | 10 | 13]_50 + {20}")},
            {"Default3", TestData.Default(Sample3, "{-1, 1} + [7 | 10 | 13]_50 + {19..21}_3")},
            {"Default4", TestData.Default(Sample4, "[7 | 10 | 13]_50 + [27 | 30 | 33]_50")},
            {"Default5", TestData.Default(Sample5, "{0} + [7 | 10 | 13]_50 + {19, 21} + [27 | 30 | 33]_50 + {40}")},
            {"Default6", TestData.Default(Sample6, "{-1, 1} + [7 | 10 | 13]_50 + <1 mode> + [47 | 50 | 53]_50")},
            {"Default7", TestData.Default(Sample7, "{-1..1}_3 + [7 | 10 | 13]_50 + <3 modes> + [87 | 90 | 93]_50 + {99..101}_3")},
            {"Auto1", TestData.Auto(Sample1, "[7 | 10 | 13]_50")},
            {"Auto2", TestData.Auto(Sample2, "{0} + [7 | 10 | 13]_50 + {20}")},
            {"Auto3", TestData.Auto(Sample3, "{-1, 1} + [7 | 10 | 13]_50 + {19..21}_3")},
            {"Auto4", TestData.Auto(Sample4, "[7; 13]_50 + [27; 33]_50")},
            {"Auto5", TestData.Auto(Sample5, "{0} + [7; 13]_50 + {19, 21} + [27; 33]_50 + {40}")},
            {"Auto6", TestData.Auto(Sample6, "{-1, 1} + [7; 13] + <1 mode> + [47; 53]")},
            {"Auto7", TestData.Auto(Sample7, "{-1..1} + [7; 13] + <3 modes> + [87; 93] + {99..101}")},
            {"Compact1", TestData.Compact(Sample1, "[7; 13]")},
            {"Compact2", TestData.Compact(Sample2, "[0; 20]")},
            {"Compact3", TestData.Compact(Sample3, "[-1; 21]")},
            {"Compact4", TestData.Compact(Sample4, "[7; 13] + [27; 33]")},
            {"Compact5", TestData.Compact(Sample5, "[0; 19] + [21; 40]")},
            {"Compact6", TestData.Compact(Sample6, "[-1; 20] + <1 mode> + [47; 53]")},
            {"Compact7", TestData.Compact(Sample7, "[-1; 20] + <3 modes> + [80; 101]")},
            {"Full1", TestData.Full(Sample1, "[7 | 10 | 13]_50")},
            {"Full2", TestData.Full(Sample2, "{0} + [7 | 10 | 13]_50 + {20}")},
            {"Full3", TestData.Full(Sample3, "{-1, 1} + [7 | 10 | 13]_50 + {19..21}_3")},
            {"Full4", TestData.Full(Sample4, "[7 | 10 | 13]_50 + [27 | 30 | 33]_50")},
            {"Full5", TestData.Full(Sample5, "{0} + [7 | 10 | 13]_50 + {19, 21} + [27 | 30 | 33]_50 + {40}")},
            {
                "Full6",
                TestData.Full(Sample6, "{-1, 1} + [7 | 10 | 13]_50 + {20, 27} + [27 | 30 | 33]_49 + {40} + [47 | 50 | 53]_50")
            },
            {
                "Full7",
                TestData.Full(Sample7,
                    "{-1..1}_3 + [7 | 10 | 13]_50 + {19..21}_3 + [27 | 30 | 33]_50 + {39..41}_3 + [47 | 50 | 53]_50 + {59..61}_3 + [67 | 70 | 73]_50 + {79..81}_3 + [87 | 90 | 93]_50 + {99..101}_3")
            },
        };

        private class TestData
        {
            public IModalityDataFormatter ModalityDataFormatter { get; }
            public IReadOnlyList<double> Values { get; }
            public string Expected { get; }

            public TestData(IModalityDataFormatter modalityDataFormatter, IReadOnlyList<double> values, string expected)
            {
                ModalityDataFormatter = modalityDataFormatter;
                Values = values;
                Expected = expected;
            }

            public static TestData Default(IReadOnlyList<double> values, string expected) =>
                new TestData(ManualModalityDataFormatter.Default(), values, expected);

            public static TestData Compact(IReadOnlyList<double> values, string expected) =>
                new TestData(ManualModalityDataFormatter.Compact(), values, expected);

            public static TestData Full(IReadOnlyList<double> values, string expected) =>
                new TestData(ManualModalityDataFormatter.Full(), values, expected);
            
            public static TestData Auto(IReadOnlyList<double> values, string expected) =>
                new TestData(AutomaticModalityDataFormatter.Instance, values, expected);
        }

        private static IReadOnlyList<double> GenerateSample(int modeCount, int lowerOutliers, int intermodalOutliers, int upperOutliers)
        {
            var values = new List<double>();
            int position = 0;

            IEnumerable<double> Series(int n, double min, double max)
            {
                if (n == 1)
                    return new[] {(min + max) / 2};
                if (n == 2)
                    return new[] {(min * 2 + max) / 3, (min + max * 2) / 3};
                double middle = (min + max) / 2;
                double scale = (max - min) / 2;
                return Enumerable.Range(0, n)
                    .Select(x => (x % 2 == 0 ? 1.0 : -1.0) * ((x / 2) * 1.0 / (n / 2)).Sqr())
                    .Select(x => middle + x * scale);
            }

            void AddValues(int count, double delta)
            {
                if (count == 1)
                {
                    values.Add(position);
                }
                else if (count >= 2)
                {
                    double min = position - delta;
                    double max = position + delta;
                    values.Add(min);
                    values.Add(max);
                    values.AddRange(Series(count - 2, min, max));
                }

                position += 10;
            }

            AddValues(lowerOutliers, 1);

            for (int i = 0; i < modeCount; i++)
            {
                if (i != 0)
                    AddValues(intermodalOutliers, 1);
                AddValues(50, 3);
            }

            AddValues(upperOutliers, 1);

            return values;
        }

        [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);

        [Theory]
        [MemberData(nameof(TestDataKeys))]
        public void ManualModalityDataFormatterTest([NotNull] string testKey)
        {
            var testData = TestDataMap[testKey];
            var modalityDetector = new LowlandModalityDetector();
            var modalityData = modalityDetector.DetectModes(testData.Values.ToSample());
            var modalityFormatter = testData.ModalityDataFormatter;

            string expected = testData.Expected;
            string actual = modalityFormatter.Format(modalityData, "N0");
            output.WriteLine($"Expected : {expected}");
            output.WriteLine($"Actual   : {actual}");

            Assert.Equal(expected, actual);
        }
    }
}
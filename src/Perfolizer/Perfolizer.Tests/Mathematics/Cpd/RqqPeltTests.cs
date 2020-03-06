using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Cpd;
using Perfolizer.Mathematics.Randomization;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Cpd
{
    public class RqqPeltTests
    {
        private readonly ITestOutputHelper output;
        private readonly PeltChangePointDetector detector = RqqPeltChangePointDetector.Instance;

        public RqqPeltTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [AssertionMethod]
        private void Check([NotNull] double[] data, int minDistance, [NotNull] int[] expectedChangePoints)
        {
            var actualChangePoints = detector.GetChangePointIndexes(data, minDistance);
            output.WriteLine("EXPECTED : " + string.Join(", ", expectedChangePoints));
            output.WriteLine("ACTUAL   : " + string.Join(", ", actualChangePoints));
            output.WriteLine("");

            Assert.Equal(expectedChangePoints, actualChangePoints);
        }

        [Fact]
        public void Tie01() => Check(new double[]
        {
            0, 0, 0, 0, 0, 100, 100, 100, 100
        }, 1, new[] {4});

        [Fact]
        public void Tie02() => Check(new double[]
        {
            0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2
        }, 1, new[] {5, 11});

        [Fact]
        public void Check_WhenTwoMinDistanceLessThanDataLength_ReturnEmptyArray() => Check(new double[]
        {
            0, 0, 0, 0, 0
        }, 4, new int[0]);

        [Fact]
        public void ArithmeticProgression() => Check(Enumerable.Range(1, 500).Select(it => (double) it).ToArray(), 10, new[]
        {
            9, 19, 29, 39, 49, 59, 69, 79, 89, 99, 109, 119, 129, 139, 149, 159, 169, 179, 189, 199, 209, 219, 229, 239,
            249, 259, 269, 279, 289, 299, 309, 319, 329, 339, 349, 359, 369, 379, 389, 399, 409, 419, 429, 439, 449,
            459, 469, 479, 489
        });

        [AssertionMethod]
        private void Check100(int count, int error, int[] indexes)
        {
            for (int i = 0; i < indexes.Length; i++)
                output.WriteLine(indexes[i].ToString());

            Assert.Equal(count - 1, indexes.Length);

            int totalError = 0;
            for (int i = 0; i < count - 1; i++)
                totalError += Math.Abs((i + 1) * 100 - 1 - indexes[i]);
            output.WriteLine("Total Error: " + totalError);

            for (int i = 0; i < count - 1; i++)
            {
                int trueValue = (i + 1) * 100 - 1;
                Assert.True(Math.Abs(trueValue - indexes[i]) <= error);
            }
        }

        public static IEnumerable<object[]> GaussianMeanProgressionData()
        {
            var counts = new[] {2, 3, 4, 10};
            var meanFactors = new[] {20};
            var stdDevToError = new Dictionary<int, int>
            {
                {1, 0},
                {5, 1},
                {7, 14}
            };
            foreach (int count in counts)
            foreach (int meanFactor in meanFactors)
            foreach ((int stdDev, int error) in stdDevToError)
            {
                yield return new object[] {count, meanFactor, stdDev, error};
                yield return new object[] {count, -meanFactor, stdDev, error};
            }
        }

        [Theory]
        [MemberData(nameof(GaussianMeanProgressionData))]
        public void GaussianMeanProgression(int count, int meanFactor, int stdDev, int error)
        {
            var random = new RandomDistribution(42);

            var data = new List<double>();
            for (int i = 0; i < count; i++)
                data.AddRange(random.Gaussian(100, mean: meanFactor * i, stdDev: stdDev));

            var indexes = detector.GetChangePointIndexes(data.ToArray(), 20);
            Check100(count, error, indexes);
        }

        [Theory]
        [InlineData(0, "1;5")]
        [InlineData(0, "1;5;30")]
        public void GaussianStdDevProgression(int error, [NotNull] string stdDevValuesString)
        {
            var random = new RandomDistribution(42);

            var stdDevValues = stdDevValuesString.Split(';').Select(double.Parse).ToArray();
            var data = new List<double>();
            foreach (double stdDev in stdDevValues)
                data.AddRange(random.Gaussian(100, mean: 0, stdDev: stdDev));

            var indexes = detector.GetChangePointIndexes(data.ToArray(), 20);
            Check100(stdDevValues.Length, error, indexes);
        }

        public static IEnumerable<object[]> BimodalProgressionData()
        {
            var counts = new[] {2, 10};
            var meanFactors = new[] {20};
            var stdDevToError = new Dictionary<int, int>
            {
                {1, 7},
                {5, 10}
            };
            foreach (int count in counts)
            foreach (int meanFactor in meanFactors)
            foreach ((int stdDev, int error) in stdDevToError)
            {
                yield return new object[] {count, meanFactor, stdDev, error};
                yield return new object[] {count, -meanFactor, stdDev, error};
            }
        }

        [Theory]
        [MemberData(nameof(BimodalProgressionData))]
        public void BimodalProgression(int count, int meanFactor, int stdDev, int error)
        {
            var random = new RandomDistribution(42);
            var shuffler = new Shuffler(42);

            var data = new List<double>();
            for (int i = 0; i < count; i++)
            {
                data.AddRange(random.Gaussian(30, 0, stdDev));
                data.AddRange(random.Gaussian(70, (i + 1) * meanFactor, stdDev));
                shuffler.Shuffle(data, i * 100, 100);
            }

            var indexes = detector.GetChangePointIndexes(data.ToArray(), 20);
            Check100(count, error, indexes);
        }
    }
}
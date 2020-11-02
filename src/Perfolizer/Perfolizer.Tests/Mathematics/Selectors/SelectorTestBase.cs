using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Selectors
{
    public abstract class SelectorTestBase
    {
        protected readonly ITestOutputHelper Output;

        protected SelectorTestBase(ITestOutputHelper output)
        {
            Output = output;
        }

        protected abstract class SelectorAdapter
        {
            public abstract double Select(int k);
        }

        [JetBrains.Annotations.NotNull]
        protected abstract SelectorAdapter CreateEstimator([JetBrains.Annotations.NotNull] double[] values);

        [AssertionMethod]
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        protected void Check([JetBrains.Annotations.NotNull] double[] data)
        {
            var expectedSortedData = data.ToArray();
            Array.Sort(expectedSortedData);
            var actualSortedData = new double[data.Length];
            var estimator = CreateEstimator(data);
            for (int i = 0; i < data.Length; i++)
                actualSortedData[i] = estimator.Select(i);

            Output.WriteLine("EXPECTED : " + string.Join("; ", expectedSortedData));
            Output.WriteLine("ACTUAL   : " + string.Join("; ", actualSortedData));
            Output.WriteLine("");
            var mismatch = Enumerable.Range(0, data.Length)
                .Where(i => actualSortedData[i] != expectedSortedData[i])
                .ToArray();
            if (mismatch.Any())
                Output.WriteLine("MISMATCHED INDEXES: " + string.Join("; ", mismatch));

            DumpDetails(estimator);

            Assert.Equal(actualSortedData, expectedSortedData);
        }

        protected virtual void DumpDetails(SelectorAdapter selectorAdapter)
        {
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(18)]
        [InlineData(19)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(22)]
        [InlineData(23)]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(26)]
        [InlineData(27)]
        [InlineData(28)]
        [InlineData(29)]
        [InlineData(30)]
        [InlineData(31)]
        [InlineData(32)]
        [InlineData(33)]
        [InlineData(34)]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(37)]
        [InlineData(38)]
        [InlineData(39)]
        [InlineData(40)]
        [InlineData(41)]
        [InlineData(42)]
        [InlineData(43)]
        [InlineData(44)]
        [InlineData(45)]
        [InlineData(46)]
        [InlineData(47)]
        [InlineData(48)]
        [InlineData(49)]
        [InlineData(50)]
        [InlineData(51)]
        [InlineData(52)]
        [InlineData(53)]
        [InlineData(54)]
        [InlineData(55)]
        [InlineData(56)]
        [InlineData(57)]
        [InlineData(58)]
        [InlineData(59)]
        [InlineData(60)]
        [InlineData(61)]
        [InlineData(62)]
        [InlineData(63)]
        [InlineData(64)]
        [InlineData(65)]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(300)]
        [InlineData(400)]
        [InlineData(500)]
        public void RandomSpecificN(int n)
        {
            var random = new Random(42);
            Check(Enumerable.Range(2, n).Select(x => (double) random.Next(1000)).ToArray());
        }

        private void RandomManyN(int minN, int maxN)
        {
            var random = new Random(42);
            for (int n = minN; n <= maxN; n++)
            {
                var data = Enumerable.Range(2, n).Select(u => (double) random.Next(1000)).ToArray();
                var sorted = new double[n];
                Array.Copy(data, sorted, n);
                Array.Sort(sorted);
                int k = random.Next(n);
                var estimator = CreateEstimator(data);
                var sw = Stopwatch.StartNew();
                double x = estimator.Select(k);
                sw.Stop();
                Output.WriteLine($"N = {n}, Elapsed = {sw.ElapsedMilliseconds}ms");
                Assert.Equal(sorted[k], x);
            }
        }

        [Fact]
        public void RandomSmallN() => RandomManyN(2, 100);

        [Fact]
        [Trait(TraitConstants.Category, TraitConstants.Slow)]
        public void RandomMediumN() => RandomManyN(100_000, 100_010);
        
        // [Fact]
        public void RandomHugeN() => RandomManyN(100_000_000, 100_000_000);

        [Theory]
        [InlineData("2,2,3")]
        [InlineData("2,3,2")]
        [InlineData("3,2,2")]
        [InlineData("2,2,2,3")]
        [InlineData("2,2,3,2")]
        [InlineData("2,3,2,2")]
        [InlineData("3,2,2,2")]
        public void Ties([JetBrains.Annotations.NotNull] string data) =>
            Check(data.Split(',').Select(s => (double) int.Parse(s)).ToArray());
    }
}
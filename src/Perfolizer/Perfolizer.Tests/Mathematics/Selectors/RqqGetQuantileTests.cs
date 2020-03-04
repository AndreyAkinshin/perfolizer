using System;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Randomization;
using Perfolizer.Mathematics.Selectors;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Selectors
{
    public class RqqGetQuantileTests : SelectorTestBase
    {
        public RqqGetQuantileTests(ITestOutputHelper output) : base(output)
        {
        }

        private class RqqSelectorAdapter : SelectorAdapter
        {
            public Rqq Rqq { get; }
            private readonly int n;

            public RqqSelectorAdapter([NotNull] double[] values)
            {
                Rqq = new Rqq(values);
                n = values.Length;
            }

            public override double Select(int k) => Rqq.Select(0, n - 1, k);
        }

        protected override SelectorAdapter CreateEstimator(double[] values)
        {
            return new RqqSelectorAdapter(values);
        }

        protected override void DumpDetails(SelectorAdapter selectorAdapter)
        {
            DumpRqqTree(((RqqSelectorAdapter) selectorAdapter).Rqq);
        }

        private void DumpRqqTree([NotNull] Rqq rqq)
        {
            using var memoryStream = new MemoryStream();
            using var sw = new StreamWriter(memoryStream);
            rqq.DumpTreeAscii(sw, true);
            Output.WriteLine(Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int) memoryStream.Length));
        }

        [Fact]
        public void SuperEtalon()
        {
            var rqq = new Rqq(new double[] {6, 2, 0, 7, 9, 3, 1, 8, 5, 4});
            double actual = rqq.Select(2, 8, 4);
            Assert.Equal(7, actual);
        }

        [Fact]
        public void Etalon() => Check(new double[] {6, 2, 0, 7, 9, 3, 1, 8, 5, 4});

        [Fact]
        public void Test02() => Check(new double[] {3, 26, 13, 12, 8, 12, 3});

        [Fact]
        public void Test03() => Check(new double[] {0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2});

        [Fact]
        public void Test04() => Check(new double[] {3240, 3207, 2029, 3028, 3021, 2624, 3290, 2823, 3573});

        [Fact]
        public void RandomAllRanges()
        {
            for (int n = 3; n < 20; n++)
            {
                var data = new RandomDistribution(42).IntegerUniform(n, 0, 100).Select(x => (double)x).ToArray();
                Output.WriteLine("[DATA] " + string.Join("; ", data));
                var buffer = new double[data.Length];
                var rqq = new Rqq(data);
                for (int i = 0; i < n; i++)
                for (int j = i; j < n; j++)
                for (int k = 0; k < j - i; k++)
                {
                    double actual = rqq.Select(i, j, k);
                    Array.Copy(data, i, buffer, 0, j - i + 1);
                    Array.Sort(buffer, 0, j - i + 1);
                    double expected = buffer[k];
                    Output.WriteLine($"n = {n}, i = {i}, j = {j}, k = {k}, expected = {expected}, actual = {actual}");
                    if (actual != expected)
                        DumpRqqTree(rqq);
                    Assert.Equal(expected, actual);
                }
            }
        }
        

        [Fact]
        public void Test01()
        {
            var rqq = new Rqq(new double[] {0, 0, 0, 0, 0, 100, 100, 100, 100});
            DumpRqqTree(rqq);
            var probs = Enumerable.Range(0, 10).Select(x => x * 1.0 / 9.0).ToArray();
            foreach (double prob in probs)
            {
                Assert.Equal(0, rqq.GetQuantile(0, 4, prob));
                Assert.Equal(100, rqq.GetQuantile(5, 8, prob));
            }
        }
    }
}
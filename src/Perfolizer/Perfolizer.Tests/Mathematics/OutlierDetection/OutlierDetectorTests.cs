using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.OutlierDetection;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.OutlierDetection
{
    public abstract class OutlierDetectorTests
    {
        [NotNull] protected readonly ITestOutputHelper Output;

        protected OutlierDetectorTests([NotNull] ITestOutputHelper output)
        {
            Output = output;
        }

        /// <summary>
        /// Data set based on Table 1 "Outlier Score Cases" from the following paper:
        /// Yang, Jiawei, Susanto Rahardja, and Pasi Fränti. "Outlier detection: how to threshold outlier scores?."
        /// In Proceedings of the International Conference on Artificial Intelligence,
        /// Information Processing and Cloud Computing, pp. 1-6. 2019.
        /// https://www.researchgate.net/publication/337883760_Outlier_detection_how_to_threshold_outlier_scores
        /// </summary>
        protected static class YangDataSet
        {
            public static readonly double[] X0 = {1, 2, 3, 6, 8};
            public static readonly double[] X1 = {1, 2, 3, 6, 8, 1000};
            public static readonly double[] X2 = {1, 2, 3, 6, 8, 500, 1000};
            public static readonly double[] X3 = {1, 2, 3, 6, 8, 16, 17, 18, 18, 60, 1000};
            public static readonly double[] X4 = {1, 2, 3, 6, 8, 16, 17, 18, 18, 60, 300, 500, 1000, 1500};
        }

        protected class TestData
        {
            [NotNull] public double[] Values { get; }
            [NotNull] public double[] ExpectedOutliers { get; }

            public TestData([NotNull] double[] values, [NotNull] double[] expectedOutliers)
            {
                Values = values;
                ExpectedOutliers = expectedOutliers;
            }

            public void Deconstruct([NotNull] out double[] values, [NotNull] out double[] expectedOutliers)
            {
                values = Values;
                expectedOutliers = ExpectedOutliers;
            }
        }

        protected abstract IOutlierDetector CreateOutlierDetector([NotNull] IReadOnlyList<double> values);

        protected void Check([NotNull] TestData testData)
        {
            var (values, expectedOutliers) = testData;
            var outlierDetector = CreateOutlierDetector(values);
            var actualOutliers = outlierDetector.GetAllOutliers(values).ToList();

            Output.WriteLine("Values: [{0}]", string.Join("; ", values));
            Output.WriteLine("ExpectedOutliers : [{0}]", string.Join("; ", expectedOutliers));
            Output.WriteLine("ActualOutliers   : [{0}]", string.Join("; ", actualOutliers));
            DumpDetails(testData);

            Assert.Equal(expectedOutliers, actualOutliers);
        }

        protected virtual void DumpDetails([NotNull] TestData testData)
        {
        }
    }
}
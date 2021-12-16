using System;
using System.Text;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.QuantileEstimators
{
    public class MovingP2QuantileEstimatorTests
    {
        private readonly ITestOutputHelper output;

        public MovingP2QuantileEstimatorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(1000, 51, 0.5, 0.2, 0.7)]
        public void MovingP2QuantileEstimatorMedianTest1(int n, int windowSize, double probability, double relativeThreshold,
            double minSuccessRate)
        {
            var random = new Random();
            double[] data = new double[n];
            for (int i = 0; i < n; i++)
            {
                data[i] = 10 + Math.Sin(i / 20.0) * 5 + random.NextDouble(-3, 3);
                if (random.Next(10) == 0 && i > windowSize / 2)
                    data[i] += random.Next(20, 50);
                data[i] = Math.Round(data[i], 3);
            }

            var mp2Estimator = new MovingP2QuantileEstimator(probability, windowSize);
            var phEstimator = new PartitioningHeapsMovingQuantileEstimator(probability, windowSize);

            var outputBuilder = new StringBuilder();
            outputBuilder.AppendLine("i,data,estimation,true");
            int successCounter = 0;
            for (int i = 0; i < n; i++)
            {
                mp2Estimator.Add(data[i]);
                phEstimator.Add(data[i]);

                double mp2Estimation = mp2Estimator.GetQuantile();
                double trueValue = phEstimator.GetQuantile();

                if (Math.Abs(mp2Estimation - trueValue) / trueValue < relativeThreshold)
                    successCounter++;

                outputBuilder.Append(i);
                outputBuilder.Append(',');
                outputBuilder.Append(data[i].ToString(TestCultureInfo.Instance));
                outputBuilder.Append(',');
                outputBuilder.Append(mp2Estimation.ToString(TestCultureInfo.Instance));
                outputBuilder.Append(',');
                outputBuilder.Append(trueValue.ToString(TestCultureInfo.Instance));
                outputBuilder.AppendLine();
            }
            double actualSuccessRate = successCounter * 1.0 / n;

            output.WriteLine("ExpectedSuccessRate = " + minSuccessRate.ToString(TestCultureInfo.Instance));
            output.WriteLine("ActualSuccessRate   = " + actualSuccessRate.ToString(TestCultureInfo.Instance));
            output.WriteLine();
            output.WriteLine(outputBuilder.ToString());

            Assert.True(successCounter * 1.0 / n > minSuccessRate);
        }
    }
}
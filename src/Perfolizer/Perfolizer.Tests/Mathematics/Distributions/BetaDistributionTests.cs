using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Distributions;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Distributions
{
    public class BetaDistributionTests
    {
        private readonly ITestOutputHelper output;

        public BetaDistributionTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private class TestData
        {
            public double Alpha { get; }
            public double Beta { get; }
            public double[] Expected { get; }

            public TestData(double alpha, double beta, double[] expected)
            {
                Alpha = alpha;
                Beta = beta;
                Expected = expected;
            }
        }

        private static readonly IDictionary<string, TestData> TestDataMap = new Dictionary<string, TestData>
        {
            {
                "1/1", new TestData(1, 1, new[]
                    {
                        0, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5, 0.55, 0.6, 0.65, 0.7, 0.75, 0.8, 0.85, 0.9, 0.95, 1
                    }
                )
            },
            {
                "1/2", new TestData(1, 2, new[]
                    {
                        0, 0.0975, 0.19, 0.2775, 0.36, 0.4375, 0.51, 0.5775, 0.64, 0.6975, 0.75, 0.7975, 0.84, 0.8775, 0.91, 0.9375, 0.96,
                        0.9775, 0.99, 0.9975, 1
                    }
                )
            },
            {
                "1/9", new TestData(1, 9, new[]
                    {
                        0, 0.369750590275391, 0.612579511, 0.768383053716797, 0.865782272,
                        0.924915313720703, 0.959646393, 0.979288087162109, 0.989922304,
                        0.995394633416016, 0.998046875, 0.999243319357422, 0.999737856,
                        0.999921184361328, 0.999980317, 0.999996185302734, 0.999999488,
                        0.999999961556641, 0.999999999, 0.999999999998047, 1
                    }
                )
            },
            {
                "2/3", new TestData(2, 3, new[]
                    {
                        0, 0.01401875, 0.0523, 0.10951875, 0.1808, 0.26171875, 0.3483,
                        0.43701875, 0.5248, 0.60901875, 0.6875, 0.75851875, 0.8208, 0.87351875,
                        0.9163, 0.94921875, 0.9728, 0.98801875, 0.9963, 0.99951875, 1
                    }
                )
            },
            {
                "3/4", new TestData(3, 4, new[]
                    {
                        0, 0.00222984375, 0.01585, 0.04733859375, 0.0988800000000001,
                        0.16943359375, 0.25569, 0.35291484375, 0.45568, 0.55848234375,
                        0.65625, 0.74473609375, 0.8208, 0.88257609375, 0.92953, 0.96240234375,
                        0.98304, 0.99411484375, 0.99873, 0.99991359375, 1
                    }
                )
            },
            {
                "3/9", new TestData(3, 9, new[]
                    {
                        0, 0.0152352973052979, 0.0895618508500001, 0.22118801812273,
                        0.3825984512, 0.544799089431763, 0.68725954575, 0.799871142203882,
                        0.8810831872, 0.934776495754321, 0.96728515625, 0.985197434929565,
                        0.9940755456, 0.997961630544849, 0.99942230395, 0.999873876571655,
                        0.999981056, 0.99999841757522, 0.99999995445, 0.999999999902026, 1
                    }
                )
            },
            {
                "7/9", new TestData(7, 9, new[]
                    {
                        0, 3.51819989673472e-06, 0.000310630537603001, 0.00360558584527071,
                        0.018058806984704, 0.0566203100606799, 0.131142573383121, 0.245157531759255,
                        0.390186844291072, 0.547839597959584, 0.696380615234375, 0.818239534961649,
                        0.904952591843328, 0.957806162061808, 0.984757474230229, 0.995806985534728,
                        0.999215014608896, 0.999919095375415, 0.999997153517547, 0.999999992581744, 1
                    }
                )
            },
            {
                "20/30", new TestData(20, 30, new[]
                    {
                        0, 6.56738006947342e-14, 1.56945459993621e-08, 1.1078466689654e-05, 
                        0.00068772762874159, 0.0108557281062408, 0.0699870933005074, 
                        0.238560163534381, 0.507700199657648, 0.767111393213431, 0.923796114019261, 
                        0.983626775463556, 0.997831590079582, 0.99983815136103, 0.999994056247415, 
                        0.999999913551269, 0.999999999656384, 0.999999999999815, 1, 1, 1
                    }
                )
            },
            {
                "100/200", new TestData(100, 200, new[]
                    {
                        0, 6.57083740806873e-91, 1.43861799878687e-62, 9.30307912108281e-47, 
                        4.2358741961308e-36, 2.77899816370087e-28, 2.80568701979187e-22, 
                        1.53954987363643e-17, 9.73857901023567e-14, 1.15221521461999e-10, 
                        3.54600736656723e-08, 3.5909419276845e-06, 0.000142424760368366, 
                        0.00252993220342043, 0.0224244247310462, 0.10884306564491, 0.315878806513032, 
                        0.60162293459243, 0.836589797625881, 0.954696094562049, 0.991684464926086, 
                        0.999000087730313, 0.999921762637511, 0.999996042019178, 0.999999871550767, 
                        0.999999997353445, 0.999999999965849, 0.999999999999729, 0.999999999999999, 
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
                    }
                )
            }
        };

        [UsedImplicitly] public static TheoryData<string> TestDataKeys = TheoryDataHelper.Create(TestDataMap.Keys);


        [Theory]
        [MemberData(nameof(TestDataKeys))]
        private void BetaCdf(string testDataKey)
        {
            var testData = TestDataMap[testDataKey];
            double a = testData.Alpha, b = testData.Beta;
            var expected = testData.Expected;
            
            var distribution = new BetaDistribution(a, b);
            var comparer = new AbsoluteEqualityComparer(1e-2);
            int n = expected.Length - 1;

            double maxDelta = 0;
            for (int i = 0; i < expected.Length; i++)
            {
                double x = i * 1.0 / n;
                double actual = distribution.Cdf(x);
                output.WriteLine($"x = {x}");
                output.WriteLine($"Actual   = {actual}");
                output.WriteLine($"Expected = {expected[i]}");

                maxDelta = Math.Max(maxDelta, Math.Abs(actual - expected[i]));
                Assert.Equal(expected[i], actual, comparer);
            }
            output.WriteLine("-----");
            output.WriteLine($"MaxDelta = {maxDelta}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Perfolizer.Mathematics.OutlierDetection;
using Perfolizer.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.OutlierDetection
{
    public abstract class OutlierDetectorTests
    {
        protected readonly ITestOutputHelper Output;

        protected OutlierDetectorTests(ITestOutputHelper output)
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

        /// <summary>
        /// Data sets based on real performance measurements
        /// </summary>
        protected static class RealDataSet
        {
            public static readonly double[] X0 =
            {
                185, 336, 382, 411, 421, 496, 535, 942, 1236, 1263, 1739, 2037, 2062, 2163, 2540, 2921, 3086, 4225, 4340, 4864, 5026, 5224,
                5607, 5637, 5659, 6437, 6599, 6683, 7133, 7149, 7494, 7674, 8061, 8530, 8903, 9363, 9491, 9522, 10007, 10235, 10271, 10652,
                10775, 10924, 10980, 11030, 11442, 11444, 11703, 11986, 12165, 12168, 12266, 12490, 12556, 12558, 12668, 12682, 13082,
                13100, 13156, 13259, 13278, 13351, 13514, 13811, 14059, 14183, 14221, 14256, 14259, 14319, 15111, 15127, 15342, 15721,
                15791, 15811, 16084, 16093, 16219, 16511, 16800, 16816, 17177, 17273, 17277, 18218, 18632, 18867, 19239, 19348, 19391,
                19691, 19738, 19938, 19985, 20381, 20886, 21270, 21310, 22321, 22599, 22723, 22899, 23086, 24141, 24166, 24260, 24457,
                25905, 28928, 29552, 30738, 34109, 38594, 39075
            };

            public static readonly double[] X1 =
            {
                0, 0, 0, 0, 1821, 4151, 4166, 4199, 4265, 4283, 4288, 4299, 4304, 4314, 4318, 4357, 4402, 4413, 4418, 4418, 4426, 4436,
                4439, 4449, 4467, 4470, 4495, 4510, 4519, 4525, 4529, 4543, 4557, 4571, 4582, 4603, 4607, 4615, 4616, 4618, 4623, 4626,
                4627, 4635, 4650, 4656, 4658, 4668, 4673, 4685, 4692, 4703, 4710, 4727, 4730, 4731, 4743, 4748, 4766, 4767, 4797, 4799,
                4803, 4804, 4805, 4806, 4827, 4840, 4841, 4853, 4853, 4864, 4866, 4872, 4878, 4895, 4896, 4900, 4915, 4920, 4921, 4923,
                4934, 4935, 4939, 4975, 4993, 5017, 5024, 5036, 5039, 5041, 5065, 5079, 5129, 5136, 5172, 5265
            };

            public static readonly double[] X2 =
            {
                95, 3868, 3879, 3905, 3909, 3911, 3917, 3928, 3937, 3937, 3944, 3947, 3947, 3949, 3950, 3953, 3954, 3956, 3960, 3960, 3960,
                3962, 3963, 3963, 3965, 3966, 3968, 3968, 3972, 3975, 3976, 3980, 3981, 3985, 3988, 3992, 3993, 3995, 4000, 4001, 4003,
                4008, 4009, 4010, 4010, 4013, 4014, 4016, 4017, 4019, 4019, 4019, 4021, 4022, 4026, 4027, 4030, 4032, 4041, 4051, 4056,
                4058, 4060, 4061, 4065, 4065, 4066, 4067, 4081, 4083, 4087, 4089, 4090, 4090, 4092, 4107, 4108, 4109, 4118, 4123, 4126,
                4126, 4126, 4128, 4129, 4141, 4146, 4151, 4188, 4190, 4196, 4204, 4205, 4364
            };

            public static readonly double[] X3 =
            {
                1067, 1085, 1133, 1643, 3328, 3351, 3369, 3385, 3412, 3438, 3441, 3451, 3462, 3465, 3497, 3505, 3514, 3519, 3521, 3525,
                3529, 3531, 3555, 3575, 3587, 3600, 3624, 3634, 3635, 3639, 3652, 3652, 3660, 3662, 3665, 3667, 3673, 3677, 3687, 3688,
                3700, 3717, 3736, 3739, 3743, 3761, 3773, 3780, 3783, 3791, 3823, 3833, 3834, 3848, 3859, 3860, 3861, 3866, 3870, 3881,
                3882, 3884, 3892, 3897, 3903, 3909, 3920, 3921, 3928, 3942, 3946, 3959, 3994, 3998, 4023, 4065, 4115, 4161, 4175, 4183,
                4228, 4247, 4253, 4256, 4275, 4277, 4327, 4398, 4416, 4642
            };

            public static readonly double[] X4 =
            {
                16, 79, 650, 653, 663, 1549, 1584, 1601, 1605, 1637, 1654, 1654, 1672, 1710, 1752, 1945, 2009, 2116, 2156, 2183, 2219, 2297,
                2419, 2422, 2504, 2508, 2516, 2593, 2601, 2643, 2662, 2682, 2727, 2942, 3005, 3015, 3072, 3106, 3385, 3468, 3478, 3504,
                3513, 3538, 3559, 3577, 3612, 3631, 3668, 3694, 3738, 3777, 3806, 3871, 3876, 3887, 3907, 3910, 3911, 3924, 3929, 3943,
                3954, 3966, 4029, 4029, 4039, 4064, 4082, 4087, 4140, 4155, 4159, 4198, 4208, 4222, 4340, 4392, 4507, 4551, 4555, 4597,
                4648, 4690, 4694, 4695, 4798, 4799, 4831, 4946, 5114, 5123, 5177, 5563, 5593, 5620, 5790, 6017, 6199, 6230, 6242, 6257,
                6299, 6322, 6325, 6364, 6592, 6639, 6643, 6753, 6810, 7037, 7040, 7125, 7143, 7234, 7278, 7370, 7473, 7558, 7644, 7687,
                7809, 7872, 7934, 7939, 8032, 8056, 8132, 8179, 8348, 8497, 8671, 8723, 8725, 8826, 8916, 8980, 8988, 9086, 9172, 9192,
                9426, 9655, 9681, 9703, 9727, 9950, 10055, 10394, 10574, 10724, 10996, 11030, 11277, 13143, 13517, 13713, 14060, 14893,
                15056, 15364, 15483, 16504, 17208, 17446
            };
        }

        /// <summary>
        /// Data set based on beta-distribution (1, 10) multiplied by 10000
        /// </summary>
        protected static class BetaDataSet
        {
            public static readonly double[] X0 =
            {
                5, 8, 12, 40, 42, 58, 73, 80, 96, 111, 126, 128, 160, 166, 180, 180, 182, 196, 208, 210, 218, 220, 253, 269, 275, 283, 305,
                307, 318, 331, 331, 332, 336, 371, 384, 390, 428, 433, 441, 446, 448, 477, 478, 478, 490, 500, 501, 503, 535, 550, 564, 574,
                580, 603, 629, 635, 678, 694, 722, 733, 744, 770, 845, 853, 916, 1035, 1036, 1037, 1047, 1098, 1106, 1164, 1186, 1190, 1192,
                1230, 1266, 1267, 1292, 1313, 1327, 1375, 1457, 1551, 1587, 1692, 1701, 1827, 1977, 2308, 2504, 2556, 2569, 2591, 2604,
                2754, 2899, 3262, 3325, 3329
            };

            public static readonly double[] X1 =
            {
                1, 13, 16, 57, 58, 66, 67, 77, 117, 122, 130, 140, 146, 152, 157, 165, 168, 234, 238, 241, 246, 255, 274, 300, 346, 348,
                367, 399, 410, 412, 416, 431, 441, 457, 465, 472, 473, 517, 537, 537, 549, 626, 629, 658, 660, 662, 679, 689, 692, 702, 713,
                749, 841, 843, 868, 871, 871, 878, 880, 905, 911, 918, 922, 925, 960, 1007, 1018, 1037, 1071, 1098, 1126, 1128, 1153, 1200,
                1205, 1216, 1242, 1267, 1298, 1388, 1444, 1496, 1547, 1633, 1650, 1714, 1725, 1820, 1839, 1886, 1931, 1990, 2098, 2155,
                2215, 2388, 2468, 2579, 2611, 3071
            };

            public static readonly double[] X2 =
            {
                9, 29, 52, 57, 60, 61, 65, 73, 78, 83, 90, 96, 131, 133, 155, 162, 165, 167, 170, 181, 181, 183, 183, 221, 241, 245, 263,
                279, 290, 291, 368, 370, 373, 375, 377, 398, 413, 420, 463, 464, 468, 560, 565, 568, 575, 578, 579, 590, 649, 653, 653, 665,
                683, 690, 702, 738, 742, 764, 766, 780, 784, 790, 834, 849, 886, 903, 932, 976, 987, 1022, 1040, 1045, 1087, 1137, 1139,
                1212, 1240, 1283, 1399, 1430, 1499, 1523, 1573, 1629, 1645, 1674, 1702, 1743, 1811, 1821, 1828, 1916, 2074, 2091, 2278,
                2316, 2390, 2490, 2531, 3642
            };
        }

        /// <summary>
        /// Data set based on beta-distribution (1, 10) multiplied by 10000 with additional outliers
        /// </summary>
        protected static class ModifiedBetaDataSet
        {
            public static readonly double[] Upper1 =
            {
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612,
                6000
            };

            public static readonly double[] Upper2 =
            {
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612,
                6000, 6001
            };

            public static readonly double[] Upper3 =
            {
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612,
                6000, 6001, 6002
            };
            
            public static readonly double[] Lower1 =
            {
                -2000,
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612
            };

            public static readonly double[] Lower2 =
            {
                -2001, -2000,
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612
            };

            public static readonly double[] Lower3 =
            {
                -2002, -2001, -2000,
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612
            };
            
             public static readonly double[] Both0 =
            {
                -2000,
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612,
                6000
            };

            public static readonly double[] Both1 =
            {
                -2001, -2000,
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612,
                6000, 6001
            };

            public static readonly double[] Both2 =
            {
                -2002, -2001, -2000,
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612,
                6000, 6001, 6002
            };
        }

        protected static class CornerCaseDataSet
        {
            public static readonly double[] Empty = new double[0];
            public static readonly double[] Same = {0, 0, 0, 0, 0};
        } 

        protected class TestData
        {
            public double[] Values { get; }
            public double[] ExpectedOutliers { get; }

            public TestData(double[] values, double[] expectedOutliers)
            {
                Values = values;
                ExpectedOutliers = expectedOutliers;
            }

            public void Deconstruct(out double[] values, out double[] expectedOutliers)
            {
                values = Values;
                expectedOutliers = ExpectedOutliers;
            }
        }

        protected void Check(TestData testData, Func<IReadOnlyList<double>, IOutlierDetector> createOutlierDetector)
        {
            var (values, expectedOutliers) = testData;
            var outlierDetector = createOutlierDetector(values);
            var actualOutliers = outlierDetector.AllOutliers(values).ToList();

            void Dump(string name, IReadOnlyList<double> data) => Output.WriteLine("{0}: [{1}]",
                name.PadRight(17), string.Join(", ", data.Select(v => v.ToString(TestCultureInfo.Instance))));

            Dump("Values", values);
            Dump("ExpectedOutliers", expectedOutliers);
            Dump("ActualOutliers", actualOutliers);

            Assert.Equal(expectedOutliers, actualOutliers);
        }
    }
}
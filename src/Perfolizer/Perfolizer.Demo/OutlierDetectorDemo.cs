using System;
using System.Collections.Generic;
using System.Linq;
using Perfolizer.Mathematics.OutlierDetection;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.ScaleEstimators;

namespace Perfolizer.Demo
{
    public class OutlierDetectorDemo : IDemo
    {
        public void Run()
        {
            var simple = SimpleQuantileEstimator.Instance;
            var hd = HarrellDavisQuantileEstimator.Instance;
            var simpleMadEstimator = MedianAbsoluteDeviationEstimator.Simple;
            var hdMadEstimator = MedianAbsoluteDeviationEstimator.HarrellDavis;

            var detectors = new[]
            {
                new Detector("TukeySimple", values => TukeyOutlierDetector.Create(values, quantileEstimator: simple)),
                new Detector("TukeyHd", values => TukeyOutlierDetector.Create(values, quantileEstimator: hd)),
                new Detector("MadSimple", values => MadOutlierDetector.Create(values, simpleMadEstimator)),
                new Detector("MadHd", values => MadOutlierDetector.Create(values, hdMadEstimator)),
                new Detector("DoubleMadSimple", values => DoubleMadOutlierDetector.Create(values, simpleMadEstimator)),
                new Detector("DoubleMadHd", values => DoubleMadOutlierDetector.Create(values, hdMadEstimator))
            };

            Case.DumpBaseSample();
            BuildTable(detectors, new[] { new Case(1, 0), new Case(2, 0), new Case(3, 0) });
            BuildTable(detectors, new[] { new Case(0, 1), new Case(0, 2), new Case(0, 3) });
            BuildTable(detectors, new[] { new Case(1, 1), new Case(2, 2), new Case(3, 3) });
        }

        private void BuildTable(Detector[] detectors, Case[] cases)
        {
            var result = new string[detectors.Length + 1, cases.Length + 1];
            result[0, 0] = "";
            for (int i = 0; i < detectors.Length; i++)
                result[i + 1, 0] = detectors[i].Name;
            for (int j = 0; j < cases.Length; j++)
                result[0, j + 1] = cases[j].Name;

            for (int i = 0; i < detectors.Length; i++)
            for (int j = 0; j < cases.Length; j++)
            {
                var outliers = detectors[i].Create(cases[j].Values).GetAllOutliers(cases[j].Values);
                result[i + 1, j + 1] = string.Join(",", outliers);
            }

            for (int j = 0; j < cases.Length + 1; j++)
            {
                int maxWith = 0;
                for (int i = 0; i < detectors.Length + 1; i++)
                    maxWith = Math.Max(maxWith, result[i, j].Length);
                for (int i = 0; i < detectors.Length + 1; i++)
                    result[i, j] = result[i, j].PadRight(maxWith + 1);
            }

            for (int j = 0; j < cases.Length; j++)
                cases[j].Dump();
            Console.WriteLine();

            for (int i = 0; i < detectors.Length + 1; i++)
            {
                for (int j = 0; j < cases.Length + 1; j++)
                    Console.Write(result[i, j]);
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private class Case
        {
            private static readonly double[] BaseSample =
            {
                9, 47, 50, 71, 78, 79, 97, 98, 117, 123, 136, 138, 143, 145, 167, 185, 202, 216, 217, 229, 235, 242, 257, 297, 300, 315,
                344, 347, 347, 360, 362, 368, 387, 400, 428, 455, 468, 484, 493, 523, 557, 574, 586, 605, 617, 618, 634, 641, 646, 649, 674,
                678, 689, 699, 703, 709, 714, 740, 795, 798, 839, 880, 938, 941, 983, 1014, 1021, 1022, 1165, 1183, 1195, 1250, 1254, 1288,
                1292, 1326, 1362, 1363, 1421, 1549, 1585, 1605, 1629, 1694, 1695, 1719, 1799, 1827, 1828, 1862, 1991, 2140, 2186, 2255,
                2266, 2295, 2321, 2419, 2919, 3612
            };

            private static readonly double[] LowerOutliers = { -2002, -2001, -2000 };
            private static readonly double[] UpperOutliers = { 6000, 6001, 6002 };

            public string Name { get; }
            public IReadOnlyList<double> Values { get; }

            private int LowerOutlierCount { get; }
            private int UpperOutlierCount { get; }

            public Case(int lowerOutlierCount, int upperOutlierCount)
            {
                LowerOutlierCount = lowerOutlierCount;
                UpperOutlierCount = upperOutlierCount;

                if (lowerOutlierCount > 0 && upperOutlierCount > 0 && lowerOutlierCount == upperOutlierCount)
                    Name = "Both" + lowerOutlierCount;
                else if (lowerOutlierCount == 0 && upperOutlierCount > 0)
                    Name = "Upper" + upperOutlierCount;
                else if (lowerOutlierCount > 0 && upperOutlierCount == 0)
                    Name = "Lower" + lowerOutlierCount;
                else
                    throw new ArgumentOutOfRangeException();
                Values = LowerOutliers.TakeLast(lowerOutlierCount)
                    .Concat(BaseSample)
                    .Concat(UpperOutliers.Take(upperOutlierCount))
                    .ToList();
            }

            public void Dump()
            {
                Console.WriteLine("{0} = {{{1}BaseSample{2}}}",
                    Name,
                    string.Join("", LowerOutliers.TakeLast(LowerOutlierCount).Select(o => o + ", ")),
                    string.Join("", UpperOutliers.Take(UpperOutlierCount).Select(o => ", " + o)));
            }

            public static void DumpBaseSample()
            {
                Console.WriteLine("BaseSample = {");
                for (int i = 0; i < BaseSample.Length; i++)
                {
                    if (i % 10 == 0)
                        Console.Write("  ");
                    Console.Write(BaseSample[i]);
                    if (i != BaseSample.Length - 1)
                    {
                        Console.Write(", ");
                        if ((i + 1) % 10 == 0)
                            Console.WriteLine();
                    }
                    else
                        Console.WriteLine("}");
                }

                Console.WriteLine();
            }
        }

        private class Detector
        {
            public string Name { get; }
            public Func<IReadOnlyList<double>, IOutlierDetector> Create { get; }

            public Detector(string name, Func<IReadOnlyList<double>, IOutlierDetector> create)
            {
                Name = name;
                Create = create;
            }
        }
    }
}
using System;
using CommandLine;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Mathematics.RangeEstimators;

namespace Perfolizer.Tool.Base
{
    public class DistributionCompareOptions : ArrayOptions
    {
        [Option("data1", Group = "source1",
            HelpText = "First source data array")]
        public string Data1 { get; set; }

        [Option("file1", Group = "source1",
            HelpText = "File name with the first source data array")]
        public string FileName1 { get; set; }

        [Option("data2", Group = "source2",
            HelpText = "Second source data array")]
        public string Data2 { get; set; }

        [Option("file2", Group = "source2",
            HelpText = "File name with the second source data array")]
        public string FileName2 { get; set; }

        [Option('p', "probs",
            HelpText = "Probabilities that defines quantile to evaluate. If not specified, the range estimation will be returned")]
        public string Probabilities { get; set; }

        [Option('m', "margin",
            HelpText = "Margin (0.0-0.5) for range estimation")]
        public double? Margin { get; set; }

        [NotNull]
        public double[] GetSourceArray1() => GetArrayFromDataOrFile(Data1, FileName1, "first source array");

        [NotNull]
        public double[] GetSourceArray2() => GetArrayFromDataOrFile(Data2, FileName2, "second source array");

        protected static void Run(DistributionCompareOptions options, DistributionCompareFunction function)
        {
            var x = options.GetSourceArray1();
            var y = options.GetSourceArray2();
            if (string.IsNullOrEmpty(options.Probabilities))
            {
                var rangeEstimator = new DistributionCompareRangeEstimator(function);
                if (options.Margin.HasValue)
                    Console.WriteLine(rangeEstimator.GetRange(x, y, options.Margin.Value).ToString());
                else
                    Console.WriteLine(rangeEstimator.GetRange(x, y).ToString());
            }
            else
            {
                var probabilities = options.ConvertStringToArray(options.Probabilities, "probabilities");
                Console.WriteLine(string.Join(";", function.Values(x, y, probabilities)));
            }
        }
    }
}
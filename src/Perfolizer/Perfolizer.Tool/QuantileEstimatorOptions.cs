using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Tool.Base;

namespace Perfolizer.Tool
{
    [Verb("qe", HelpText = "Estimate quantiles")]
    public class QuantileEstimatorOptions : SourceArrayOptions
    {
        public enum QuantileEstimatorAlgorithm
        {
            Simple,
            Hd
        }

        [Option('a', "algo",
            HelpText = "Quantile Estimator algorithms",
            Default = QuantileEstimatorAlgorithm.Hd)]
        public QuantileEstimatorAlgorithm Algorithm { get; set; }

        [Option('p', "probs", Required = true,
            HelpText = "Probabilities that specify locations of requested quantiles")]
        public string Probabilities { get; set; }
        
        [Usage(ApplicationAlias = KnowStrings.ApplicationAlias)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Estimate quantiles", new QuantileEstimatorOptions
                {
                    Data = "'0;50;100'",
                    Probabilities = "'0;0.25;0.5;0.75;1'"
                });
            }
        }

        public static void Run(QuantileEstimatorOptions options)
        {
            var estimator = options.Algorithm switch
            {
                QuantileEstimatorAlgorithm.Simple => SimpleQuantileEstimator.Instance,
                QuantileEstimatorAlgorithm.Hd => HarrellDavisQuantileEstimator.Instance,
                _ => throw new ArgumentOutOfRangeException(nameof(options.Algorithm))
            };

            var data = options.GetSourceArray();
            var probabilities = Probability.ToProbabilities(options.ConvertStringToArray(options.Probabilities, "probabilities"));

            var quantiles = estimator.GetQuantiles(data, probabilities);
            Console.WriteLine(string.Join(options.SourceSeparator, quantiles));
        }
    }
}
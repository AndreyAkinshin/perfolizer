using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using CommandLine.Text;
using Perfolizer.Mathematics.Histograms;

namespace Perfolizer.Tool
{
    [Verb("hist", HelpText = "Detect changepoints")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class HistogramBuilderOptions : SourceArrayOptions
    {
        public enum HistogramBuilderAlgorithm
        {
            Simple,
            Adaptive
        }

        [Option('a', "algo",
            HelpText = "Algorithms that builds a histogram",
            Default = HistogramBuilderAlgorithm.Adaptive)]
        public HistogramBuilderAlgorithm Algorithm { get; set; }

        [Option('b', "bin-size",
            HelpText = "Recommended bin size",
            Default = null)]
        public double? BinSize { get; set; }

        [Usage(ApplicationAlias = KnowStrings.ApplicationAlias)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Build a histogram from a small array", new HistogramBuilderOptions
                {
                    Data = "'0;0;0;1;2;3;54;234;234;24;12;21;3;123;3;35;134'"
                });
            }
        }

        public static void Run(HistogramBuilderOptions options)
        {
            var builder = options.Algorithm switch
            {
                HistogramBuilderAlgorithm.Simple => SimpleHistogramBuilder.Instance,
                HistogramBuilderAlgorithm.Adaptive => AdaptiveHistogramBuilder.Instance,
                _ => throw new ArgumentOutOfRangeException(nameof(options.Algorithm))
            };

            var data = options.GetSourceArray();

            var histogram = options.BinSize == null
                ? builder.Build(data)
                : builder.Build(data, options.BinSize.Value);
            Console.WriteLine(histogram.ToString());
        }
    }
}
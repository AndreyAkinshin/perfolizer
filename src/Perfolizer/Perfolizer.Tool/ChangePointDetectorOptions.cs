using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using Perfolizer.Mathematics.Cpd;

namespace Perfolizer.Tool
{
    [Verb("cpd", HelpText = "Detect changepoints")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ChangePointDetectorOptions : SourceArrayOptions
    {
        public enum ChangePointDetectorAlgorithm
        {
            EdPelt,
            RqqPelt
        }

        [Option('a', "algo",
            HelpText = "Algorithms that detects changepoints",
            Default = ChangePointDetectorAlgorithm.RqqPelt)]
        public ChangePointDetectorAlgorithm Algorithm { get; set; }

        [Option("dist", HelpText = "Minimum distance between changepoints", Default = 20)]
        public int MinDistance { get; set; }

        [Usage(ApplicationAlias = KnowStrings.ApplicationAlias)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Find changepoints in a small array", new ChangePointDetectorOptions
                {
                    Data = "'0;0;0;0;100;100;100;100;100'",
                    MinDistance = 1
                });
            }
        }

        public static void Run(ChangePointDetectorOptions options)
        {
            PeltChangePointDetector detector = options.Algorithm switch
            {
                ChangePointDetectorAlgorithm.EdPelt => EdPeltChangePointDetector.Instance,
                ChangePointDetectorAlgorithm.RqqPelt => RqqPeltChangePointDetector.Instance,
                _ => throw new ArgumentOutOfRangeException(nameof(options.Algorithm))
            };

            var data = options.GetSourceArray();

            var indexes = detector.GetChangePointIndexes(data, options.MinDistance);
            Console.WriteLine(indexes.Any()
                ? string.Join(options.SourceSeparator, indexes)
                : "No changepoints found");
        }
    }
}
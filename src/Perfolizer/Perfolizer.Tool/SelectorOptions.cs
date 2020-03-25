using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using CommandLine.Text;
using Perfolizer.Mathematics.Selectors;
using Perfolizer.Tool.Base;

namespace Perfolizer.Tool
{
    [Verb("select", HelpText = "Selects k-th smallest element in the array")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SelectorOptions : SourceArrayOptions
    {
        public enum SelectorAlgorithm
        {
            Adaptive,
            Simple,
            Rqq
        }

        [Option('a', "algo",
            HelpText = "Algorithms that builds a histogram",
            Default = SelectorAlgorithm.Adaptive)]
        public SelectorAlgorithm Algorithm { get; set; }

        [Option('k', HelpText = "Index of the number to search (zero-based)", Required = true)]
        public int K { get; set; }

        [Option('l', "left", HelpText = "Left index of the target subarray (zero-based)")]
        public int? Left { get; set; }

        [Option('r', "right", HelpText = "Right index of the target subarray (zero-based)")]
        public int? Right { get; set; }

        [Usage(ApplicationAlias = KnowStrings.ApplicationAlias)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Select 3-th smallest elements (zero-based)", new SelectorOptions
                {
                    Data = "'6;2;0;7;9;3;1;8;5;4'",
                    K = 3
                });
            }
        }

        public static void Run(SelectorOptions options)
        {
            var data = options.GetSourceArray();

            int k = options.K;
            int left = options.Left ?? 0;
            int right = options.Right ?? data.Length - 1;
            switch (options.Algorithm)
            {
                case SelectorAlgorithm.Adaptive:
                    Console.WriteLine(new QuickSelectAdaptive().Select(data, k, left, right));
                    break;
                case SelectorAlgorithm.Simple:
                    Console.WriteLine(new SimpleSelector(data).Select(left, right, k));
                    break;
                case SelectorAlgorithm.Rqq:
                    Console.WriteLine(new Rqq(data).Select(left, right, k));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options.Algorithm));
            }
        }
    }
}
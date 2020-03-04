using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using CommandLine.Text;
using Perfolizer.Mathematics.Multimodality;

namespace Perfolizer.Tool
{
    [Verb("mvalue", HelpText = "Calculates mvalue")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class MValueCalculatorOptions : SourceArrayOptions
    {
        [Usage(ApplicationAlias = KnowStrings.ApplicationAlias)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Perfect unimodal case (mvalue = 2)", new MValueCalculatorOptions
                {
                    Data = "'0;0;0;0;0;0;0;0'"
                });
                
                yield return new Example("Perfect bimodal case (mvalue = 4)", new MValueCalculatorOptions
                {
                    Data = "'0;0;0;0;1;1;1;1'"
                });
            }
        }

        public static void Run(MValueCalculatorOptions options)
        {
            var data = options.GetSourceArray();
            Console.WriteLine(MValueCalculator.Calculate(data));
        }
    }
}
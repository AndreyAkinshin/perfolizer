using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Tool.Base;

namespace Perfolizer.Tool
{
    [Verb("ratio", HelpText = "Estimate ratio range or ratio function values")]
    public class RatioFunctionOptions : DistributionCompareOptions
    {
        public static void Run(RatioFunctionOptions options) => Run(options, RatioFunction.Instance);
        
        [Usage(ApplicationAlias = KnowStrings.ApplicationAlias)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Equal distributions", new RatioFunctionOptions
                {
                    Data1 = "'2;2;2;2;2;4;4;4;4;4'",
                    Data2 = "'2;2;2;2;2;4;4;4;4;4'",
                    Margin = 0
                });
                yield return new Example("Pure ratio", new RatioFunctionOptions
                {
                    Data1 = "'2;2;2;2;2;4;4;4;4;4'",
                    Data2 = "'4;4;4;4;4;8;8;8;8;8'",
                    Margin = 0
                });
                yield return new Example("Single mode shift in a bimodal distribution", new RatioFunctionOptions
                {
                    Data1 = "'2;2;2;2;2;4;4;4;4;4'",
                    Data2 = "'2;2;2;2;2;8;8;8;8;8'",
                    Margin = 0
                });
                yield return new Example("Single mode shift in a bimodal distribution", new RatioFunctionOptions
                {
                    Data1 = "'2;2;2;2;2;4;4;4;4;4'",
                    Data2 = "'1;1;1;1;1;8;8;8;8;8'",
                    Margin = 0
                });
            }
        }
    }
}
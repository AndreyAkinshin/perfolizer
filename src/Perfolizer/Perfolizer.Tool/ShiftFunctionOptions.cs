using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using Perfolizer.Mathematics.Functions;
using Perfolizer.Tool.Base;

namespace Perfolizer.Tool
{
    [Verb("shift", HelpText = "Estimate shift range or shift function values")]
    public class ShiftFunctionOptions : DistributionCompareOptions
    {
        [Usage(ApplicationAlias = KnowStrings.ApplicationAlias)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Equal distributions", new ShiftFunctionOptions
                {
                    Data1 = "'2;2;2;2;2;4;4;4;4;4'",
                    Data2 = "'2;2;2;2;2;4;4;4;4;4'",
                    Margin = 0
                });
                yield return new Example("Pure shift", new ShiftFunctionOptions
                {
                    Data1 = "'2;2;2;2;2;4;4;4;4;4'",
                    Data2 = "'4;4;4;4;4;6;6;6;6;6'",
                    Margin = 0
                });
                yield return new Example("Single mode shift in a bimodal distribution", new ShiftFunctionOptions
                {
                    Data1 = "'2;2;2;2;2;4;4;4;4;4'",
                    Data2 = "'2;2;2;2;2;8;8;8;8;8'",
                    Margin = 0
                });
                yield return new Example("Single mode shift in a bimodal distribution", new ShiftFunctionOptions
                {
                    Data1 = "'2;2;2;2;2;4;4;4;4;4'",
                    Data2 = "'1;1;1;1;1;8;8;8;8;8'",
                    Margin = 0
                });
                yield return new Example("Shift function values for given probabilities", new ShiftFunctionOptions
                {
                    Data1 = "'10;20;30;40;50'",
                    Data2 = "'11;22;33;44;55'",
                    Probabilities = "'0.0;0.5;1.0'"
                });
            }
        }
        
        public static void Run(ShiftFunctionOptions options) => Run(options, ShiftFunction.Instance);
    }
}
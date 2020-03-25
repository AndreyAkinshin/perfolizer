using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using CommandLine.Text;
using Perfolizer.Mathematics.Selectors;
using Perfolizer.Tool.Base;

namespace Perfolizer.Tool
{
    [Verb("rqq", HelpText = "Draw Rqq ASCII tree")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class RqqOptions : SourceArrayOptions
    {
        [Usage(ApplicationAlias = KnowStrings.ApplicationAlias)]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Draw RQQ tree", new RqqOptions
                {
                    Data = "'6;2;0;7;9;3;1;8;5;4'"
                });
            }
        }

        public static void Run(RqqOptions options)
        {
            var data = options.GetSourceArray();
            Console.WriteLine(new Rqq(data).DumpTreeAscii());
        }
    }
}
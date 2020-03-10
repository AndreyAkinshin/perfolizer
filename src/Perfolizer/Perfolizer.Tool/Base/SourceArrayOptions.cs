using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace Perfolizer.Tool.Base
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public class SourceArrayOptions : ArrayOptions
    {
        [Option('d', "data", Group = "source",
            HelpText = "Source data array")]
        public string Data { get; set; }

        [Option('f', "file", Group = "source",
            HelpText = "File name with source data array")]
        public string FileName { get; set; }

        [JetBrains.Annotations.NotNull]
        public double[] GetSourceArray() => GetArrayFromDataOrFile(Data, FileName, "source array");
    }
}
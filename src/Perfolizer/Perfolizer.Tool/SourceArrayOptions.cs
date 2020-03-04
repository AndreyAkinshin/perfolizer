using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CommandLine;

namespace Perfolizer.Tool
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public class SourceArrayOptions
    {
        [Option("sep", Default = ";",
            HelpText = "Separator for data array elements for 'data' and 'file' commands")]
        public string SourceSeparator { get; set; }

        [Option('d', "data", Group = "source",
            HelpText = "Source data array")]
        public string Data { get; set; }

        [Option('f', "file", Group = "source",
            HelpText = "File name with source data array")]
        public string FileName { get; set; }

        [JetBrains.Annotations.NotNull]
        public double[] GetSourceArray()
        {
            string input;
            if (!string.IsNullOrEmpty(Data))
                input = Data;
            else if (!string.IsNullOrEmpty(FileName))
                input = File.ReadAllText(FileName);
            else
                throw new ArgumentException("'data' or 'file' options should be specified");

            var numbers = input.Trim('"', '\'', ' ').Split(SourceSeparator);
            var data = new double[numbers.Length];
            for (int i = 0; i < data.Length; i++)
            {
                if (double.TryParse(numbers[i].Trim(), out double number))
                    data[i] = number;
                else
                    throw new InvalidOperationException($"'{numbers[i]}' (element #{i} in the source array) is not a valid number");
            }

            return data;
        }
    }
}
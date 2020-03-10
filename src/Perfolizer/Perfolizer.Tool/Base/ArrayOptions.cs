using System;
using System.IO;
using CommandLine;

namespace Perfolizer.Tool.Base
{
    public class ArrayOptions
    {
        [Option("sep", Default = ";",
            HelpText = "Separator for data array elements for 'data' and 'file' commands")]
        public string SourceSeparator { get; set; }

        protected double[] GetArrayFromDataOrFile(string data, string fileName, string arrayName)
        {
            string input;
            if (!string.IsNullOrEmpty(data))
                input = data;
            else if (!string.IsNullOrEmpty(fileName))
                input = File.ReadAllText(fileName);
            else
                throw new ArgumentException("'data' or 'file' options should be specified");

            return ConvertStringToArray(input, arrayName);
        }

        public double[] ConvertStringToArray(string input, string arrayName)
        {
            var numbers = input.Trim('"', '\'', ' ').Split(SourceSeparator);
            var data = new double[numbers.Length];
            for (int i = 0; i < data.Length; i++)
            {
                if (double.TryParse(numbers[i].Trim(), out double number))
                    data[i] = number;
                else
                    throw new InvalidOperationException($"'{numbers[i]}' (element #{i} in the {arrayName}) is not a valid number");
            }

            return data;
        }
    }
}
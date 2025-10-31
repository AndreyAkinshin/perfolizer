using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Perfolizer.Horology;
using Perfolizer.Models;
using Pragmastat.Metrology;

namespace Perfolizer.Helpers;

public static class CpuBrandHelper
{
    public static string ToFullBrandName(this CpuInfo? cpu)
    {
        if (cpu == null)
            return "Unknown processor";

        var parts = new List<string> { cpu.ToShortBrandName(includeMaxFrequency: true) };

        if (cpu.PhysicalProcessorCount > 0)
            parts.Add($", {cpu.PhysicalProcessorCount} CPU");

        switch (cpu.LogicalCoreCount)
        {
            case 1:
                parts.Add(", 1 logical core");
                break;
            case > 1:
                parts.Add($", {cpu.LogicalCoreCount} logical cores");
                break;
        }

        if (cpu.LogicalCoreCount > 0 && cpu.PhysicalCoreCount > 0)
            parts.Add(" and ");
        else if (cpu.PhysicalCoreCount > 0)
            parts.Add(", ");

        switch (cpu.PhysicalCoreCount)
        {
            case 1:
                parts.Add("1 physical core");
                break;
            case > 1:
                parts.Add($"{cpu.PhysicalCoreCount} physical cores");
                break;
        }

        string result = string.Join("", parts);
        // The line with ProcessorBrandString is one of the longest lines in the summary.
        // When people past in on GitHub, it can be a reason of an ugly horizontal scrollbar.
        // To avoid this, we are trying to minimize this line and use the minimum possible number of characters.
        // Here we are removing the repetitive "cores" word.
        if (result.Contains("logical cores") && result.Contains("physical cores"))
            result = result.Replace("logical cores", "logical");

        return result;
    }


    /// <summary>
    /// Transform a processor brand string to a nice form for summary.
    /// </summary>
    /// <param name="cpu">The CPU information</param>
    /// <param name="includeMaxFrequency">Whether to include determined max frequency information</param>
    /// <returns>Prettified version</returns>
    public static string ToShortBrandName(this CpuInfo? cpu, bool includeMaxFrequency = false)
    {
        if (cpu == null || string.IsNullOrEmpty(cpu.ProcessorName))
            return "Unknown processor";

        // Remove parts which don't provide any useful information for user
        var processorName = cpu.ProcessorName!.Replace("@", "").Replace("(R)", "").Replace("(TM)", "");

        // If we have found physical core(s), we can safely assume we can drop extra info from brand
        if (cpu.PhysicalCoreCount.HasValue && cpu.PhysicalCoreCount.Value > 0)
            processorName = Regex.Replace(processorName, @"(\w+?-Core Processor)", "").Trim();

        string? frequencyString = GetBrandStyledActualFrequency(cpu.NominalFrequency());
        if (includeMaxFrequency && frequencyString != null && !processorName.Contains(frequencyString))
        {
            // show Max only if there's already a frequency name to differentiate the two
            string maxFrequency = processorName.Contains("Hz") ? $"(Max: {frequencyString})" : frequencyString;
            processorName = $"{processorName} {maxFrequency}";
        }

        // Remove double spaces
        processorName = Regex.Replace(processorName.Trim(), @"\s+", " ");

        // Add microarchitecture name if known
        string? microarchitecture = ParseMicroarchitecture(processorName);
        if (microarchitecture != null)
            processorName = $"{processorName} ({microarchitecture})";

        return processorName;
    }

    /// <summary>
    /// Presents actual processor's frequency into brand string format
    /// </summary>
    /// <param name="frequency"></param>
    private static string? GetBrandStyledActualFrequency(Frequency? frequency) => frequency == null
        ? null
        : $"{MeasurementFormatter.Default.Format(frequency.Value.ToMeasurement(FrequencyUnit.GHz), "N2")}";

    /// <summary>
    /// Parse a processor name and tries to return a microarchitecture name.
    /// Works only for well-known microarchitectures.
    /// </summary>
    private static string? ParseMicroarchitecture(string processorName)
    {
        if (processorName.StartsWith("Intel Core"))
        {
            string model = processorName.Substring("Intel Core".Length).Trim();

            // Core i3/5/7/9
            if (
                model.Length > 4 &&
                model[0] == 'i' &&
                (model[1] == '3' || model[1] == '5' || model[1] == '7' || model[1] == '9') &&
                (model[2] == '-' || model[2] == ' '))
            {
                string modelNumber = model.Substring(3);
                if (modelNumber.StartsWith("CPU"))
                    modelNumber = modelNumber.Substring(3).Trim();
                if (modelNumber.Contains("CPU"))
                    modelNumber = modelNumber.Substring(0, modelNumber.IndexOf("CPU", StringComparison.Ordinal)).Trim();
                return ParseIntelCoreMicroarchitecture(modelNumber);
            }
        }

        return null;
    }

    private static readonly Lazy<Dictionary<string, string>> KnownMicroarchitectures = new(() =>
    {
        var data = ResourceHelper.LoadResource("Perfolizer.Resources.microarchitectures.txt").Split('\r', '\n');
        var dictionary = new Dictionary<string, string>();
        string? currentMicroarchitecture = null;
        foreach (string line in data)
        {
            if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line))
                continue;
            if (line.StartsWith("#"))
            {
                currentMicroarchitecture = line.Substring(1).Trim();
                continue;
            }

            string modelNumber = line.Trim();
            if (dictionary.ContainsKey(modelNumber))
                throw new Exception($"{modelNumber} is defined twice in microarchitectures.txt");
            if (currentMicroarchitecture == null)
                throw new Exception($"{modelNumber} doesn't have defined microarchitecture in microarchitectures.txt");
            dictionary[modelNumber] = currentMicroarchitecture;
        }

        return dictionary;
    });

    // see http://www.intel.com/content/www/us/en/processors/processor-numbers.html
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    internal static string? ParseIntelCoreMicroarchitecture(string modelNumber)
    {
        if (KnownMicroarchitectures.Value.TryGetValue(modelNumber, out string? microarchitecture))
            return microarchitecture;

        if (modelNumber.Length >= 3 && modelNumber.Substring(0, 3).All(char.IsDigit) &&
            (modelNumber.Length == 3 || !char.IsDigit(modelNumber[3])))
            return "Nehalem";
        if (modelNumber.Length >= 4 && modelNumber.Substring(0, 4).All(char.IsDigit))
        {
            char generation = modelNumber[0];
            switch (generation)
            {
                case '2':
                    return "Sandy Bridge";
                case '3':
                    return "Ivy Bridge";
                case '4':
                    return "Haswell";
                case '5':
                    return "Broadwell";
                case '6':
                    return "Skylake";
                case '7':
                    return "Kaby Lake";
                case '8':
                    return "Coffee Lake";
                default:
                    return null;
            }
        }
        return null;
    }
}
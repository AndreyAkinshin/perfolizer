using Perfolizer.Helpers;
using Perfolizer.Horology;
using Perfolizer.Mathematics.Common;
using Perfolizer.Phd.Dto;

namespace Perfolizer.Tests.Helpers;

// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo
public class ProcessorBrandStringTests
{
    [Theory]
    [InlineData("Intel(R) Pentium(TM) G4560 CPU @ 3.50GHz", "Intel Pentium G4560 CPU 3.50GHz")]
    [InlineData("Intel(R) Core(TM) i7 CPU 970 @ 3.20GHz", "Intel Core i7 CPU 970 3.20GHz (Nehalem)")] // Nehalem/Westmere/Gulftown
    [InlineData("Intel(R) Core(TM) i7-920 CPU @ 2.67GHz", "Intel Core i7-920 CPU 2.67GHz (Nehalem)")]
    [InlineData("Intel(R) Core(TM) i7-2600 CPU @ 3.40GHz", "Intel Core i7-2600 CPU 3.40GHz (Sandy Bridge)")]
    [InlineData("Intel(R) Core(TM) i7-3770 CPU @ 3.40GHz", "Intel Core i7-3770 CPU 3.40GHz (Ivy Bridge)")]
    [InlineData("Intel(R) Core(TM) i7-4770K CPU @ 3.50GHz", "Intel Core i7-4770K CPU 3.50GHz (Haswell)")]
    [InlineData("Intel(R) Core(TM) i7-5775R CPU @ 3.30GHz", "Intel Core i7-5775R CPU 3.30GHz (Broadwell)")]
    [InlineData("Intel(R) Core(TM) i7-6700HQ CPU @ 2.60GHz", "Intel Core i7-6700HQ CPU 2.60GHz (Skylake)")]
    [InlineData("Intel(R) Core(TM) i7-7700 CPU @ 3.60GHz", "Intel Core i7-7700 CPU 3.60GHz (Kaby Lake)")]
    [InlineData("Intel(R) Core(TM) i7-8650U CPU @ 1.90GHz ", "Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R)")]
    [InlineData("Intel(R) Core(TM) i7-8700K CPU @ 3.70GHz", "Intel Core i7-8700K CPU 3.70GHz (Coffee Lake)")]
    public void IntelCoreIsPrettified(string processorName, string cpuBrandName) =>
        Assert.Equal(cpuBrandName, new PhdCpu { ProcessorName = processorName }.ToShortBrandName());

    [Theory]
    [InlineData("Intel(R) Pentium(TM) G4560 CPU @ 3.50GHz", "Intel Pentium G4560 CPU 3.50GHz (Max: 3.70GHz)", 3.7)]
    [InlineData("Intel(R) Core(TM) i5-2500 CPU @ 3.30GHz", "Intel Core i5-2500 CPU 3.30GHz (Max: 3.70GHz) (Sandy Bridge)", 3.7)]
    [InlineData("Intel(R) Core(TM) i7-2600 CPU @ 3.40GHz", "Intel Core i7-2600 CPU 3.40GHz (Max: 3.70GHz) (Sandy Bridge)", 3.7)]
    [InlineData("Intel(R) Core(TM) i7-3770 CPU @ 3.40GHz", "Intel Core i7-3770 CPU 3.40GHz (Max: 3.50GHz) (Ivy Bridge)", 3.5)]
    [InlineData("Intel(R) Core(TM) i7-4770K CPU @ 3.50GHz", "Intel Core i7-4770K CPU 3.50GHz (Max: 3.60GHz) (Haswell)", 3.6)]
    [InlineData("Intel(R) Core(TM) i7-5775R CPU @ 3.30GHz", "Intel Core i7-5775R CPU 3.30GHz (Max: 3.40GHz) (Broadwell)", 3.4)]
    public void CoreIsPrettifiedWithDiffFrequencies(string processorName, string brandName, double nominalFrequency)
    {
        var cpu = new PhdCpu
        {
            ProcessorName = processorName,
            NominalFrequencyHz = Frequency.FromGHz(nominalFrequency).Hertz.RoundToLong()
        };
        Assert.Equal(brandName, cpu.ToShortBrandName(includeMaxFrequency: true));
    }

    [Theory]
    [InlineData("AMD Ryzen 7 2700X Eight-Core Processor", "AMD Ryzen 7 2700X 4.10GHz", 4.1, 8, 16)]
    [InlineData("AMD Ryzen 7 2700X Eight-Core Processor", "AMD Ryzen 7 2700X Eight-Core Processor 4.10GHz", 4.1, null, null)]
    public void AmdIsPrettifiedWithDiffFrequencies(
        string processorName,
        string brandName,
        double nominalFrequency,
        int? physicalCoreCount,
        int? logicalCoreCount)
    {
        var cpu = new PhdCpu
        {
            ProcessorName = processorName,
            NominalFrequencyHz = Frequency.FromGHz(nominalFrequency).Hertz.RoundToLong(),
            PhysicalCoreCount = physicalCoreCount,
            LogicalCoreCount = logicalCoreCount
        };
        Assert.Equal(brandName, cpu.ToShortBrandName(includeMaxFrequency: true));
    }

    [Theory]
    [InlineData("8130U", "Kaby Lake")]
    [InlineData("9900K", "Coffee Lake")]
    [InlineData("8809G", "Kaby Lake G")]
    public void IntelCoreMicroarchitecture(string modelNumber, string microarchitecture)
    {
        Assert.Equal(microarchitecture, CpuBrandHelper.ParseIntelCoreMicroarchitecture(modelNumber));
    }

    [Theory]
    [InlineData("", "Unknown processor")]
    [InlineData(null, "Unknown processor")]
    public void UnknownProcessorDoesNotThrow(string? processorName, string brandName)
    {
        var cpu = new PhdCpu { ProcessorName = processorName };
        Assert.Equal(brandName, cpu.ToShortBrandName(includeMaxFrequency: true));
    }
}
using System.Text;
using Perfolizer.Helpers;
using Perfolizer.Phd.Dto;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Helpers
{
    [Collection("VerifyTests")]
    public class CpuBrandHelperTests
    {
        [Fact]
        public Task CpuBrandHelperTest01()
        {
            var captions = new StringBuilder();
            foreach (string? processorName in new[] { null, "", "Intel" })
            foreach (int? physicalProcessorCount in new int?[] { null, 0, 1, 2 })
            foreach (int? physicalCoreCount in new int?[] { null, 0, 1, 2 })
            foreach (int? logicalCoreCount in new int?[] { null, 0, 1, 2 })
            {
                var cpu = new PhdCpu
                {
                    ProcessorName = processorName,
                    PhysicalProcessorCount = physicalProcessorCount,
                    PhysicalCoreCount = physicalCoreCount,
                    LogicalCoreCount = logicalCoreCount,
                };

                captions.AppendLine(cpu.ToFullBrandName());
            }

            var settings = VerifyHelper.CreateSettings("Cpu");
            return Verifier.Verify(captions.ToString(), settings);
        }
    }
}
using JetBrains.Annotations;
using Perfolizer.Horology;
using Perfolizer.Phd;

namespace Perfolizer.Tests.Phd;

// public class PhdBdnTests : PhdTestsBase
// {
//     [Fact]
//     public Task PhdBdn()
//     {
//         PerfMetric Metric(double value, int iterationIndex) => new()
//             { Value = value, Unit = TimeUnit.Nanosecond, IterationIndex = iterationIndex, InvocationCount = 2 };
//
//         var root = new PhdAttributes
//         {
//             Info = new BdnInfo { Title = "BenchmarkDotNet.Samples.IntroExportJson-20240309-013216" },
//             Host = new BdnHost
//             {
//                 BenchmarkDotNetCaption = "BenchmarkDotNet",
//                 BenchmarkDotNetVersion = "0.13.13-develop (2024-03-09)",
//                 OsVersion = "macOS Sonoma 14.2.1 (23C71) [Darwin 23.2.0]",
//                 ProcessorName = "Apple M1 Max",
//                 PhysicalProcessorCount = "1",
//                 PhysicalCoreCount = "10",
//                 LogicalCoreCount = "10",
//                 RuntimeVersion = ".NET 8.0.0 (8.0.23.53103)",
//                 Architecture = "Arm64",
//                 HasAttachedDebugger = false,
//                 HasRyuJit = true,
//                 Configuration = "RELEASE",
//                 DotNetSdkVersion = "8.0.100",
//                 ChronometerFrequency = 1000000000,
//                 HardwareTimerKind = "Unknown"
//             }
//         }.ToPerfEntry().Add(
//             new PhdAttributes
//                 {
//                     Descriptor = new BdnDescriptor
//                     {
//                         DisplayInfo = "BenchmarkDotNet.Samples.IntroExportJson-20240309-013216",
//                         Namespace = "BenchmarkDotNet.Samples",
//                         Type = "IntroExportJson",
//                         Method = "ExportJson",
//                         MethodTitle = "ExportJson",
//                         Parameters = "Foo=1"
//                     }
//                 }.ToPerfEntry()
//                 .Add(new PhdAttributes
//                     {
//                         Lifecycle = new BdnLifecycle
//                         {
//                             IterationMode = "Pilot",
//                             IterationStage = "Overhead",
//                             LaunchIndex = 0
//                         }
//                     }.ToPerfEntry()
//                     .Add(Metric(12, 1))
//                     .Add(Metric(13, 2))
//                     .Add(Metric(14, 3)))
//                 .Add(new PhdAttributes
//                     {
//                         Lifecycle = new BdnLifecycle
//                         {
//                             IterationMode = "Result",
//                             IterationStage = "Workload",
//                             LaunchIndex = 1
//                         }
//                     }.ToPerfEntry()
//                     .Add(Metric(15, 1))
//                     .Add(Metric(16, 2))
//                     .Add(Metric(17, 3)))
//         );
//
//
//         return VerifyPhd(root, schema);
//     }
//
//
// }
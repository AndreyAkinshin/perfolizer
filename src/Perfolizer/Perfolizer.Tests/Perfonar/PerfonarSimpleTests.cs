using JetBrains.Annotations;
using Perfolizer.Horology;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Metrology;
using Perfolizer.Models;
using Perfolizer.Perfonar.Base;
using Perfolizer.Perfonar.Tables;

namespace Perfolizer.Tests.Perfonar;

public class PerfonarSimpleTests : PerfonarTestsBase
{
    private PerfonarTableConfig tableConfig = new()
    {
        ColumnDefinitions =
        [
            new PerfonarColumnDefinition(".engine") { Cloud = "primary", IsSelfExplanatory = true },
            new PerfonarColumnDefinition(".host.os") { Cloud = "primary", IsSelfExplanatory = true },
            new PerfonarColumnDefinition(".host.cpu") { Cloud = "primary", IsSelfExplanatory = true },
            new PerfonarColumnDefinition(".benchmark") { Cloud = "secondary" },
            new PerfonarColumnDefinition(".job") { Cloud = "secondary", Compressed = true },
            new PerfonarColumnDefinition("=center"),
            new PerfonarColumnDefinition("=spread")
        ]
    };

    [Fact]
    public async Task PerfonarSimpleTableTest()
    {
        var table = new PerfonarTable(CreateRoot(), tableConfig);
        string markdown = table.ToMarkdown();
        await Verify(markdown, CreateSettings());
    }

    [Fact]
    public Task PerfonarSimpleJsonTest()
    {
        var root = CreateRoot();
        var schema = new PerfonarSchema("Simple")
            .Add<SimpleHostInfo>()
            .Add<SimpleExecutionInfo>()
            .Add<SimpleBenchmarkInfo>()
            .Add<SimpleSourceInfo>();
        return VerifyPerfonar(root, schema);
    }

    private static EntryInfo CreateRoot()
    {
        var runId = Guid.Parse("11214D6B-4E25-44A4-8032-D4290C9F5617");
        var random = new NormalDistribution(10).Random(1729);
        double NextValue() => Math.Round(random.Next(), 3);
        int minute = 0;

        EntryInfo CreateEntry(string benchmarkId) => new EntryInfo
        {
            Identity = new IdentityInfo
            {
                RunId = runId,
                Timestamp = DateTimeOffset.Parse($"2021-01-01T00:0{minute++}:00Z").ToUnixTimeMilliseconds()
            },
            Source = new SimpleSourceInfo
            {
                BuildId = 142857,
                Branch = "main",
                ConfigurationId = "TeamCityConfiguration01",
                CommitHash = "1189402d156078473122bf787f3df6db81dc927c"
            },
            Host = new SimpleHostInfo
            {
                Os = new OsInfo { Name = "Linux" },
                Arch = "x64",
                ImageId = "Image42",
            },
            Benchmark = new SimpleBenchmarkInfo
            {
                BenchmarkId = benchmarkId,
                BenchmarkVersion = 1,
            },
        }.Add(
            new EntryInfo { Metric = "stage1", Value = NextValue(), Unit = TimeUnit.Millisecond },
            new EntryInfo { Metric = new MetricInfo("stage2", 2), Value = NextValue(), Unit = TimeUnit.Millisecond },
            new EntryInfo { Metric = "totalTime", Value = NextValue(), Unit = TimeUnit.Millisecond },
            new EntryInfo { Metric = "Footprint", Value = 20, Unit = SizeUnit.MB },
            new EntryInfo { Metric = "Gc.CollectCount", Value = 3 });

        var root = new EntryInfo
        {
            Identity = new IdentityInfo { Title = "Simple Measurements" }
        }.Add(
            CreateEntry("benchmark1"),
            CreateEntry("benchmark1"),
            CreateEntry("benchmark2"),
            CreateEntry("benchmark2"));
        return root;
    }

    [PublicAPI]
    private class SimpleHostInfo : HostInfo
    {
        public string Arch { get; set; } = "";
        public string ImageId { get; set; } = "";
    }

    [PublicAPI]
    private class SimpleExecutionInfo : ExecutionInfo
    {
    }

    [PublicAPI]
    private class SimpleBenchmarkInfo : BenchmarkInfo
    {
        public string BenchmarkId { get; set; } = "";
        public int BenchmarkVersion { get; set; }
    }

    [PublicAPI]
    private class SimpleSourceInfo : SourceInfo
    {
        public int BuildId { get; set; }
        public string ConfigurationId { get; set; } = "";
        public string Branch { get; set; } = "";
        public string CommitHash { get; set; } = "";
    }
}
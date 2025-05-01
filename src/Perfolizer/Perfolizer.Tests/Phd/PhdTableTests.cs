using JetBrains.Annotations;
using Perfolizer.InfoModels;
using Perfolizer.Metrology;
using Perfolizer.Phd.Base;
using Perfolizer.Phd.Presenting;
using Perfolizer.Phd.Tables;
using Perfolizer.Presenting;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Phd;

public class PhdTableTests(ITestOutputHelper output) : PhdTestsBase
{
    [Theory]
    [MemberData(nameof(EntryDataKeys))]
    public Task PhdTableTest(string key)
    {
        var entry = EntryDataMap[key];
        var table = new PhdTable(entry);
        var presenter = new StringPresenter();
        new PhdMarkdownTablePresenter(presenter).Present(table, new PhdTableStyle());
        return VerifyString(key, presenter.Dump());
    }

    private static readonly IDictionary<string, EntryInfo> EntryDataMap = new Dictionary<string, EntryInfo>
    {
        { "case01", Root().Add(Benchmark("Foo", "10ns"), Benchmark("Bar", "200ns")) },
        { "case02", Root().Add(Benchmark("Foo", "10us"), Benchmark("Bar", "200us")) },
        { "case03", Root().Add(Benchmark("Foo", "10ms"), Benchmark("Bar", "200ms")) },
        { "case04", Root().Add(Benchmark("Foo", "10s"), Benchmark("Bar", "200s")) },
        { "case05", Root().Add(Benchmark("Foo", "10ns"), Benchmark("Bar", "1us")) },
        { "case06", Root().Add(Benchmark("Foo", "10ns"), Benchmark("Bar", "1s")) },
    };

    [UsedImplicitly] public static TheoryData<string> EntryDataKeys = TheoryDataHelper.Create(EntryDataMap.Keys);

    private static EntryInfo Root() => new()
    {
        Meta = new PhdMeta
        {
            Table = new PhdTableConfig
            {
                ColumnDefinitions =
                [
                    new PhdColumnDefinition(".benchmark") { Cloud = "secondary" },
                    new PhdColumnDefinition(".job") { Cloud = "secondary", Compressed = true },
                    new PhdColumnDefinition("=center")
                ]
            }
        },
    };

    private static EntryInfo Benchmark(string name, params string[] metrics)
    {
        var entry = new EntryInfo { Benchmark = new CustomBenchmarkInfo(name) };
        for (int i = 0; i < metrics.Length; i++)
        {
            var measurement = Measurement.Parse(metrics[i]);
            entry.Add(new EntryInfo
            {
                IterationIndex = i,
                Value = measurement.NominalValue,
                Unit = measurement.Unit
            });
        }
        return entry;
    }

    private class CustomBenchmarkInfo(string name) : BenchmarkInfo
    {
        public string Name { get; set; } = name;
    }

    private Task VerifyString(string key, string content)
    {
        output.WriteLine(content);
        var settings = VerifyHelper.CreateSettings("Phd");
        settings.UseParameters(key);
        return Verify(content, settings);
    }
}
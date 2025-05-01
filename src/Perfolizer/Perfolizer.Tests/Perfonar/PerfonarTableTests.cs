using JetBrains.Annotations;
using Perfolizer.Metrology;
using Perfolizer.Models;
using Perfolizer.Perfonar.Base;
using Perfolizer.Perfonar.Presenting;
using Perfolizer.Perfonar.Tables;
using Perfolizer.Presenting;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Perfonar;

public class PerfonarTableTests(ITestOutputHelper output) : PerfonarTestsBase
{
    [Theory]
    [MemberData(nameof(EntryDataKeys))]
    public Task PerfonarTableTest(string key)
    {
        var entry = EntryDataMap[key];
        var table = new PerfonarTable(entry);
        var presenter = new StringPresenter();
        new PerfonarMarkdownTablePresenter(presenter).Present(table, new PerfonarTableStyle());
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
        Meta = new PerfonarMeta
        {
            Table = new PerfonarTableConfig
            {
                ColumnDefinitions =
                [
                    new PerfonarColumnDefinition(".benchmark") { Cloud = "secondary" },
                    new PerfonarColumnDefinition(".job") { Cloud = "secondary", Compressed = true },
                    new PerfonarColumnDefinition("=center")
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
        var settings = VerifyHelper.CreateSettings("Perfonar");
        settings.UseParameters(key);
        return Verify(content, settings);
    }
}
using System;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Selectors;
using Xunit;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Mathematics.Selectors;

public class RqqDumpTreeTests
{
    private readonly ITestOutputHelper output;

    public RqqDumpTreeTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [AssertionMethod]
    private void Check(double[] data)
    {
        var rqq = new Rqq(data);
        using var memoryStream = new MemoryStream();
        using var sw = new StreamWriter(memoryStream);
        rqq.DumpTreeAscii(sw, true);
        output.WriteLine(Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int) memoryStream.Length));
    }

    [Fact]
    public void Etalon() => Check(new double[] {6, 2, 0, 7, 9, 3, 1, 8, 5, 4});

    [Theory]
//        [InlineData(1)] // TODO
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(20)]
    [InlineData(300)]
    public void SimpleList(int n) => Check(Enumerable.Range(2, n).Select(x => (double) x).ToArray());

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(20)]
    public void Random(int n)
    {
        var random = new Random(42);
        Check(Enumerable.Range(2, n).Select(x => (double) random.Next(300)).ToArray());
    }
}
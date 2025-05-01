using Perfolizer.Models;
using Perfolizer.Perfonar.Base;

namespace Perfolizer.Tests.Perfonar;

public class PerfonarEmptyTests : PerfonarTestsBase
{
    [Fact]
    public Task PerfonarEmpty() => VerifyPerfonar(new EntryInfo(), new PerfonarSchema(""));
}
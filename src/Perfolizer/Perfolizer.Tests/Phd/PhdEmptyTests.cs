using Perfolizer.InfoModels;
using Perfolizer.Phd.Base;

namespace Perfolizer.Tests.Phd;

public class PhdEmptyTests : PhdTestsBase
{
    [Fact]
    public Task PhdEmpty() => VerifyPhd(new EntryInfo(), new PhdSchema(""));
}
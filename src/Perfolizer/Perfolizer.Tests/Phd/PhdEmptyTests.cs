using Perfolizer.Phd.Base;

namespace Perfolizer.Tests.Phd;

public class PhdEmptyTests : PhdTestsBase
{
    [Fact]
    public Task PhdEmpty() => VerifyPhd(new PhdEntry(), new PhdSchema(""));
}
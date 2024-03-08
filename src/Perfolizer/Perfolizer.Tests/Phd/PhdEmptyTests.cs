using Perfolizer.Phd;
using Perfolizer.Phd.Base;
using Perfolizer.Phd.Dto;

namespace Perfolizer.Tests.Phd;

public class PhdEmptyTests : PhdTestsBase
{
    [Fact]
    public Task PhdEmpty() => VerifyPhd(new PhdEntry(new PhdAttributes()), new PhdSchema(""));
}
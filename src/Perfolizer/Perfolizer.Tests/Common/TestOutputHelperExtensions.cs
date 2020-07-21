using JetBrains.Annotations;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Common
{
    public static class TestOutputHelperExtensions
    {
        public static void WriteLine([NotNull] this ITestOutputHelper output) => output.WriteLine("");
    }
}
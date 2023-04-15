using System;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace Perfolizer.Tests.Common;

public static class TestOutputHelperExtensions
{
    public static bool TraceMode
    {
        get
        {
            string? perfolizerTraceMode = Environment.GetEnvironmentVariable("PERFOLIZER_TRACE_MODE");
            return string.Equals(perfolizerTraceMode, "1", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(perfolizerTraceMode, "true", StringComparison.OrdinalIgnoreCase);
        }
    }

    public static void WriteLine(this ITestOutputHelper output) => output.WriteLine("");

    public static void TraceLine(this ITestOutputHelper output, string message)
    {
        if (TraceMode)
            output.WriteLine(message);
    }

    [StringFormatMethod("format")]
    public static void TraceLine(this ITestOutputHelper output, string format, params object[] args)
    {
        if (TraceMode)
            output.WriteLine(format, args);
    }
}
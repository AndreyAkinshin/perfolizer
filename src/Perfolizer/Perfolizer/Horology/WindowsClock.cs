using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Perfolizer.Horology;

internal class WindowsClock : IClock
{
    private static readonly bool GlobalIsAvailable;
    private static readonly long GlobalFrequency;

    static WindowsClock() => GlobalIsAvailable = Initialize(out GlobalFrequency);

    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long value);

    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long value);

    // 'HandleProcessCorruptedStateExceptionsAttribute' is obsolete in net6.0 because
    //   recovery from corrupted process state exceptions is not supported.
    // https://aka.ms/dotnet-warnings/SYSLIB0032
#if NETSTANDARD2_0
    [HandleProcessCorruptedStateExceptions] // https://github.com/dotnet/BenchmarkDotNet/issues/276
#endif
    [SecurityCritical]
    private static bool Initialize(out long qpf)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            qpf = default;
            return false;
        }

        try
        {
            return QueryPerformanceFrequency(out qpf) && QueryPerformanceCounter(out _);
        }
        catch
        {
            qpf = default;
            return false;
        }
    }

    public string Title => "Windows";
    public bool IsAvailable => GlobalIsAvailable;
    public Frequency Frequency => new Frequency(GlobalFrequency);

    public long GetTimestamp()
    {
        QueryPerformanceCounter(out long value);
        return value;
    }
}
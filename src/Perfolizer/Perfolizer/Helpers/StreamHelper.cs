using System.Text;

namespace Perfolizer.Helpers;

internal static class StreamHelper
{
    public static string StreamToString(Action<StreamWriter> dump)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        dump(streamWriter);
        streamWriter.Flush();
        return Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int) memoryStream.Length);
    }
}
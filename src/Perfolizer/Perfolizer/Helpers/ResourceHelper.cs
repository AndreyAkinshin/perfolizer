using System.Reflection;

namespace Perfolizer.Helpers;

internal static class ResourceHelper
{
    internal static string LoadResource(string resourceName)
    {
        using var stream = GetResourceStream(resourceName);
        if (stream == null) throw new Exception($"Resource {resourceName} not found");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static Stream? GetResourceStream(string resourceName) =>
        typeof(ResourceHelper).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName);
}
namespace Perfolizer.Perfonar.Base;

internal static class PerfonarHelper
{
    public static bool IsPerfonarPrimitive(this Type type) =>
        type.IsPrimitive ||
        type.IsEnum ||
        type == typeof(Guid) ||
        type == typeof(DateTimeOffset) ||
        type == typeof(string);
}
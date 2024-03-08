namespace Perfolizer.Phd.Base;

internal static class PhdHelper
{
    public static bool IsPhdPrimitive(this Type type) =>
        type.IsPrimitive ||
        type.IsEnum ||
        type == typeof(Guid) ||
        type == typeof(DateTimeOffset) ||
        type == typeof(string);
}
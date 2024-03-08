using System.Text;

namespace Perfolizer.Extensions;

internal static class StringBuilderExtensions
{
    public static StringBuilder TrimEnd(this StringBuilder builder, params char[] trimChars)
    {
        int length = builder.Length;
        if (trimChars.Any())
            while (length > 0 && trimChars.Contains(builder[length - 1]))
                length--;
        else
            while (length > 0 && char.IsWhiteSpace(builder[length - 1]))
                length--;

        builder.Length = length;
        return builder;
    }
}
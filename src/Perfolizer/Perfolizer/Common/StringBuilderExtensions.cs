using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Perfolizer.Common
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder TrimEnd([CanBeNull] this StringBuilder builder, params char[] trimChars)
        {
            if (builder == null)
                return null;

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
}
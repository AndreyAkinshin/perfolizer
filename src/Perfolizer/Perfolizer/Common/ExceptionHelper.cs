using System;
using JetBrains.Annotations;

namespace Perfolizer.Common
{
    internal static class ExceptionHelper
    {
        public static ArgumentOutOfRangeException RangeRequirementException(string paramName, double actualValue, double min, double max)
        {
            string message = Format("{0}={1}, but it should be in range [{2};{3}]", paramName, actualValue, min, max);
            return new ArgumentOutOfRangeException(paramName, actualValue, message);
        }

        [StringFormatMethod("format")]
        private static string Format(string format, params object[] args) => string.Format(DefaultCultureInfo.Instance, format, args);
    }
}
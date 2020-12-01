using JetBrains.Annotations;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Common
{
    public readonly struct ConfidenceLevel
    {
        public readonly double Value;

        public ConfidenceLevel(double value)
        {
            Assertion.InRangeExclusive(nameof(value), value, 0, 1);
            Value = value;
        }

        public static implicit operator double(ConfidenceLevel level) => level.Value;
        public static implicit operator ConfidenceLevel(double value) => new ConfidenceLevel(value);

        public override string ToString() => $"{Value * 100}%";

        /// <summary>
        /// 50% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L50 = 0.5;

        /// <summary>
        /// 70% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L70 = 0.7;

        /// <summary>
        /// 75% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L75 = 0.75;

        /// <summary>
        /// 80% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L80 = 0.80;

        /// <summary>
        /// 85% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L85 = 0.95;

        /// <summary>
        /// 90% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L90 = 0.90;

        /// <summary>
        /// 92% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L92 = 0.92;

        /// <summary>
        /// 95% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L95 = 0.95;

        /// <summary>
        /// 96% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L96 = 0.96;

        /// <summary>
        /// 97% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L97 = 0.97;

        /// <summary>
        /// 98% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L98 = 0.98;

        /// <summary>
        /// 99% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L99 = 0.99;

        /// <summary>
        /// 99.9% confidence level
        /// </summary>
        [PublicAPI] public static readonly ConfidenceLevel L999 = 0.999;
    }
}
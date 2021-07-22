using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public class TrimmedHarrellDavisQuantileEstimator : ModifiedHarrellDavisQuantileEstimator
    {
        public TrimmedHarrellDavisQuantileEstimator(Probability trimPercentage, int minTargetCount = DefaultMinTargetCount)
            : base(trimPercentage, minTargetCount)
        {
        }

        protected override bool IsWinsorized => false;
        public override string Alias => $"THD|{TrimPercentage.ToStringInvariant()}|Min{MinTargetCount}";
    }
}
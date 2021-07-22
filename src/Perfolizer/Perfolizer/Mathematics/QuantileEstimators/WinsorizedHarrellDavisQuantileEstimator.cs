using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public class WinsorizedHarrellDavisQuantileEstimator : ModifiedHarrellDavisQuantileEstimator
    {
        public WinsorizedHarrellDavisQuantileEstimator(Probability trimPercentage, int minTargetCount = DefaultMinTargetCount)
            : base(trimPercentage, minTargetCount)
        {
        }

        protected override bool IsWinsorized => true;
        public override string Alias => $"WHD|{TrimPercentage.ToStringInvariant()}|Min{MinTargetCount}";
    }
}
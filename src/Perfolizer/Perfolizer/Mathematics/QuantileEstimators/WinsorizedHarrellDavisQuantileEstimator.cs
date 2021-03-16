using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public class WinsorizedHarrellDavisQuantileEstimator : ModifiedHarrellDavisQuantileEstimator
    {
        public WinsorizedHarrellDavisQuantileEstimator(Probability trimPercent) : base(trimPercent)
        {
        }

        protected override bool IsWinsorized => true;
        public override string Alias => $"WHD|{TrimPercent.ToStringInvariant()}";
    }
}
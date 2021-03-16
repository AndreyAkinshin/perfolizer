using Perfolizer.Common;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    public class TrimmedHarrellDavisQuantileEstimator : ModifiedHarrellDavisQuantileEstimator
    {
        public TrimmedHarrellDavisQuantileEstimator(Probability trimPercent) : base(trimPercent)
        {
        }

        protected override bool IsWinsorized => false;
        public override string Alias => $"THD|{TrimPercent.ToStringInvariant()}";
    }
}
using System;
using Perfolizer.Collections;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// This quantile estimator supports nine popular estimation algorithms that are described in [Hyndman1996].
    /// <remarks>
    /// Hyndman, R. J. and Fan, Y. (1996) Sample quantiles in statistical packages, American Statistician 50, 361–365. doi: 10.2307/2684934.
    /// </remarks>
    /// </summary>
    public class HyndmanYanQuantileEstimator : IQuantileEstimator
    {
        private readonly HyndmanYanType type;

        public HyndmanYanQuantileEstimator(HyndmanYanType type)
        {
            if (!Enum.IsDefined(typeof(HyndmanYanType), type))
                throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown type");

            this.type = type;
        }
        
        /// <summary>
        /// Returns 1-based real index estimation
        /// </summary>
        protected double GetH(double n, double p) => type switch
        {
            HyndmanYanType.Type1 => n * p + 0.5,
            HyndmanYanType.Type2 => n * p + 0.5,
            HyndmanYanType.Type3 => n * p,
            HyndmanYanType.Type4 => n * p,
            HyndmanYanType.Type5 => n * p + 0.5,
            HyndmanYanType.Type6 => (n + 1) * p,
            HyndmanYanType.Type7 => (n - 1) * p + 1,
            HyndmanYanType.Type8 => (n + 1.0 / 3) * p + 1.0 / 3,
            HyndmanYanType.Type9 => (n + 1.0 / 4) * p + 3.0 / 8,
            _ => throw new InvalidOperationException()
        };

        public double GetQuantile(ISortedReadOnlyList<double> data, double probability)
        {
            QuantileEstimatorHelper.CheckArguments(data, probability);

            int n = data.Count;
            double p = probability;
            double h = GetH(n, p);

            double Value(int index)
            {
                index -= 1; // Adapt one-based formula to the zero-based list
                if (index <= 0)
                    return data[0];
                if (index >= data.Count)
                    return data[data.Count - 1];
                return data[index];
            }

            double LinearInterpolation()
            {
                int hFloor = (int) h;
                double fraction = h - hFloor;
                if (hFloor + 1 <= n)
                    return Value(hFloor) * (1 - fraction) + Value(hFloor + 1) * fraction;
                return Value(hFloor);
            }

            return type switch
            {
                HyndmanYanType.Type1 => Value((int) Math.Ceiling(h - 0.5)),
                HyndmanYanType.Type2 => (Value((int) Math.Ceiling(h - 0.5)) + Value((int) Math.Floor(h + 0.5))) / 2,
                HyndmanYanType.Type3 => Value((int) Math.Round(h, MidpointRounding.ToEven)),
                _ => LinearInterpolation()
            };
        }
    }
}
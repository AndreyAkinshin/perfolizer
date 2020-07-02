using System;
using System.Diagnostics.CodeAnalysis;
using Perfolizer.Mathematics.Common;
using static System.Math;

namespace Perfolizer.Mathematics.Distributions
{
    public class GumbelDistribution
    {
        private const double EulerMascheroni = 0.57721566490153286060651209008240243104215933593992;

        public double Location { get; }
        public double Scale { get; }

        private double Mu => Location;
        private double Beta => Scale;

        public GumbelDistribution(double location = 0, double scale = 1)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException(nameof(scale), $"{nameof(scale)} should be positive");
            Location = location;
            Scale = scale;
        }

        /// <summary>
        /// Probability density function 
        /// </summary>
        public double Pdf(double x)
        {
            double z = (x - Mu) / Beta;
            return Exp(-(z + Exp(-z))) / Beta;
        }

        /// <summary>
        /// Cumulative distribution function
        /// </summary>
        public double Cdf(double x) => Exp(-Exp(-(x - Mu) / Beta));

        /// <summary>
        /// Quantile function
        /// </summary>
        public double Quantile(double x)
        {
            if (x < 0 || x > 1)
                throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} should be be between 0 and 1");
            return x switch
            {
                0 => double.NegativeInfinity,
                1 => double.PositiveInfinity,
                _ => Mu - Beta * Log(-Log(x))
            };
        }

        public double Mean => Mu + Beta * EulerMascheroni;
        public double Median => Mu - Beta * Log(Log(2));
        public double Variance => PI * PI * Beta * Beta / 6;
        public double StandardDeviation => Variance.Sqrt();
    }
}
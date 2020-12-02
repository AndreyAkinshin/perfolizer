using System;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Functions;
using static System.Math;

namespace Perfolizer.Mathematics.Distributions
{
    /// <summary>
    /// Fréchet distribution (Generalized Extreme Value distribution Type-II)
    /// </summary>
    public class FrechetDistribution : IDistribution
    {
        public double Location { get; }
        public double Scale { get; }
        public double Shape { get; }

        private double M => Location;
        private double S => Scale;
        private double A => Shape;
        private double Z(double x) => (x - M) / S;

        public FrechetDistribution(double location = 0, double scale = 1, double shape = 1)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException(nameof(scale), $"{nameof(scale)} should be positive");
            if (shape <= 0)
                throw new ArgumentOutOfRangeException(nameof(shape), $"{nameof(shape)} should be positive");

            Location = location;
            Scale = scale;
            Shape = shape;
        }

        public double Pdf(double x)
        {
            double z = Z(x);
            return A / S * Pow(z, -1 - A) * Exp(-Pow(z, -A));
        }

        public double Cdf(double x) => Exp(-Pow(Z(x), -A));

        public double Quantile(Probability p)
        {
            return p.Value switch
            {
                0 => M,
                1 => double.PositiveInfinity,
                _ => M + S * Pow(-Log(p), -1 / A)
            };
        }

        public double Mean => A <= 1 ? double.PositiveInfinity : M + S * GammaFunction.Value(1 - 1 / A);
        public double Median => M + S / Pow(Constants.Log2, 1 / A);

        public double Variance => A <= 2
            ? double.PositiveInfinity
            : S.Sqr() * (GammaFunction.Value(1 - 2 / A) - GammaFunction.Value(1 - 1 / A).Sqr());

        public double StandardDeviation => Variance.Sqrt();
    }
}
namespace Perfolizer.Mathematics.Distributions
{
    public interface IDistribution
    {
        /// <summary>
        /// Probability density function 
        /// </summary>
        double Pdf(double x);

        /// <summary>
        /// Cumulative distribution function
        /// </summary>
        double Cdf(double x);

        /// <summary>
        /// Quantile function
        /// </summary>
        double Quantile(double x);

        double Mean { get; }
        double Median { get; }
        double Variance { get; }
        double StandardDeviation { get; }
    }
}
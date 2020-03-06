using Perfolizer.Mathematics.Functions;

namespace Perfolizer.Mathematics.Distributions
{
    public class BetaDistribution
    {
        private readonly double alpha, beta;

        public BetaDistribution(double alpha, double beta)
        {
            this.alpha = alpha;
            this.beta = beta;
        }

        public double Cdf(double x) => BetaFunction.RegularizedIncompleteValue(alpha, beta, x);
    }
}
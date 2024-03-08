using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.ScaleEstimators;

/// <summary>
/// Quantile absolute deviation: a scale estimator consistent with the standard deviation under normality.
/// 
/// Based on: Andrey Akinshin "Quantile absolute deviation" (2022) arXiv:2208.13459 [stat.ME] https://doi.org/10.48550/arXiv.2208.13459
/// </summary>
public abstract class QuantileAbsoluteDeviationEstimator : IScaleEstimator
{
    /// <summary>
    /// Invariant quantile absolute deviation (no consistency with the standard deviation).
    /// </summary>
    public static QuantileAbsoluteDeviationEstimator Invariant(Probability p) => new InvariantEstimator(p);

    /// <summary>
    /// The standard quantile absolute deviation.
    /// Asymptotic breakdown point: ≈31.73%.
    /// Asymptotic Gaussian efficiency: ≈54.06%.
    /// </summary>
    public static readonly QuantileAbsoluteDeviationEstimator Standard = new StandardEstimator();

    /// <summary>
    /// The standard quantile absolute deviation.
    /// Asymptotic breakdown point: ≈13.83%.
    /// Asymptotic Gaussian efficiency: ≈65.22%.
    /// </summary>
    public static readonly QuantileAbsoluteDeviationEstimator Optimal = new OptimalEstimator();

    public Probability P { get; }

    [PublicAPI]
    protected QuantileAbsoluteDeviationEstimator(Probability p)
    {
        P = p;
    }

    protected abstract double ScaleFactor(int n);
    public abstract IQuantileEstimator QuantileEstimator { get; }

    double IScaleEstimator.Scale(Sample sample) => Qad(sample);

    public double Qad(Sample sample)
    {
        Assertion.NotNull(nameof(sample), sample);
        if (sample.Size == 1)
            return 0;

        double scaleFactor = ScaleFactor(sample.Size);
        double median = QuantileEstimator.Median(sample);
        double[] deviations = new double[sample.Size];
        for (int i = 0; i < sample.Size; i++)
            deviations[i] = Math.Abs(sample.Values[i] - median);
        return scaleFactor * QuantileEstimator.Quantile(new Sample(deviations), P);
    }

    private class InvariantEstimator : QuantileAbsoluteDeviationEstimator
    {
        public InvariantEstimator(Probability p) : base(p)
        {
        }

        protected override double ScaleFactor(int n) => 1;

        public override IQuantileEstimator QuantileEstimator => SimpleQuantileEstimator.Instance;
    }

    private class StandardEstimator : QuantileAbsoluteDeviationEstimator
    {
        public StandardEstimator() : base(0.682689492137086)
        {
        }

        private static readonly double[] Constants =
        {
            double.NaN, double.NaN,
            1.7724, 1.3506, 1.3762, 1.1881, 1.1773, 1.1289, 1.1248, 1.0920, 1.0943,
            1.0764, 1.0738, 1.0630, 1.0637, 1.0533, 1.0537, 1.0482, 1.0468, 1.0419, 1.0429,
            1.0377, 1.0376, 1.0351, 1.0343, 1.0314, 1.0320, 1.0292, 1.0290, 1.0272, 1.0271,
            1.0251, 1.0253, 1.0238, 1.0235, 1.0223, 1.0224, 1.0210, 1.0210, 1.0201, 1.0199,
            1.0189, 1.0192, 1.0180, 1.0180, 1.0174, 1.0172, 1.0165, 1.0166, 1.0158, 1.0158,
            1.0152, 1.0152, 1.0146, 1.0146, 1.0141, 1.0140, 1.0135, 1.0137, 1.0130, 1.0131,
            1.0127, 1.0126, 1.0123, 1.0124, 1.0118, 1.0119, 1.0115, 1.0115, 1.0111, 1.0112,
            1.0108, 1.0108, 1.0106, 1.0106, 1.0102, 1.0103, 1.0100, 1.0100, 1.0097, 1.0097,
            1.0095, 1.0095, 1.0093, 1.0092, 1.0090, 1.0091, 1.0089, 1.0088, 1.0086, 1.0086,
            1.0084, 1.0084, 1.0082, 1.0082, 1.0081, 1.0081, 1.0079, 1.0079, 1.0078, 1.0077
        };

        protected override double ScaleFactor(int n) => n <= 100 ? Constants[n] : 1 + 0.762 / n + 0.967 / n / n;

        public override IQuantileEstimator QuantileEstimator => SimpleQuantileEstimator.Instance;
    }

    private class OptimalEstimator : QuantileAbsoluteDeviationEstimator
    {
        public OptimalEstimator() : base(0.861678977787423)
        {
        }

        private static readonly double[] Constants =
        {
            double.NaN, double.NaN,
            1.7729, 0.9788, 0.9205, 0.8194, 0.8110, 0.7792, 0.7828, 0.7600, 0.7535,
            0.7388, 0.7365, 0.7282, 0.7284, 0.7241, 0.7234, 0.7170, 0.7155, 0.7113, 0.7110,
            0.7083, 0.7088, 0.7068, 0.7056, 0.7030, 0.7024, 0.7006, 0.7006, 0.6995, 0.6998,
            0.6979, 0.6974, 0.6960, 0.6958, 0.6949, 0.6949, 0.6944, 0.6940, 0.6929, 0.6927,
            0.6918, 0.6918, 0.6913, 0.6914, 0.6907, 0.6904, 0.6897, 0.6896, 0.6891, 0.6892,
            0.6888, 0.6887, 0.6882, 0.6880, 0.6875, 0.6875, 0.6871, 0.6872, 0.6870, 0.6868,
            0.6863, 0.6862, 0.6859, 0.6859, 0.6857, 0.6858, 0.6854, 0.6853, 0.6850, 0.6849,
            0.6847, 0.6847, 0.6846, 0.6845, 0.6842, 0.6841, 0.6839, 0.6839, 0.6837, 0.6838,
            0.6836, 0.6834, 0.6833, 0.6832, 0.6831, 0.6830, 0.6829, 0.6830, 0.6827, 0.6827,
            0.6825, 0.6825, 0.6823, 0.6823, 0.6823, 0.6822, 0.6820, 0.6820, 0.6819, 0.6819
        };

        protected override double ScaleFactor(int n) => n <= 100 ? Constants[n] : 0.6747309 * (1 + 1.047 / n + 1.193 / n / n);

        public override IQuantileEstimator QuantileEstimator => SimpleQuantileEstimator.Instance;
    }
}
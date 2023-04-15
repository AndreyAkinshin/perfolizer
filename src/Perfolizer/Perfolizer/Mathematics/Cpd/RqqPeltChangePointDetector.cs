using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Selectors;

namespace Perfolizer.Mathematics.Cpd;

[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
public class RqqPeltChangePointDetector : PeltChangePointDetector
{
    private const double DefaultSensitivity = 0.5;
    private const int DefaultQuantileCount = 12;
    private const double DefaultHeterogeneityFactor = 1.35;
    private const double DefaultHomogeneityFactor = 0.35;

    public static readonly RqqPeltChangePointDetector Instance = new RqqPeltChangePointDetector();

    private readonly double sensitivity;
    private readonly int quantileCount;
    private readonly double heterogeneityFactor;
    private readonly double homogeneityFactor;
    private readonly double[] probabilities;
    private readonly double[] factors;

    [PublicAPI]
    public RqqPeltChangePointDetector(
        double sensitivity = DefaultSensitivity,
        int quantileCount = DefaultQuantileCount,
        double heterogeneityFactor = DefaultHeterogeneityFactor,
        double homogeneityFactor = DefaultHomogeneityFactor
    )
    {
        this.sensitivity = sensitivity;
        this.quantileCount = quantileCount;
        this.heterogeneityFactor = heterogeneityFactor;
        this.homogeneityFactor = homogeneityFactor;

        probabilities = new double[quantileCount];
        factors = new double[quantileCount];
        for (int i = 0; i < quantileCount; i++)
        {
            probabilities[i] = i * 1.0 / (quantileCount - 1);
            int group = i / 5;
            factors[i] = Math.Max(1 - group * 0.1, 0);
        }
    }

    internal RqqPeltChangePointDetector(
        double[] probabilities,
        double[] factors,
        double sensitivity = DefaultSensitivity,
        double heterogeneityFactor = DefaultHeterogeneityFactor,
        double homogeneityFactor = DefaultHomogeneityFactor
    )
    {
        if (probabilities.Length != factors.Length)
            throw new ArgumentException($"{nameof(probabilities)} and {nameof(factors)} should have the same length");

        this.sensitivity = sensitivity;
        this.quantileCount = probabilities.Length;
        this.heterogeneityFactor = heterogeneityFactor;
        this.homogeneityFactor = homogeneityFactor;
        this.probabilities = probabilities;
        this.factors = factors;
    }

    public class CostCalculator : ICostCalculator
    {
        private readonly RqqPeltChangePointDetector detector;
        private double Sensitivity => detector.sensitivity;
        private int QuantileCount => detector.quantileCount;
        private double HeterogeneityFactor => detector.heterogeneityFactor;
        private double HomogeneityFactor => detector.homogeneityFactor;
        private double[] Probabilities => detector.probabilities;
        private double[] Factors => detector.factors;

        private readonly Rqq rqq;
        private readonly double[] quantileLeft;
        private readonly double[] quantileRight;

        public CostCalculator(double[] data, RqqPeltChangePointDetector detector)
        {
            this.detector = detector;
            quantileLeft = new double[QuantileCount];
            quantileRight = new double[QuantileCount];
            Penalty = Sensitivity;
            rqq = new Rqq(data);
        }

        public double Penalty { get; }

        public double GetCost(int tau0, int tau1, int tau2)
        {
            if (tau0 == tau1 || tau1 == tau2)
                return 0;

            double homogeneity1 = GetDistance(tau0, (tau0 + tau1) / 2, tau1);
            double homogeneity2 = GetDistance(tau1, (tau1 + tau2) / 2, tau2);
            double heterogeneity = GetDistance(tau0, tau1, tau2);
            double maxHomogeneity = Math.Max(homogeneity1, homogeneity2);
            double distance = heterogeneity * HeterogeneityFactor - maxHomogeneity * HomogeneityFactor;

            return distance > Sensitivity ? -distance : 0;
        }

        public double GetDistance(int tau0, int tau1, int tau2)
        {
            if (tau0 == tau1 || tau1 == tau2)
                return 0;

            for (int i = 0; i < QuantileCount; i++)
            {
                quantileLeft[i] = rqq.Quantile(tau0, tau1 - 1, Probabilities[i]);
                quantileRight[i] = rqq.Quantile(tau1, tau2 - 1, Probabilities[i]);
            }

            return CalcDistance(quantileLeft, quantileRight);
        }

        private double CalcDistance(double[] qa, double[] qb)
        {
            double sumA = 0, sumB = 0;
            int minJ = 0;
            for (int i = 0; i < qa.Length - 1; i++)
            {
                double x1 = qa[i], y1 = qa[i + 1], z1 = y1 - x1;
                for (int j = minJ; j < qb.Length - 1; j++)
                {
                    double x2 = qb[j], y2 = qb[j + 1], z2 = y2 - x2;
                    if (y2 < x1)
                    {
                        minJ = j + 1;
                        continue;
                    }

                    if (x2 > y1)
                        break;

                    double factor = Factors[Math.Abs(i - j)];
                    double x3 = Math.Max(x1, x2), y3 = Math.Min(y1, y2), z3 = y3 - x3;
                    if (z3 < 0)
                        continue;

                    if (x3 <= x1 && y1 <= y3)
                        sumA += 1;
                    else if (z1 > 1e-5)
                        sumA += z3 / z1 * factor;

                    if (x3 <= x2 && y2 <= y3)
                        sumB += 1;
                    else if (z2 > 1e-5)
                        sumB += z3 / z2 * factor;
                }
            }

            return 1 - sumA * sumB / (qa.Length - 1) / (qb.Length - 1);
        }
    }

    public override ICostCalculator CreateCostCalculator(double[] data) => new CostCalculator(data, this);
}
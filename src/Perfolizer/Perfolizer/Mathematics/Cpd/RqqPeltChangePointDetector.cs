using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Perfolizer.Mathematics.Selectors;

namespace Perfolizer.Mathematics.Cpd
{
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public class RqqPeltChangePointDetector : PeltChangePointDetector
    {
        public static readonly RqqPeltChangePointDetector Instance = new RqqPeltChangePointDetector();

        private readonly double penalty;
        private readonly int quantileCount;

        [PublicAPI]
        public RqqPeltChangePointDetector(double penalty = 0.45, int quantileCount = 12)
        {
            this.penalty = penalty;
            this.quantileCount = quantileCount;
        }

        public class CostCalculator : ICostCalculator
        {
            private readonly int quantileCount;
            private readonly Rqq rqq;
            private readonly double[] probs;
            private readonly double[] factors;
            private readonly double[] quantileLeft;
            private readonly double[] quantileRight;

            public CostCalculator([NotNull] double[] data, double penalty, int quantileCount)
            {
                this.quantileCount = quantileCount;
                probs = new double[quantileCount];
                factors = new double[quantileCount];
                quantileLeft = new double[quantileCount];
                quantileRight = new double[quantileCount];

                Penalty = penalty;
                rqq = new Rqq(data);
                for (int i = 0; i < quantileCount; i++)
                {
                    probs[i] = i * 1.0 / (quantileCount - 1);
                    int group = i / 5;
                    factors[i] = Math.Max(1 - group * 0.1, 0);
                }
            }

            public double Penalty { get; }

            public double GetCost(int tau0, int tau1, int tau2)
            {
                if (tau0 == tau1 || tau1 == tau2)
                    return 0;

                for (int i = 0; i < quantileCount; i++)
                {
                    quantileLeft[i] = rqq.GetQuantile(tau0, tau1 - 1, probs[i]);
                    quantileRight[i] = rqq.GetQuantile(tau1, tau2 - 1, probs[i]);
                }

                return CalcDistance(quantileLeft, quantileRight);
            }

            private double CalcDistance([NotNull] double[] qa, [NotNull] double[] qb)
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

                        double factor = factors[Math.Abs(i - j)];
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

                double u = 1 - sumA * sumB / (qa.Length - 1) / (qb.Length - 1);
                return u < Penalty ? 0 : -u;
            }
        }

        public override ICostCalculator CreateCostCalculator(double[] data)
        {
            return new CostCalculator(data, penalty, quantileCount);
        }
    }
}
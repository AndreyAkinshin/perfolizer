using System;
using Perfolizer.Mathematics.Common;
using static System.Math;

namespace Perfolizer.Mathematics.Distributions
{
    public class StudentDistribution
    {
        private readonly double df;

        public StudentDistribution(double df)
        {
            this.df = df;
        }

        public double Cdf(double x) => StudentOneTail(x, df);
        
        public double Quantile(double x)
        {
            if (x < 0 || x > 1)
                throw new ArgumentOutOfRangeException(nameof(x), "x should be in [0;1]");
            return InverseOneTailStudent(x, df);
        }

        /// <summary>
        /// ACM Algorithm 395: Student's t-distribution
        ///
        /// Evaluates the two-tail probability P(t|n) that t is exceeded
        /// in magnitude for Student's t-distribution with n degrees of freedom.
        ///
        /// http://dl.acm.org/citation.cfm?id=355599
        /// </summary>
        /// <param name="t">t-value, t > 0</param>
        /// <param name="n">Degree of freedom, n >= 1</param>
        /// <returns>2-tail p-value</returns>
        public static double StudentTwoTail(double t, double n)
        {
            if (t < 0)
                throw new ArgumentOutOfRangeException(nameof(t), "t should be >= 0");
            if (n < 1)
                throw new ArgumentOutOfRangeException(nameof(n), "n should be >= 1");
            t = t.Sqr();
            double y = t / n;
            double b = y + 1.0;
            int nn = (int) Round(n);
            if (Abs(n - nn) > 1e-9 || n >= 20 || t < n && n > 200)
            {
                if (y > 1.0e-6)
                    y = Log(b);
                double a = n - 0.5;
                b = 48.0 * a.Sqr();
                y = a * y;
                y = (((((-0.4 * y - 3.3) * y - 24.0) * y - 85.5) / (0.8 * y.Sqr() + 100.0 + b) + y + 3.0) / b + 1.0) * Sqrt(y);
                return 2 * NormalDistribution.Gauss(-y);
            }

            {
                double z = 1;

                double a;
                if (n < 20 && t < 4.0)
                {
                    y = Sqrt(y);
                    a = y;
                    if (nn == 1)
                        a = 0;
                }
                else
                {
                    a = Sqrt(b);
                    y = a * nn;
                    int j = 0;
                    while (Abs(a - z) > 0)
                    {
                        j += 2;
                        z = a;
                        y *= (j - 1) / (b * j);
                        a += y / (nn + j);
                    }

                    nn += 2;
                    z = 0;
                    y = 0;
                    a = -a;
                }

                while (true)
                {
                    nn -= 2;
                    if (nn > 1)
                        a = (nn - 1) / (b * nn) * a + y;
                    else
                        break;
                }

                a = nn == 0 ? a / Sqrt(b) : (Atan(y) + a / b) * 2 / PI;
                return z - a;
            }
        }

        public static double StudentOneTail(double t, double n) => t >= 0
            ? 1 - StudentTwoTail(t, n) / 2
            : 1 - StudentOneTail(-t, n);

        // TODO: Optimize, support corner cases
        public static double InverseTwoTailStudent(double p, double n)
        {
            double lower = 0.0;
            double upper = 1000.0;
            while (upper - lower > 1e-9)
            {
                double t = (lower + upper) / 2;
                double p2 = StudentTwoTail(t, n);
                if (p2 < p)
                    upper = t;
                else
                    lower = t;
            }

            return (lower + upper) / 2;
        }
        
        // TODO: Optimize, support corner cases
        public static double InverseOneTailStudent(double p, double n)
        {
            double lower = -1000.0;
            double upper = 1000.0;
            while (upper - lower > 1e-9)
            {
                double t = (lower + upper) / 2;
                double p2 = StudentOneTail(t, n);
                if (p2 > p)
                    upper = t;
                else
                    lower = t;
            }

            return (lower + upper) / 2;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Perfolizer.Tests.Common
{
    public class AbsoluteEqualityComparer : IEqualityComparer<double>
    {
        private readonly double eps;

        public AbsoluteEqualityComparer(double eps)
        {
            this.eps = eps;
        }

        public bool Equals(double x, double y)
        {
            if (double.IsPositiveInfinity(x) && double.IsPositiveInfinity(y))
                return true;
            if (double.IsNegativeInfinity(x) && double.IsNegativeInfinity(y))
                return true;
            if (double.IsNaN(x) && double.IsNaN(y))
                return true;
            return Math.Abs(x - y) < eps;
        }

        public int GetHashCode(double x) => x.GetHashCode();
    }
}
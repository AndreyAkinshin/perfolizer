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

        public bool Equals(double x, double y) => Math.Abs(x - y) < eps;

        public int GetHashCode(double x) => x.GetHashCode();
    }
}
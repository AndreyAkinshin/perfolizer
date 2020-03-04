using System;
using System.Collections.Generic;
using System.Linq;

namespace Perfolizer.Mathematics.OutlierDetection
{
    public abstract class OutlierDetector
    {
        public abstract bool IsLowerOutlier(double x);
        public abstract bool IsUpperOutlier(double x);
        public bool IsOutlier(double x) => IsLowerOutlier(x) || IsUpperOutlier(x);

        public IReadOnlyList<double> WithoutLowerOutliers(IEnumerable<double> values)
        {
            return values.Where(x => !IsLowerOutlier(x)).ToList();
        }

        public IReadOnlyList<double> WithoutUpperOutliers(IEnumerable<double> values)
        {
            return values.Where(x => !IsLowerOutlier(x)).ToList();
        }

        public IReadOnlyList<double> WithoutAllOutliers(IEnumerable<double> values)
        {
            return values.Where(x => !IsOutlier(x)).ToList();
        }

        public IReadOnlyList<double> ApplyOutlierMode(IReadOnlyList<double> values, OutlierMode mode)
        {
            switch (mode)
            {
                case OutlierMode.DontRemove:
                    return values;
                case OutlierMode.RemoveUpper:
                    return WithoutUpperOutliers(values);
                case OutlierMode.RemoveLower:
                    return WithoutLowerOutliers(values);
                case OutlierMode.RemoveAll:
                    return WithoutAllOutliers(values);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
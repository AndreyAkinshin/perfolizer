using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.OutlierDetection
{
    public static class OutlierDetectorExtensions
    {
        public static bool IsOutlier(this IOutlierDetector outlierDetector, double x)
        {
            return outlierDetector.IsLowerOutlier(x) || outlierDetector.IsUpperOutlier(x);
        }
        
        public static IEnumerable<double> LowerOutliers(this IOutlierDetector outlierDetector,
            IEnumerable<double> values)
        {
            return values.Where(outlierDetector.IsLowerOutlier);
        }

        public static IEnumerable<double> UpperOutliers(this IOutlierDetector outlierDetector,
            IEnumerable<double> values)
        {
            return values.Where(outlierDetector.IsUpperOutlier);
        }

        public static IEnumerable<double> AllOutliers(this IOutlierDetector outlierDetector,
            IEnumerable<double> values)
        {
            return values.Where(outlierDetector.IsOutlier);
        }

        public static IEnumerable<double> WithoutLowerOutliers(this IOutlierDetector outlierDetector,
            IEnumerable<double> values)
        {
            return values.Where(x => !outlierDetector.IsLowerOutlier(x));
        }

        public static IEnumerable<double> WithoutUpperOutliers(this IOutlierDetector outlierDetector,
            IEnumerable<double> values)
        {
            return values.Where(x => !outlierDetector.IsUpperOutlier(x));
        }

        public static IEnumerable<double> WithoutAllOutliers(this IOutlierDetector outlierDetector,
            IEnumerable<double> values)
        {
            return values.Where(x => !outlierDetector.IsOutlier(x));
        }

        public static IEnumerable<double> ApplyOutlierMode(this IOutlierDetector outlierDetector,
            IEnumerable<double> values, OutlierMode mode)
        {
            switch (mode)
            {
                case OutlierMode.DontRemove:
                    return values;
                case OutlierMode.RemoveUpper:
                    return outlierDetector.WithoutUpperOutliers(values);
                case OutlierMode.RemoveLower:
                    return outlierDetector.WithoutLowerOutliers(values);
                case OutlierMode.RemoveAll:
                    return outlierDetector.WithoutAllOutliers(values);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
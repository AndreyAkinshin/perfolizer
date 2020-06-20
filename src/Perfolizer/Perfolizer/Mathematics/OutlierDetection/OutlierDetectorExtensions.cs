using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.OutlierDetection
{
    public static class OutlierDetectorExtensions
    {
        public static bool IsOutlier([NotNull] this IOutlierDetector outlierDetector, double x)
        {
            return outlierDetector.IsLowerOutlier(x) || outlierDetector.IsUpperOutlier(x);
        }
        
        [NotNull]
        public static IEnumerable<double> GetLowerOutliers([NotNull] this IOutlierDetector outlierDetector,
            [NotNull] IEnumerable<double> values)
        {
            return values.Where(outlierDetector.IsLowerOutlier);
        }

        [NotNull]
        public static IEnumerable<double> GetUpperOutliers([NotNull] this IOutlierDetector outlierDetector,
            [NotNull] IEnumerable<double> values)
        {
            return values.Where(outlierDetector.IsUpperOutlier);
        }

        [NotNull]
        public static IEnumerable<double> GetAllOutliers([NotNull] this IOutlierDetector outlierDetector,
            [NotNull] IEnumerable<double> values)
        {
            return values.Where(outlierDetector.IsOutlier);
        }

        [NotNull]
        public static IEnumerable<double> WithoutLowerOutliers([NotNull] this IOutlierDetector outlierDetector,
            [NotNull] IEnumerable<double> values)
        {
            return values.Where(x => !outlierDetector.IsLowerOutlier(x));
        }

        [NotNull]
        public static IEnumerable<double> WithoutUpperOutliers([NotNull] this IOutlierDetector outlierDetector,
            [NotNull] IEnumerable<double> values)
        {
            return values.Where(x => !outlierDetector.IsUpperOutlier(x));
        }

        [NotNull]
        public static IEnumerable<double> WithoutAllOutliers([NotNull] this IOutlierDetector outlierDetector,
            [NotNull] IEnumerable<double> values)
        {
            return values.Where(x => !outlierDetector.IsOutlier(x));
        }

        [NotNull]
        public static IEnumerable<double> ApplyOutlierMode([NotNull] this IOutlierDetector outlierDetector,
            [NotNull] IEnumerable<double> values, OutlierMode mode)
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
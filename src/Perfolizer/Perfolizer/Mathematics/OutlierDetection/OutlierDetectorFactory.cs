using System;
using System.Collections.Generic;

namespace Perfolizer.Mathematics.OutlierDetection
{
    public class OutlierDetectorFactory : IOutlierDetectorFactory
    {
        public static readonly IOutlierDetectorFactory Tukey =
            new OutlierDetectorFactory(values => TukeyOutlierDetector.Create(values));

        public static readonly IOutlierDetectorFactory Mad =
            new OutlierDetectorFactory(values => MadOutlierDetector.Create(values));

        public static readonly IOutlierDetectorFactory DoubleMad =
            new OutlierDetectorFactory(values => DoubleMadOutlierDetector.Create(values));

        private readonly Func<IReadOnlyList<double>, IOutlierDetector> create;

        public OutlierDetectorFactory(Func<IReadOnlyList<double>, IOutlierDetector> create)
        {
            this.create = create;
        }

        public IOutlierDetector Create(IReadOnlyList<double> values) => create(values);
    }
}
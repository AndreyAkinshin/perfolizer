namespace Perfolizer.Mathematics.OutlierDetection;

public class SimpleOutlierDetectorFactory : IOutlierDetectorFactory
{
    public static readonly IOutlierDetectorFactory Tukey =
        new SimpleOutlierDetectorFactory(values => TukeyOutlierDetector.Create(values));

    public static readonly IOutlierDetectorFactory Mad =
        new SimpleOutlierDetectorFactory(values => MadOutlierDetector.Create(values));

    public static readonly IOutlierDetectorFactory DoubleMad =
        new SimpleOutlierDetectorFactory(values => DoubleMadOutlierDetector.Create(values));

    private readonly Func<IReadOnlyList<double>, IOutlierDetector> create;

    public SimpleOutlierDetectorFactory(Func<IReadOnlyList<double>, IOutlierDetector> create)
    {
        this.create = create;
    }

    public IOutlierDetector Create(IReadOnlyList<double> values) => create(values);
}
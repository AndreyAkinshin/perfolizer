using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.LocationEstimators;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.ScaleEstimators;
using Perfolizer.Metrology;
using Perfolizer.Perfonar.Base;
using Pragmastat.Estimators;
using Pragmastat.Metrology;

namespace Perfolizer.Perfonar.Functions;

public class PerfonarFunctionResolver
{
    private readonly Dictionary<string, PerfonarFunction> registeredFunctions = new(StringComparer.OrdinalIgnoreCase);

    [PublicAPI]
    public PerfonarFunctionResolver Register(params PerfonarFunction[] functions)
    {
        foreach (var function in functions)
            registeredFunctions[function.Id] = function;
        return this;
    }

    public PerfonarFunctionResolver RegisterDefaults() => Register(
        new PerfonarFunction<Measurement>("n", sample => sample.Size.AsMeasurement())
        { Legend = "Sample Size" },
        new PerfonarFunction<Measurement>("mean", sample => MeanEstimator.Instance.Estimate(sample))
        { Legend = "Arithmetic Average" },
        new PerfonarFunction<Measurement>("stddev", sample => Moments.Create(sample).StandardDeviation.WithUnit(sample.Unit))
        { Legend = "Standard Deviation" },
        new PerfonarFunction<Measurement>("min", sample => sample.Min()),
        new PerfonarFunction<Measurement>("max", sample => sample.Max()),
        new PerfonarFunction<Measurement>("median",
            sample => TrimmedHarrellDavisQuantileEstimator.Sqrt.Median(sample)),
        new PerfonarFunction<Measurement>("center",
            sample => CenterEstimator.Instance.Estimate(sample)),
        new PerfonarFunction<Measurement>("spread",
            sample => ShamosEstimator.Instance.Scale(sample).WithUnit(sample.Unit))
    );

    [SuppressMessage("ReSharper", "CanSimplifyDictionaryTryGetValueWithGetValueOrDefault")]
    public PerfonarFunction? Resolve(string id) =>
        registeredFunctions.TryGetValue(id.TrimStart(PerfonarSymbol.Function), out var function) ? function : null;
}
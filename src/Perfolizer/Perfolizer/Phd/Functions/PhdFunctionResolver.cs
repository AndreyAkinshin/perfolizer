using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Perfolizer.Extensions;
using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.GenericEstimators;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.ScaleEstimators;
using Perfolizer.Metrology;
using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Functions;

public class PhdFunctionResolver
{
    private readonly Dictionary<string, PhdFunction> registeredFunctions = new (StringComparer.OrdinalIgnoreCase);

    [PublicAPI]
    public PhdFunctionResolver Register(params PhdFunction[] functions)
    {
        foreach (var function in functions)
            registeredFunctions[function.Id] = function;
        return this;
    }

    public PhdFunctionResolver RegisterDefaults() => Register(
        new PhdFunction<Measurement>("n", sample => sample.Size.AsMeasurement())
            { Legend = "Sample Size" },
        new PhdFunction<Measurement>("mean", sample => sample.Mean().WithUnit(sample.Unit))
            { Legend = "Arithmetic Average" },
        new PhdFunction<Measurement>("stddev", sample => Moments.Create(sample).StandardDeviation.WithUnit(sample.Unit))
            { Legend = "Standard Deviation" },
        new PhdFunction<Measurement>("min", sample => sample.Min().WithUnit(sample.Unit)),
        new PhdFunction<Measurement>("max", sample => sample.Max().WithUnit(sample.Unit)),
        new PhdFunction<Measurement>("median",
            sample => TrimmedHarrellDavisQuantileEstimator.Sqrt.Median(sample).WithUnit(sample.Unit)),
        new PhdFunction<Measurement>("center",
            sample => HodgesLehmannEstimator.Instance.Median(sample).WithUnit(sample.Unit)),
        new PhdFunction<Measurement>("spread",
            sample => ShamosEstimator.Instance.Scale(sample).WithUnit(sample.Unit))
    );

    [SuppressMessage("ReSharper", "CanSimplifyDictionaryTryGetValueWithGetValueOrDefault")]
    public PhdFunction? Resolve(string id) =>
        registeredFunctions.TryGetValue(id.TrimStart(PhdSymbol.Function), out var function) ? function : null;
}
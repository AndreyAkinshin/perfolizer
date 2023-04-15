using System;
using Perfolizer.Common;

namespace Perfolizer.Mathematics.Randomization;

public class LimitedRandomGenerator : RandomGenerator
{
    private readonly double[] values;

    public LimitedRandomGenerator(double[] values)
    {
        Assertion.NotNullOrEmpty(nameof(values), values);
        this.values = values;
    }

    public LimitedRandomGenerator(double[] values, int seed) : base(seed)
    {
        Assertion.NotNullOrEmpty(nameof(values), values);
        this.values = values;
    }

    public LimitedRandomGenerator(double[] values, Random random) : base(random)
    {
        Assertion.NotNullOrEmpty(nameof(values), values);
        this.values = values;
    }

    public override double Next() => values[Random.Next(values.Length)];
}
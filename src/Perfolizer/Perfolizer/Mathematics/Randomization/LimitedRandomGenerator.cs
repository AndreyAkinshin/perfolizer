using Perfolizer.Common;
using Pragmastat.Randomization;

namespace Perfolizer.Mathematics.Randomization;

public class LimitedRandomGenerator : RandomGenerator
{
    private readonly double[] values;

    public LimitedRandomGenerator(double[] values)
    {
        Assertion.NotNullOrEmpty(nameof(values), values);
        this.values = values;
    }

    public LimitedRandomGenerator(double[] values, long seed) : base(seed)
    {
        Assertion.NotNullOrEmpty(nameof(values), values);
        this.values = values;
    }

    public LimitedRandomGenerator(double[] values, Rng rng) : base(rng)
    {
        Assertion.NotNullOrEmpty(nameof(values), values);
        this.values = values;
    }

    public override double Next() => values[(int)Rng.UniformInt(0, values.Length)];
}
using Pragmastat.Randomization;

namespace Perfolizer.Extensions;

internal static class RngExtensions
{
    public static double Uniform(this Rng rng, double min, double max)
    {
        return min + rng.Uniform() * (max - min);
    }
}
using Perfolizer.Mathematics.Selectors;
using Pragmastat.Randomization;

namespace Perfolizer.Demo;

public class QuickSelectAdaptiveDemo : IDemo
{
    public void Run()
    {
        var rng = new Rng(42);
        var data = rng.Shuffle(new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}).ToArray();
            
        var selector = new QuickSelectAdaptive();
        for (int i = 0; i < data.Length; i++)
            Console.WriteLine($"data[{i}] = {selector.Select(data, i)}");
    }
}
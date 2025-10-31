using Perfolizer.Mathematics.Common;
using Perfolizer.Mathematics.Distributions.ContinuousDistributions;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Reference;
using Pragmastat;

namespace Perfolizer.Simulations;

public class QuantileCiSimulation
{
    public void Run(string[] args)
    {
        var random = new Random(42);
        var sampleSizes = Enumerable.Range(2, 49).ToList();
        var confidenceLevels = new[] { ConfidenceLevel.L95 };
        var probabilities = new Probability[] { 0.5, 0.7, 0.9, 0.95, 0.99 };

        string tableHeader = "Dist " + string.Join(" ", sampleSizes.Select(it => it.ToString().PadLeft(4)));

        using var writer = new Writer();
        foreach (var probability in probabilities)
        foreach (var confidenceLevel in confidenceLevels)
        {
            writer.SectionStart();
            writer.WriteLine($"                              Quantile = {probability.Value:N2}, ConfidenceLevel = {confidenceLevel}");
            writer.WriteLine(tableHeader);
            foreach (var referenceDistribution in SyntheticLatencyBrendanGreggSet.Instance.Distributions)
            {
                writer.Write(referenceDistribution.Key.PadRight(5));
                foreach (int sampleSize in sampleSizes)
                {
                    double rate = CoveragePercentage(referenceDistribution.Distribution, probability, confidenceLevel, random,
                        sampleSize, 10_000);
                    string rateMessage = rate.ToString("N3") + " ";
                    if (rate > confidenceLevel.Value)
                        writer.WriteGood(rateMessage);
                    else if (rate > confidenceLevel.Value * 0.98)
                        writer.WriteMedium(rateMessage);
                    else
                        writer.WriteBad(rateMessage);
                }
                writer.WriteLine();
            }
            writer.SectionEnd();
            writer.WriteLine();
        }
    }

    private double CoveragePercentage(IContinuousDistribution distribution, Probability probability, ConfidenceLevel confidenceLevel,
        Random random, int sampleSize, int repetitions)
    {
        var generator = distribution.Random(random);
        var estimator = HarrellDavisQuantileEstimator.Instance;
        double trueValue = distribution.Quantile(probability);
        int success = 0;
        for (int i = 0; i < repetitions; i++)
        {
            var sample = new Sample(generator.Next(sampleSize));
            var estimatedCi = estimator
                .QuantileConfidenceIntervalEstimator(sample, probability)
                .ConfidenceInterval(confidenceLevel);
            if (estimatedCi.Contains(trueValue))
                success++;
        }
        return success * 1.0 / repetitions;
    }

    private class Writer : IDisposable
    {
        private readonly StreamWriter fileWriter = new("report.html");

        public void Write(string message)
        {
            fileWriter.Write(message);

            Console.Write(message);
        }

        public void WriteLine(string message = "")
        {
            fileWriter.Write(message);
            fileWriter.Write("<br />");

            Console.WriteLine(message);
        }

        private void FileWriteColored(string message, string color)
        {
            fileWriter.Write($"<span style='color: {color}'>{message}</span>");
        }

        public void WriteGood(string message)
        {
            FileWriteColored(message, "#28CC2D");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ResetColor();
        }

        public void WriteMedium(string message)
        {
            FileWriteColored(message, "#FFBF00");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
            Console.ResetColor();
        }

        public void WriteBad(string message)
        {
            FileWriteColored(message, "#D82E3F");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            Console.ResetColor();
        }

        public void SectionStart()
        {
            fileWriter.Write("<pre class='chroma'>");
        }

        public void SectionEnd()
        {
            fileWriter.WriteLine("</pre>");
        }

        public void Dispose()
        {
            fileWriter.Close();
        }
    }
}
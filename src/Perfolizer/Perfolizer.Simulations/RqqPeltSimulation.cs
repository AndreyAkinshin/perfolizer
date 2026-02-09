using System.Diagnostics;
using Perfolizer.Mathematics.Cpd;
using Perfolizer.Mathematics.QuantileEstimators;
using Perfolizer.Mathematics.Sequences;
using Perfolizer.SimulationTests.Cpd.TestDataSets;
using Pragmastat.Randomization;

namespace Perfolizer.Simulations;

public class RqqPeltSimulation : IDisposable
{
    private readonly StreamWriter writer;

    public RqqPeltSimulation() => writer = new StreamWriter("report.txt");
    public void Dispose() => writer.Close();

    private void PrintLine(string message = "")
    {
        writer.WriteLine(message);
        Console.WriteLine(message);
    }

    public void Run(string[] args)
    {
        var stopwatch = Stopwatch.StartNew();

        var fullDataSet = CpdReferenceDataSet.Generate(new Rng(42), 2);
        string dataSetArg = args.Length > 0 ? args[0] : "*";
        bool printReports = args.Contains("--reports");
        int limit = int.MaxValue;
        int limitArgIndex = Array.IndexOf(args, "--limit");
        if (limitArgIndex >= 0 && limitArgIndex < args.Length - 1)
            if (int.TryParse(args[limitArgIndex + 1], out int actualLimit))
                limit = actualLimit;

        var dataSet = dataSetArg == "*" ? fullDataSet : fullDataSet.Where(data => data.Name.Contains(dataSetArg)).ToList();
        if (limit < dataSet.Count)
        {
            dataSet = new Rng(42).Shuffle(dataSet);
            dataSet.RemoveRange(limit, dataSet.Count - limit);
        }

        if (dataSet.Count == 0)
        {
            PrintLine("DataSet is empty");
            return;
        }

        dataSet.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

        if (args.Contains("--tune"))
        {
            var heterogeneityFactors = new ArithmeticProgressionSequence(1.1, 0.05).GenerateArray(7);
            var sensitivities = new ArithmeticProgressionSequence(0.4, 0.02).GenerateArray(8);
            var quantileSets = new List<QuantileSet>
            {
                QuantileSet.Classic,
                QuantileSet.ArithmeticProgression(12, 0),
                QuantileSet.SteadyPlusArithmeticProgression(12, 7, -0.01),
                QuantileSet.ArithmeticProgressionWithRepetitions(12, 4, -0.1)
            };
            int quantileSetMaxLength = quantileSets.Max(s => s.Name.Length);

            var results =
                new List<(double HeterogeneityFactor, double Sensitivity, string QuantileSet, double MaxPenalty, double SumPenalty)>();
            foreach (double heterogeneityFactor in heterogeneityFactors)
                foreach (double sensitivity in sensitivities)
                    foreach (var quantileSet in quantileSets)
                    {
                        double homogeneityFactor = heterogeneityFactor - 1;
                        PrintLine(Separator('@'));
                        PrintLine(
                            $"@ HeterogeneityFactor = {heterogeneityFactor:0.0}, Sensitivity = {sensitivity:0.00}, QuantileSet = {quantileSet.Name}");
                        var detector = new RqqPeltChangePointDetector(
                            quantileSet.Probabilities,
                            quantileSet.Factors,
                            sensitivity: sensitivity,
                            heterogeneityFactor: heterogeneityFactor,
                            homogeneityFactor: homogeneityFactor);
                        var penalties = RunSingle(detector, dataSet, printReports);
                        results.Add((heterogeneityFactor, sensitivity, quantileSet.Name, penalties.Max(), penalties.Sum()));
                    }

            PrintLine(Separator('*'));
            PrintLine(Separator('*'));
            PrintLine(Separator('*'));
            results.Sort((a, b) =>
                Math.Sign(b.MaxPenalty.CompareTo(a.MaxPenalty)) * 10 + Math.Sign(b.SumPenalty.CompareTo(a.SumPenalty)));
            foreach ((double heterogeneityFactor, double sensitivity, string quantileSet, double maxPenalty,
                         double sumPenalty) in results)
                PrintLine(
                    $"{heterogeneityFactor:0.00} {sensitivity:0.00} {quantileSet.PadRight(quantileSetMaxLength)} : {maxPenalty} / {sumPenalty}");
        }
        else
            RunSingle(RqqPeltChangePointDetector.Instance, dataSet, printReports);

        stopwatch.Stop();
        PrintLine();
        PrintLine($"TotalTime = {stopwatch.Elapsed.TotalSeconds:0.0} sec");
    }

    private List<double> RunSingle(RqqPeltChangePointDetector detector, IReadOnlyList<CpdTestData> dataSet, bool printReports)
    {
        int maxNameLength = dataSet.Select(data => data.Name.Length).Max();

        var penalties = new List<double>();

        Parallel.ForEach(dataSet, testData =>
        {
            var changePointIndexes = detector.GetChangePointIndexes(testData.Values.ToArray());
            var verification = CpdTestDataVerification.Verify(testData, changePointIndexes);
            lock (detector)
            {
                penalties.Add(verification.Penalty);
                PrintLine($"{testData.Name.PadRight(maxNameLength)} : {verification.Penalty}");
                if (printReports)
                    PrintLine(verification.Report);
            }
        });

        PrintLine("  Sum  = " + penalties.Sum());
        PrintLine("  Max  = " + penalties.Max());
        foreach (double p in new[] { 0.5, 0.90, 0.99 })
        {
            string metric = $"P{p * 100}".PadRight(4);
            double estimate = HarrellDavisQuantileEstimator.Instance.Quantile(penalties, p);
            PrintLine($"  {metric} = {estimate:0.##}");
        }

        return penalties;
    }

    private static string Separator(char c) => new string(c, 80);

    private class QuantileSet
    {
        public string Name { get; }
        public double[] Probabilities { get; }
        public double[] Factors { get; }

        private QuantileSet(string name, double[] probabilities, double[] factors)
        {
            Name = name;
            Probabilities = probabilities;
            Factors = factors;
        }

        public static readonly QuantileSet Classic = new QuantileSet("[12/Classic]",
            new ArithmeticProgressionSequence(0, 1 / 11.0).GenerateArray(12),
            new[] { 1.0, 1.0, 1.0, 1.0, 1.0, 0.9, 0.9, 0.9, 0.9, 0.9, 0.8, 0.8 });

        public static QuantileSet ArithmeticProgression(int n, double step) => new QuantileSet($"[{n}@A{step}]",
            new ArithmeticProgressionSequence(0, 1.0 / (n - 1)).GenerateArray(n),
            new ArithmeticProgressionSequence(1, step).GenerateArray(n));

        public static QuantileSet SteadyPlusArithmeticProgression(int n, int steadyCount, double step) => new QuantileSet(
            $"[{n}@{steadyCount}A{step}]",
            new ArithmeticProgressionSequence(0, 1.0 / (n - 1)).GenerateArray(n),
            Enumerable.Repeat(1.0, steadyCount - 1).Concat(
                new ArithmeticProgressionSequence(1, step).GenerateArray(n - steadyCount + 1)).ToArray());

        public static QuantileSet ArithmeticProgressionWithRepetitions(int n, int rep, double step)
        {
            var factors = new double[n];
            for (int i = 0; i < n; i++)
            {
                int group = i / rep;
                factors[i] = 1.0 + group * step;
            }

            return new QuantileSet($"[{n}@{rep}AR{step}]",
                new ArithmeticProgressionSequence(0, 1.0 / (n - 1)).GenerateArray(n),
                factors);
        }
    }
}
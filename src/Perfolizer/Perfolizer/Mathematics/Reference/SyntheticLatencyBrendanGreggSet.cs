using Perfolizer.Mathematics.Distributions.ContinuousDistributions;

namespace Perfolizer.Mathematics.Reference
{
    /// <summary>
    /// Synthetic Latency Distributions by Brendan Gregg.
    /// The original set can be found here: 
    /// https://github.com/brendangregg/PerfModels/blob/8612f83119869e510e29196c6c49743445ae4559/da/da-libsynth.r
    /// </summary>
    public class SyntheticLatencyBrendanGreggSet : IReferenceDistributionSet
    {
        public static readonly IReferenceDistributionSet Instance = new SyntheticLatencyBrendanGreggSet();
        
        public string Key => "SLBG";
        public string Description => "Synthetic Latency Distributions by Brendan Gregg";
        public ReferenceDistribution[] Distributions { get; }

        private SyntheticLatencyBrendanGreggSet()
        {
            Distributions = new[]
            {
                new ReferenceDistribution("0", "uniform narrow", new UniformDistribution(500, 1500)),
                new ReferenceDistribution("1", "uniform wide", new UniformDistribution(0, 3000)),
                new ReferenceDistribution("2", "uniform outliers", new MixtureDistribution(
                    new[]
                    {
                        new UniformDistribution(500, 1500),
                        new UniformDistribution(1500, 10000)
                    },
                    new[] {0.99, 0.01})),
                new ReferenceDistribution("100", "unimodal normal narrow", new NormalDistribution(1000, 100)),
                new ReferenceDistribution("101", "unimodal normal medium", new NormalDistribution(1000, 200)),
                new ReferenceDistribution("102", "unimodal normal wide", new NormalDistribution(1000, 300)),
                new ReferenceDistribution("103", "unimodal normal with tail", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 2250)
                    }, new[] {0.96, 0.04})),
                new ReferenceDistribution("104", "unimodal normal wide", new NormalDistribution(1120, 700)),
                // TODO: 110 unimodal band reject
                new ReferenceDistribution("111", "uniform normal spike", new MixtureDistribution(
                    new[]
                    {
                        new NormalDistribution(1000, 200),
                        new NormalDistribution(750, 1)
                    },
                    new[] {0.98, 0.02})),
                // TODO: 112 unimodal normal fence
                // TODO: 113 unimodal normal quantized
                // TODO: 120 unimodal poisson
                // TODO: 121 unimodal poisson outliers
                new ReferenceDistribution("130", "unimodal pareto narrow", new ParetoDistribution(1000, 3)),
                new ReferenceDistribution("131", "unimodal pareto wide", new ParetoDistribution(1000, 10)),
                new ReferenceDistribution("140", "unimodal normal outliers 1% medium", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.99, 0.01})),
                new ReferenceDistribution("141", "unimodal normal outliers 1% far", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 10000)
                    }, new[] {0.99, 0.01})),
                new ReferenceDistribution("142", "unimodal normal outliers 1% very far", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 50000)
                    }, new[] {0.99, 0.01})),
                new ReferenceDistribution("143", "unimodal normal outliers 2%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.98, 0.02})),
                new ReferenceDistribution("144", "unimodal normal outliers 4%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.96, 0.04})),
                new ReferenceDistribution("145", "unimodal normal outliers 2% clustered", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new NormalDistribution(3000, 35)
                    }, new[] {0.98, 0.02})),
                new ReferenceDistribution("146", "unimodal normal outliers 4% close 1", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 2700)
                    }, new[] {0.96, 0.04})),
                new ReferenceDistribution("147", "unimodal normal outliers 4% close 2", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 2900)
                    }, new[] {0.96, 0.04})),
                new ReferenceDistribution("148", "unimodal normal outliers 4% close 3", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 3100)
                    }, new[] {0.96, 0.04})),
                new ReferenceDistribution("149", "unimodal normal outliers 4% close 4", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 3300)
                    }, new[] {0.96, 0.04})),
                new ReferenceDistribution("150", "unimodal normal outliers 4% close 5", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 3500)
                    }, new[] {0.96, 0.04})),
                new ReferenceDistribution("151", "unimodal normal outliers 4% close 6", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 3700)
                    }, new[] {0.96, 0.04})),
                new ReferenceDistribution("152", "unimodal normal outliers 4% close 7", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 3900)
                    }, new[] {0.96, 0.04})),
                new ReferenceDistribution("153", "unimodal normal outliers 0.5%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.995, 0.005})),
                new ReferenceDistribution("154", "unimodal normal outliers 0.2%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.998, 0.002})),
                new ReferenceDistribution("155", "unimodal normal outliers 0.1%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(1000, 200),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.999, 0.001})),
                new ReferenceDistribution("200", "bimodal normal very close", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(850, 110),
                        new NormalDistribution(1150, 110)
                    }, new[] {0.5, 0.5})),
                new ReferenceDistribution("201", "bimodal normal close", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(825, 110),
                        new NormalDistribution(1175, 110)
                    }, new[] {0.5, 0.5})),
                new ReferenceDistribution("202", "bimodal normal medium", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(750, 110),
                        new NormalDistribution(1250, 110)
                    }, new[] {0.5, 0.5})),
                new ReferenceDistribution("203", "bimodal normal far", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(600, 110),
                        new NormalDistribution(1400, 110)
                    }, new[] {0.5, 0.5})),
                new ReferenceDistribution("204", "bimodal normal outliers 1%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(750, 110),
                        new NormalDistribution(1250, 110),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.495, 0.495, 0.01})),
                new ReferenceDistribution("205", "bimodal normal outliers 2%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(750, 110),
                        new NormalDistribution(1250, 110),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.49, 0.49, 0.02})),
                new ReferenceDistribution("206", "bimodal normal outliers 4%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(750, 110),
                        new NormalDistribution(1250, 110),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.48, 0.48, 0.04})),
                new ReferenceDistribution("210", "bimodal normal major minor", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(750, 110),
                        new NormalDistribution(1250, 110)
                    }, new[] {0.7, 0.3})),
                new ReferenceDistribution("211", "bimodal normal minor major", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(750, 110),
                        new NormalDistribution(1250, 110)
                    }, new[] {0.3, 0.7})),
                new ReferenceDistribution("212", "bimodal normal major minor outliers", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(750, 110),
                        new NormalDistribution(1250, 110),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.695, 0.295, 0.01})),
                new ReferenceDistribution("213", "bimodal normal major minor outliers", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(750, 110),
                        new NormalDistribution(1250, 110),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.295, 0.695, 0.01})),
                new ReferenceDistribution("214", "bimodal far normal far outliers 1%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 150),
                        new NormalDistribution(2000, 300),
                        new UniformDistribution(1000, 180_000)
                    }, new[] {0.499, 0.499, 0.002})),
                new ReferenceDistribution("215", "bimodal very far normal far outliers 1%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 100),
                        new NormalDistribution(4000, 500),
                        new UniformDistribution(1000, 180_000)
                    }, new[] {0.499, 0.499, 0.002})),
                new ReferenceDistribution("216", "bimodal very far major minor outliers 1%", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 100),
                        new NormalDistribution(4000, 100),
                        new UniformDistribution(1000, 180_000)
                    }, new[] {0.667, 0.333, 0.002})),
                new ReferenceDistribution("300", "trimodal normal close", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(750, 90),
                        new NormalDistribution(1000, 90),
                        new NormalDistribution(1250, 90)
                    }, new[] {0.333, 0.334, 0.333})),
                new ReferenceDistribution("301", "trimodal normal medium", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 100),
                        new NormalDistribution(1000, 100),
                        new NormalDistribution(1500, 100)
                    }, new[] {0.333, 0.334, 0.333})),
                new ReferenceDistribution("302", "trimodal normal far", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 65),
                        new NormalDistribution(1000, 65),
                        new NormalDistribution(1500, 65)
                    }, new[] {0.333, 0.334, 0.333})),
                new ReferenceDistribution("303", "trimodal normal outliers", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 100),
                        new NormalDistribution(1000, 100),
                        new NormalDistribution(1500, 100),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.333, 0.334, 0.333, 0.01})),
                new ReferenceDistribution("304", "trimodal normal major medium minor", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 100),
                        new NormalDistribution(1000, 100),
                        new NormalDistribution(1500, 100)
                    }, new[] {0.50, 0.33, 0.17})),
                new ReferenceDistribution("305", "trimodal normal minor major minor", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 100),
                        new NormalDistribution(1000, 100),
                        new NormalDistribution(1500, 100)
                    }, new[] {0.25, 0.50, 0.25})),
                new ReferenceDistribution("306", "trimodal normal minor major medium", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 100),
                        new NormalDistribution(1000, 100),
                        new NormalDistribution(1500, 100)
                    }, new[] {0.17, 0.50, 0.33})),
                new ReferenceDistribution("307", "trimodal normal major minor medium", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(500, 100),
                        new NormalDistribution(1000, 100),
                        new NormalDistribution(1500, 100)
                    }, new[] {0.50, 0.17, 0.33})),
                new ReferenceDistribution("400", "quad normal close", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(700, 75),
                        new NormalDistribution(900, 75),
                        new NormalDistribution(1100, 75),
                        new NormalDistribution(1300, 75)
                    }, new[] {0.25, 0.25, 0.25, 0.25})),
                new ReferenceDistribution("401", "quad normal medium", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(700, 50),
                        new NormalDistribution(900, 50),
                        new NormalDistribution(1100, 50),
                        new NormalDistribution(1300, 50)
                    }, new[] {0.25, 0.25, 0.25, 0.25})),
                new ReferenceDistribution("402", "quad normal far", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(400, 60),
                        new NormalDistribution(800, 60),
                        new NormalDistribution(1200, 60),
                        new NormalDistribution(1600, 60)
                    }, new[] {0.25, 0.25, 0.25, 0.25})),
                new ReferenceDistribution("403", "quad normal outliers", new MixtureDistribution(
                    new IContinuousDistribution[]
                    {
                        new NormalDistribution(700, 50),
                        new NormalDistribution(900, 50),
                        new NormalDistribution(1100, 50),
                        new NormalDistribution(1300, 50),
                        new UniformDistribution(1000, 5000)
                    }, new[] {0.25, 0.25, 0.25, 0.25, 0.01}))
            };
        }
    }
}
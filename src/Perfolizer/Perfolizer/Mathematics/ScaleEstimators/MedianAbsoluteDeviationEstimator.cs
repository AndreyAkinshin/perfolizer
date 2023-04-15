using System;
using System.Collections.Generic;
using Perfolizer.Common;
using Perfolizer.Mathematics.QuantileEstimators;

namespace Perfolizer.Mathematics.ScaleEstimators;

/// <summary>
/// The median absolute deviation (MAD).
/// MAD = ConsistencyFactor * median(abs(x[i] - median(x)))
/// </summary>
public abstract class MedianAbsoluteDeviationEstimator : IScaleEstimator
{
    /// <summary>
    /// Invariant instance.
    /// Based on <see cref="SimpleQuantileEstimator"/>; ConsistencyFactor = 1.
    /// </summary>
    public static readonly MedianAbsoluteDeviationEstimator Invariant = new InvariantEstimator();

    /// <summary>
    /// Simple normalized instance.
    /// Based on <see cref="SimpleQuantileEstimator"/>; consistent with the standard deviation under normality.
    ///
    /// Consistency factors are taken from the following paper:
    /// Park, Chanseok, Haewon Kim, and Min Wang.
    /// “Investigation of finite-sample properties of robust location and scale estimators.”
    /// Communications in Statistics-Simulation and Computation (2020): 1-27.
    /// https://doi.org/10.1080/03610918.2019.1699114
    /// </summary>
    public static readonly MedianAbsoluteDeviationEstimator Simple = new SimpleNormalizedEstimator();
        
    /// <summary>
    /// Harrell-Davis normalized instance.
    /// Based on <see cref="HarrellDavisQuantileEstimator"/>; consistent with the standard deviation under normality.
    /// </summary>
    public static readonly MedianAbsoluteDeviationEstimator HarrellDavis = new HarrellDavisNormalizedEstimator();

    protected abstract double ScaleFactor(Sample sample);

    public abstract IQuantileEstimator QuantileEstimator { get; }

    double IScaleEstimator.Scale(Sample sample) => Mad(sample);

    public double Mad(Sample sample)
    {
        Assertion.NotNull(nameof(sample), sample);
        if (sample.Count == 1)
            return 0;

        double scaleFactor = ScaleFactor(sample);
        double median = QuantileEstimator.Median(sample);
        double[] deviations = new double[sample.Count];
        for (int i = 0; i < sample.Count; i++)
            deviations[i] = Math.Abs(sample.Values[i] - median);
        return scaleFactor * QuantileEstimator.Median(new Sample(deviations));
    }

    public double LowerMad(Sample sample)
    {
        Assertion.NotNull(nameof(sample), sample);
        if (sample.Count == 1)
            return 0;

        double scaleFactor = ScaleFactor(sample);
        double median = QuantileEstimator.Median(sample);
        var deviations = new List<double>(sample.Count);
        for (int i = 0; i < sample.Count; i++)
            if (sample.Values[i] <= median)
                deviations.Add(Math.Abs(sample.Values[i] - median));
        return scaleFactor * QuantileEstimator.Median(new Sample(deviations));
    }

    public double UpperMad(Sample sample)
    {
        Assertion.NotNull(nameof(sample), sample);
        if (sample.Count == 1)
            return 0;

        double scaleFactor = ScaleFactor(sample);
        double median = QuantileEstimator.Median(sample);
        var deviations = new List<double>(sample.Count);
        for (int i = 0; i < sample.Count; i++)
            if (sample.Values[i] >= median)
                deviations.Add(Math.Abs(sample.Values[i] - median));
        return scaleFactor * QuantileEstimator.Median(new Sample(deviations));
    }

    private class InvariantEstimator : MedianAbsoluteDeviationEstimator
    {
        protected override double ScaleFactor(Sample sample) => 1;
        public override IQuantileEstimator QuantileEstimator => SimpleQuantileEstimator.Instance;
    }

    private class SimpleNormalizedEstimator : MedianAbsoluteDeviationEstimator
    {
        public override IQuantileEstimator QuantileEstimator => SimpleQuantileEstimator.Instance;

        private static readonly double[] ParkBias =
        {
            double.NaN, double.NaN, -0.163388, -0.3275897, -0.2648275, -0.178125, -0.1594213,
            -0.1210631, -0.1131928, -0.0920658, -0.0874503, -0.0741303, -0.0711412,
            -0.0620918, -0.060021, -0.0534603, -0.0519047, -0.0467319, -0.0455579,
            -0.0417554, -0.0408248, -0.0376967, -0.036835, -0.0342394, -0.033539,
            -0.0313065, -0.0309765, -0.029022, -0.0287074, -0.0269133, -0.0265451,
            -0.0250734, -0.0248177, -0.023646, -0.0232808, -0.0222099, -0.0220756,
            -0.0210129, -0.0207309, -0.0199272, -0.019714, -0.0188446, -0.0188203,
            -0.0180521, -0.0178185, -0.0171866, -0.0170796, -0.0165391, -0.0163509,
            -0.0157862, -0.0157372, -0.015282, -0.0149951, -0.0146042, -0.0145007,
            -0.0140391, -0.0139674, -0.0136336, -0.0134819, -0.0130812, -0.0129708,
            -0.0126589, -0.0125598, -0.0122696, -0.0121523, -0.0118163, -0.0118244,
            -0.0115177, -0.0114479, -0.0111309, -0.0110816, -0.0108875, -0.0108319,
            -0.0106032, -0.0105424, -0.0102237, -0.0102132, -0.0099408, -0.0099776,
            -0.0097815, -0.0097399, -0.0094837, -0.0094713, -0.009239, -0.0092875,
            -0.0091508, -0.0090145, -0.0088191, -0.0088205, -0.0086622, -0.0085714,
            -0.0084718, -0.0083861, -0.0082559, -0.008265, -0.0080977, -0.0080708,
            -0.007881, -0.0078492, -0.0077043, -0.0077614
        };

        public static double ScaleFactor(int n)
        {
            double bias = n < ParkBias.Length - 1 ? ParkBias[n] : -0.76213 / n - 0.86413 / n / n;
            return 1 / (0.674489750196082 * (1 + bias));
        }

        protected override double ScaleFactor(Sample sample)
        {
            return ScaleFactor(sample.Count);
        }
    }

    private class HarrellDavisNormalizedEstimator : MedianAbsoluteDeviationEstimator
    {
        public override IQuantileEstimator QuantileEstimator => HarrellDavisQuantileEstimator.Instance;

        /// <summary>
        /// See https://aakinshin.net/posts/unbiased-mad-hd/
        /// </summary>
        private static readonly double[] InverseFactors =
        {
            double.NaN, double.NaN, 0.5641745553088423, 0.637689464277234, 0.6266114358022791, 0.6385252621371729, 0.6383396054837751,
            0.6391483024649844, 0.6414054128515725, 0.6423654653358132, 0.6439650633303657, 0.6453466713309995, 0.6466167720829089,
            0.647896194684998, 0.6490803621640332, 0.6501815281116196, 0.6512459087349248, 0.6522589758796201, 0.653165439546788,
            0.6540376403452666, 0.6548870130335538, 0.6556512800977367, 0.656379724662582, 0.657075137823794, 0.6577094288730985,
            0.6583179057553981, 0.6588811618092122, 0.6594277977978977, 0.659908870089771, 0.6603588047247171, 0.6608151995722138,
            0.6612328229852198, 0.6616140717545177, 0.6620030572297076, 0.6623539917300408, 0.6626959754627264, 0.6630223332045908,
            0.6633360054645671, 0.6636243283748828, 0.6639105168758841, 0.6641720839075258, 0.664434153066093, 0.6646879048074016,
            0.6649254820008966, 0.6651534642632377, 0.6653857164019107, 0.6655749803090755, 0.6657801861663875, 0.6659788048283622,
            0.6661560720144832, 0.6663297834046026, 0.666492997774477, 0.66667768027542, 0.6668167805264079, 0.6669936882784248,
            0.6671333902749049, 0.6672768469804923, 0.6674118209231669, 0.6675329032333017, 0.6676705801665649, 0.6677988084612649,
            0.6679113433053215, 0.6680284336199781, 0.6681499690725486, 0.6682525518350259, 0.6683567067565843, 0.668460965004546,
            0.6685662390024206, 0.6686511224106276, 0.6687586776357963, 0.6688316676951251, 0.6689284932354674, 0.6690095175560199,
            0.6690950890783779, 0.6691790078669322, 0.6692461369136855, 0.6693343233247978, 0.6694005551533061, 0.6694801807393673,
            0.6695488689419441, 0.6696179041146787, 0.669682207645742, 0.6697388884954646, 0.6698006816087986, 0.6698773600892196,
            0.6699283460577496, 0.6699865494232141, 0.6700459021371715, 0.6700932244841693, 0.6701564319389123, 0.6702090710385918,
            0.6702581431297143, 0.6703089257542638, 0.6703600426491966, 0.6704145194361312, 0.6704554666719077, 0.6704917442933176,
            0.6705506553462088, 0.6705950233298209, 0.6706268391507145, 0.670683579816257
        };

        private static double ScaleFactor(int n)
        {
            double inverseFactor = n < InverseFactors.Length - 1
                ? InverseFactors[n]
                : 0.674489750196082 * (1 - 0.5 / n + 6.5 / n / n);
            return 1.0 / inverseFactor;
        }

        protected override double ScaleFactor(Sample sample) => ScaleFactor(sample.Count);
    }
}
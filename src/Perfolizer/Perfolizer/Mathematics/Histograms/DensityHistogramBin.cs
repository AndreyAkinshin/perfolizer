namespace Perfolizer.Mathematics.Histograms
{
    public class DensityHistogramBin
    {
        public double Lower { get; }
        public double Upper { get; }
        public double Height { get; }

        public double Middle => (Lower + Upper) / 2;

        public DensityHistogramBin(double lower, double upper, double height)
        {
            Lower = lower;
            Upper = upper;
            Height = height;
        }
    }
}
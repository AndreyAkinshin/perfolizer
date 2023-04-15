using System;

namespace Perfolizer.Mathematics.Histograms;

public static class HistogramBinSizeCalculator
{
    public static double CalcFreedmanDiaconis(int n, double interquartileRange)
    {
        return 2 * interquartileRange / Math.Pow(n, 1.0 / 3);
    }

    public static double CalcScott(int n, double standardDeviation)
    {
        return 3.5 * standardDeviation / Math.Pow(n, 1.0 / 3);
    }
        
    public static double CalcScott2(int n, double standardDeviation)
    {
        return 3.5 * standardDeviation / Math.Pow(n, 1.0 / 3) / 2.0;
    }

    public static double CalcSquareRoot(int n, double min, double max)
    {
        return (max - min) / Math.Sqrt(n);
    }

    public static double CalcSturges(int n, double min, double max)
    {
        return (max - min) / (Math.Ceiling(Math.Log(n, 2)) + 1);
    }
        
    public static double CalcRice(int n, double min, double max)
    {
        return (max - min) / (2 * Math.Pow(n, 1.0 / 3));
    }
}
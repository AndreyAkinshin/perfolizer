namespace Perfolizer.Mathematics.Functions;

public class InverseMonotonousFunction
{
    private readonly Func<double, double> referenceFunction;
    private readonly double min, max;
    private readonly bool isIncreasing;

    public InverseMonotonousFunction(Func<double, double> referenceFunction,
        double min = double.MinValue / 3, double max = double.MaxValue / 3)
    {
        this.referenceFunction = referenceFunction;
        this.min = min;
        this.max = max;
        isIncreasing = referenceFunction(max) > referenceFunction(min);
    }

    public double Value(double x, double eps = 1e-9)
    {
        double lower = min;
        double upper = max;
        while (upper - lower > eps)
        {
            double middle = (lower + upper) / 2;
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (lower == middle || upper == middle)
                break;

            double referenceValue = referenceFunction(middle);
            if (referenceValue < x ^ isIncreasing)
                upper = middle;
            else
                lower = middle;
        }

        return (lower + upper) / 2;
    }
}
namespace Perfolizer.Mathematics.OutlierDetection;

public interface IOutlierDetector
{
    bool IsLowerOutlier(double x);
    bool IsUpperOutlier(double x);
}
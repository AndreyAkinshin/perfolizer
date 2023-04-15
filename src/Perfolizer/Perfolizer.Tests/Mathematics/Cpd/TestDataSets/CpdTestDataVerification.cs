using System;
using System.Text;

namespace Perfolizer.Tests.Mathematics.Cpd.TestDataSets;

public class CpdTestDataVerification
{
    public double Penalty { get; }
    public string Report { get; }

    private CpdTestDataVerification(double penalty, string report)
    {
        Penalty = penalty;
        Report = report;
    }

    public static CpdTestDataVerification Verify(CpdTestData testData, int[] actualIndexes)
    {
        var expectedChangePoints = testData.ExpectedChangePoints;
        var penalties = testData.Penalties;
        int actualCount = actualIndexes.Length;
        int expectedCount = expectedChangePoints.Count;
        int maxAcceptableMissingPointCount = testData.MaxAcceptableMissingPointCount;
        int indexMaxWidth = Math.Max(testData.Values.Count.ToString().Length, 3);
        double penalty = 0;
        int missingPointCounter = 0;

        var report = new StringBuilder();
        report.AppendLine($"*** Report for {testData.Name} ***");
        report.AppendLine($"{"Exp".PadRight(indexMaxWidth)} {"Act".PadRight(indexMaxWidth)} Penalty");

        void AddPenalty(int expectedIndex, int actualIndex, double indexPenalty, string comment)
        {
            penalty += indexPenalty;
            string expectedMessage = (expectedIndex == -1 ? "-" : expectedIndex.ToString()).PadRight(indexMaxWidth);
            string actualMessage = (actualIndex == -1 ? "-" : actualIndex.ToString()).PadRight(indexMaxWidth);
            string penaltyMessage = indexPenalty.ToString("0.00").PadLeft(7);
            string commentMessage = string.IsNullOrEmpty(comment) ? "" : $"  {comment}";
            report.AppendLine($"{expectedMessage} {actualMessage} {penaltyMessage}{commentMessage}");
        }

        int actualPointer = 0;
        for (int expectedPointer = 0; expectedPointer < expectedCount; expectedPointer++)
        {
            var expectedPoint = expectedChangePoints[expectedPointer];
            int expectedIndex = expectedPoint.Index;
            while (actualPointer < actualCount - 1 &&
                   Math.Abs(expectedIndex - actualIndexes[actualPointer]) >
                   Math.Abs(expectedIndex - actualIndexes[actualPointer + 1]))
            {
                AddPenalty(-1, actualIndexes[actualPointer], penalties.ExtraPoint, "ExtraPoint");
                actualPointer++;
            }

            if (actualPointer < actualCount && Math.Abs(expectedIndex - actualIndexes[actualPointer]) <= expectedPoint.MaxDisplacement)
            {
                int displacement = Math.Abs(expectedIndex - actualIndexes[actualPointer]);
                int extraDisplacement = Math.Max(0, displacement - expectedPoint.AcceptableDisplacement);
                AddPenalty(expectedIndex, actualIndexes[actualPointer],
                    penalties.Displacement * extraDisplacement * expectedPoint.Importance,
                    displacement > 0 ? $"Displacement(Max={expectedPoint.AcceptableDisplacement})" : "");
                actualPointer++;
            }
            else
            {
                if (missingPointCounter++ < maxAcceptableMissingPointCount)
                    AddPenalty(expectedIndex, -1, penalties.AcceptableMissingPoint, "AcceptableMissingPoint");
                else
                {
                    if (Math.Abs(expectedPoint.Importance - 1) < 1e-9)
                        AddPenalty(expectedIndex, -1, penalties.MissingPoint, "MissingPoint");
                    else
                        AddPenalty(expectedIndex, -1, penalties.MissingPoint * expectedPoint.Importance,
                            $"MissingPoint(Importance={expectedPoint.Importance:0.00})");
                }
            }
        }

        while (actualPointer < actualCount)
        {
            AddPenalty(-1, actualIndexes[actualPointer], penalties.ExtraPoint, "ExtraPoint");
            actualPointer++;
        }

        return new CpdTestDataVerification(penalty, report.ToString().Trim());
    }
}
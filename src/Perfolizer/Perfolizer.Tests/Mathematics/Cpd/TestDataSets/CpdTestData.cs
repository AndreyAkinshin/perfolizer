using System.Collections.Generic;

namespace Perfolizer.Tests.Mathematics.Cpd.TestDataSets
{
    public class CpdTestData
    {
        public class ExpectedChangePoint
        {
            public int Index { get; }
            public int AcceptableDisplacement { get; }
            public int MaxDisplacement { get; }
            public double Importance { get; }

            public ExpectedChangePoint(int index, int acceptableDisplacement, int maxDisplacement, double importance = 1.0)
            {
                Index = index;
                AcceptableDisplacement = acceptableDisplacement;
                MaxDisplacement = maxDisplacement;
                Importance = importance;
            }
        }

        public class PenaltyValues
        {
            public static readonly PenaltyValues Default = new PenaltyValues(100, 20, 20, 1);
            public static readonly PenaltyValues Light = new PenaltyValues(100, 20, 2, 1);

            public int MissingPoint { get; }
            public int AcceptableMissingPoint { get; }
            public int ExtraPoint { get; }
            public int Displacement { get; }

            public PenaltyValues(int missingPoint, int acceptableMissingPoint, int extraPoint, int displacement)
            {
                MissingPoint = missingPoint;
                ExtraPoint = extraPoint;
                Displacement = displacement;
                AcceptableMissingPoint = acceptableMissingPoint;
            }
        }

        public string Name { get; }
        public IReadOnlyList<double> Values { get; }
        public IReadOnlyList<ExpectedChangePoint> ExpectedChangePoints { get; }

        public PenaltyValues Penalties { get; }
        public int MaxAcceptableMissingPointCount { get; }

        public CpdTestData(string name, IReadOnlyList<double> values, IReadOnlyList<ExpectedChangePoint> expectedChangePoints,
            PenaltyValues penalties = null, int maxAcceptableMissingPointCount = 0)
        {
            Name = name;
            Values = values;
            ExpectedChangePoints = expectedChangePoints;
            MaxAcceptableMissingPointCount = maxAcceptableMissingPointCount;
            Penalties = penalties ?? PenaltyValues.Default;
        }

        public override string ToString() => Name;
    }
}
using System;

namespace Perfolizer.Mathematics.Sequences
{
    public class ExponentialDecaySequence : ISequence
    {
        private readonly double initialValue, decayConstant;

        public ExponentialDecaySequence(double initialValue, double decayConstant)
        {
            this.initialValue = initialValue;
            this.decayConstant = decayConstant;
        }

        public double Value(int index) => initialValue * Math.Exp(-decayConstant * index);

        public static ISequence CreateFromHalfLife(int halfLife) => new ExponentialDecaySequence(1, Math.Log(2) / halfLife);
    }
}
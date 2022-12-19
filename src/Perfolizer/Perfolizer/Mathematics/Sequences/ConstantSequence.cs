namespace Perfolizer.Mathematics.Sequences
{
    public class ConstantSequence : ISequence
    {
        public static readonly ConstantSequence Zero = new ConstantSequence(0);
        public static readonly ConstantSequence NaN = new ConstantSequence(double.NaN);
        public static readonly ConstantSequence PositiveInfinity = new ConstantSequence(double.PositiveInfinity);
        public static readonly ConstantSequence NegativeInfinity = new ConstantSequence(double.NegativeInfinity);
        
        private readonly double value;

        public ConstantSequence(double value)
        {
            this.value = value;
        }

        public double Value(int index) => value;
    }
}
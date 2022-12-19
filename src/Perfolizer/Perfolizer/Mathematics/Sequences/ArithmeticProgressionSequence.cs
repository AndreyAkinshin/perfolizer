namespace Perfolizer.Mathematics.Sequences
{
    public class ArithmeticProgressionSequence : ISequence
    {
        private readonly double start, step;

        public ArithmeticProgressionSequence(double start, double step)
        {
            this.start = start;
            this.step = step;
        }

        public double Value(int index) => start + index * step;
    }
}
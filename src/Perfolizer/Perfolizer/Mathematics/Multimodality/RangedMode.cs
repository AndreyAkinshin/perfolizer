namespace Perfolizer.Mathematics.Multimodality
{
    public class RangedMode
    {
        public double Location { get; }
        public double Left { get; }
        public double Right { get; }

        public RangedMode(double location, double left, double right)
        {
            Location = location;
            Left = left;
            Right = right;
        }
    }
}
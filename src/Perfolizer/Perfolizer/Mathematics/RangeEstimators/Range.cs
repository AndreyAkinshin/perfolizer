namespace Perfolizer.Mathematics.RangeEstimators
{
    public readonly struct Range
    {
        public double Left { get; }
        public double Right { get; }

        public Range(double left, double right)
        {
            Left = left;
            Right = right;
        }

        public bool IsInside(Range outerRange)
        {
            return outerRange.Left <= Left && Right <= outerRange.Right;
        }

        public override string ToString() => $"[{Left};{Right}]";
    }
}
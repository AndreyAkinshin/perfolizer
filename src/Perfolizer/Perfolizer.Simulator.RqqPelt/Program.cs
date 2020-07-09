namespace Perfolizer.Simulator.RqqPelt
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using var runner = new Runner();
            runner.Run(args);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Perfolizer.Demo
{
    static class Program
    {
        private class DemoItem
        {
            private readonly Type type;

            public DemoItem(Type type)
            {
                this.type = type;
            }

            public string Title => type.Name.Replace("Demo", "");

            public bool Matches(string name) =>
                string.Equals(Title, name, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase);

            public void Run()
            {
                var demo = Activator.CreateInstance(type) as IDemo;
                if (demo == null)
                    throw new Exception($"Type {type.Name} doesn't implement {nameof(IDemo)}");
                demo.Run();
            }
        }

        private static readonly List<DemoItem> Demos = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(IDemo).IsAssignableFrom(type))
            .Select(type => new DemoItem(type))
            .ToList();

        private static void PrintAvailableDemos()
        {
            Console.WriteLine("Available demos:");
            foreach (var demo in Demos)
                Console.WriteLine($"* {demo.Title}");
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("You should provide the Demo title.");
                PrintAvailableDemos();
                return;
            }

            string demoName = args[0];
            var demo = Demos.FirstOrDefault(d => d.Matches(demoName));
            if (demo == null)
            {
                Console.WriteLine($"'{demoName}' is not a valid demo name.");
                PrintAvailableDemos();
                return;
            }

            demo.Run();
        }
    }
}
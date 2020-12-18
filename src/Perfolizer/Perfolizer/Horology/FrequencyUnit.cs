using System.Linq;
using JetBrains.Annotations;

namespace Perfolizer.Horology
{
    public class FrequencyUnit
    {
        public string Name { get; }
        public string Description { get; }
        public long HertzAmount { get; }

        private FrequencyUnit(string name, string description, long hertzAmount)
        {
            Name = name;
            Description = description;
            HertzAmount = hertzAmount;
        }

        public static readonly FrequencyUnit Hz = new("Hz", "Hertz", 1);
        public static readonly FrequencyUnit KHz = new("KHz", "Kilohertz", 1000);
        public static readonly FrequencyUnit MHz = new("MHz", "Megahertz", 1000 * 1000);
        public static readonly FrequencyUnit GHz = new("GHz", "Gigahertz", 1000 * 1000 * 1000);
        public static readonly FrequencyUnit[] All = { Hz, KHz, MHz, GHz };

        public Frequency ToFrequency(long value = 1) => new(value, this);
        
        public static double Convert(double value, [NotNull] FrequencyUnit from, [CanBeNull] FrequencyUnit to) =>
            value * from.HertzAmount / (to ?? GetBestFrequencyUnit(value)).HertzAmount;

        /// <summary>
        /// This method chooses the best frequency unit for representing a set of frequency measurements.
        /// </summary>
        /// <param name="values">The list of frequency measurements in hertz.</param>
        /// <returns>Best frequency unit.</returns>
        public static FrequencyUnit GetBestFrequencyUnit(params double[] values)
        {
            if (values.Length == 0)
                return Hz;
            // Use the largest unit to display the smallest recorded measurement without loss of precision.
            double minValue = values.Min();
            foreach (var frequencyUnit in All)
                if (minValue < frequencyUnit.HertzAmount * 1000)
                    return frequencyUnit;
            return All.Last();
        }
    }
}
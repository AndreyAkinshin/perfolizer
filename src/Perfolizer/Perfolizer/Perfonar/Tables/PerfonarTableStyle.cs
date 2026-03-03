using Perfolizer.Common;

namespace Perfolizer.Perfonar.Tables;

public class PerfonarTableStyle
{
    public IFormatProvider FormatProvider { get; set; } = DefaultCultureInfo.Instance;
    public int PreferredWidth { get; set; } = 100;
}

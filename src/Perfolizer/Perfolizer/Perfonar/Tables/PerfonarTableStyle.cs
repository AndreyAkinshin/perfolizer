using Perfolizer.Common;
using Perfolizer.Metrology;

namespace Perfolizer.Perfonar.Tables;

public class PerfonarTableStyle
{
    public IFormatProvider FormatProvider { get; set; } = DefaultCultureInfo.Instance;
    public UnitPresentation UnitPresentation { get; set; } = UnitPresentation.WithGap;
    public int PreferredWidth { get; set; } = 100;
}
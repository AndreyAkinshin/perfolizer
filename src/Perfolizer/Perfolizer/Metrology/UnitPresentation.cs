using JetBrains.Annotations;

namespace Perfolizer.Metrology;

public class UnitPresentation(bool isVisible, int minUnitWidth, bool gap = false, bool forceAscii = false)
{
    [PublicAPI] public static readonly UnitPresentation Default = new(true, 0);
    [PublicAPI] public static readonly UnitPresentation WithGap = new(true, 0, gap: true);
    [PublicAPI] public static readonly UnitPresentation Ascii = new(true, 0, forceAscii: true);
    [PublicAPI] public static readonly UnitPresentation Invisible = new(false, 0);

    public bool IsVisible { get; private set; } = isVisible;
    public int MinUnitWidth { get; private set; } = minUnitWidth;
    public bool Gap { get; private set; } = gap;
    public bool ForceAscii { get; private set; } = forceAscii;

    [PublicAPI] public static UnitPresentation FromVisibility(bool isVisible) => new(isVisible, 0);
    [PublicAPI] public static UnitPresentation FromWidth(int unitWidth) => new(true, unitWidth);
}
using Perfolizer.Common;
using Perfolizer.Helpers;
using Perfolizer.Horology;

namespace Perfolizer.Metrology;

public abstract class MeasurementUnit(string abbreviation, string fullName, long baseUnits)
    : IEquatable<MeasurementUnit>
{
    public static IEnumerable<MeasurementUnit> GetAll()
    {
        foreach (var unit in TimeUnit.All)
            yield return unit;
        foreach (var unit in SizeUnit.All)
            yield return unit;
        yield return NumberUnit.Instance;
        yield return PercentUnit.Instance;
        yield return EffectSizeUnit.Instance;
        yield return RatioUnit.Instance;
    }

    public string Abbreviation { get; } = abbreviation;
    public string FullName { get; } = fullName;
    public long BaseUnits { get; } = baseUnits;

    public string AbbreviationAscii => Abbreviation.ConvertToAscii();

    public override string ToString() => Abbreviation;

    public string ToString(UnitPresentation? unitPresentation = null)
    {
        unitPresentation ??= UnitPresentation.Default;
        if (!unitPresentation.IsVisible)
            return "";

        string abbreviation = unitPresentation.ForceAscii ? AbbreviationAscii : Abbreviation;
        string unitName = abbreviation.PadLeft(unitPresentation.MinUnitWidth);
        string gap = unitPresentation.Gap ? " " : "";
        return $"{gap}{unitName}";
    }


    public bool Equals(MeasurementUnit other) =>
        Abbreviation == other.Abbreviation &&
        FullName == other.FullName &&
        BaseUnits == other.BaseUnits;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((MeasurementUnit)obj);
    }

    public override int GetHashCode() => HashCodeHelper.Combine(Abbreviation, FullName, BaseUnits);
    public static bool operator ==(MeasurementUnit? left, MeasurementUnit? right) => Equals(left, right);
    public static bool operator !=(MeasurementUnit? left, MeasurementUnit? right) => !Equals(left, right);
}
using Perfolizer.Extensions;

namespace Perfolizer.Perfonar.Base;

public class PerfonarKey(string path, Type type) : IEquatable<PerfonarKey>
{
    public static readonly PerfonarKey Empty = new("", typeof(object));

    public string Path { get; } = path;
    public Type Type { get; } = type;

    public string Name { get; } = path.Split(PerfonarSymbol.Attribute).DefaultIfEmpty("").Last();
    public override string ToString() => Path;

    public bool IsComposite() => !Type.IsPerfonarPrimitive();

    public bool IsMatched(string selector) =>
        Path.EquationsIgnoreCase(selector) ||
        Path.StartWithIgnoreCase(selector) ||
        Name.EquationsIgnoreCase(selector);

    public bool Equals(PerfonarKey? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Path == other.Path;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((PerfonarKey)obj);
    }

    public override int GetHashCode() => Path.GetHashCode();
    public static bool operator ==(PerfonarKey? left, PerfonarKey? right) => Equals(left, right);
    public static bool operator !=(PerfonarKey? left, PerfonarKey? right) => !Equals(left, right);

    public PerfonarKey Append(string subName, Type subType) =>
        new($"{Path}{PerfonarSymbol.Attribute}{subName.ToCamelCase()}", subType);
}
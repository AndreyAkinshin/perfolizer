using Perfolizer.Extensions;

namespace Perfolizer.Phd.Base;

public class PhdKey(string path, Type type) : IEquatable<PhdKey>
{
    public static readonly PhdKey Empty = new ("", typeof(object));

    public string Path { get; } = path;
    public Type Type { get; } = type;

    public string Name { get; } = path.Split(PhdSymbol.Attribute).DefaultIfEmpty("").Last();
    public override string ToString() => Path;

    public bool IsComposite() => !Type.IsPhdPrimitive();

    public bool IsMatched(string selector) =>
        Path.EquationsIgnoreCase(selector) ||
        Path.StartWithIgnoreCase(selector) ||
        Name.EquationsIgnoreCase(selector);

    public bool Equals(PhdKey? other)
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
        return Equals((PhdKey)obj);
    }

    public override int GetHashCode() => Path.GetHashCode();
    public static bool operator ==(PhdKey? left, PhdKey? right) => Equals(left, right);
    public static bool operator !=(PhdKey? left, PhdKey? right) => !Equals(left, right);

    public PhdKey Append(string subName, Type subType) =>
        new ($"{Path}{PhdSymbol.Attribute}{subName.ToCamelCase()}", subType);
}
namespace Perfolizer.Phd.Base;

public class PhdSchema(string name)
{
    public string Name { get; } = name;
    private readonly List<PhdImplementation> implementations = [];
    public IReadOnlyList<PhdImplementation> Implementations => implementations;

    public PhdSchema Add<T>() where T : PhdObject
    {
        var baseType = GetBaseType<T>();
        if (baseType == null)
            throw new InvalidCastException($"Failed to case {typeof(T).Name} to {nameof(PhdObject)}");
        implementations.Add(new PhdImplementation(baseType, typeof(T)));
        return this;
    }

    private static Type? GetBaseType<T>()
    {
        var t = typeof(T);
        while (t != null && t.Assembly != typeof(PhdObject).Assembly)
            t = t.BaseType;
        return t;
    }
}

public class PhdImplementation(Type @base, Type derived)
{
    public Type Base { get; } = @base;
    public Type Derived { get; } = derived;
}
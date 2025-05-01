using Perfolizer.Models;

namespace Perfolizer.Perfonar.Base;

public class PerfonarSchema(string name)
{
    public string Name { get; } = name;
    private readonly List<PerfonarImplementation> implementations = [];
    public IReadOnlyList<PerfonarImplementation> Implementations => implementations;

    public PerfonarSchema Add<T>() where T : AbstractInfo
    {
        var baseType = GetBaseType<T>();
        if (baseType == null)
            throw new InvalidCastException($"Failed to case {typeof(T).Name} to {nameof(AbstractInfo)}");
        implementations.Add(new PerfonarImplementation(baseType, typeof(T)));
        return this;
    }

    private static Type? GetBaseType<T>()
    {
        var t = typeof(T);
        while (t != null && t.Assembly != typeof(AbstractInfo).Assembly)
            t = t.BaseType;
        return t;
    }
}

public class PerfonarImplementation(Type @base, Type derived)
{
    public Type Base { get; } = @base;
    public Type Derived { get; } = derived;
}
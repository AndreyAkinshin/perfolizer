using Perfolizer.Phd.Functions;

namespace Perfolizer.Phd.Tables;

public class PhdFunctionColumn(PhdColumnDefinition definition, PhdFunction function)
    : PhdColumn(function.Title, definition.Selector, definition)
{
    public PhdFunction Function { get; } = function;
}

public class PhdFunctionColumn<T>(PhdColumnDefinition definition, PhdFunction<T> function)
    : PhdFunctionColumn(definition, function)
{
    public new PhdFunction<T> Function { get; } = function;
}
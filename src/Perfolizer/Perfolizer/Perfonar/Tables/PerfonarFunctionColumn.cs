using Perfolizer.Perfonar.Functions;

namespace Perfolizer.Perfonar.Tables;

public class PerfonarFunctionColumn(PerfonarColumnDefinition definition, PerfonarFunction function)
    : PerfonarColumn(function.Title, definition.Selector, definition)
{
    public PerfonarFunction Function { get; } = function;
}

public class PerfonarFunctionColumn<T>(PerfonarColumnDefinition definition, PerfonarFunction<T> function)
    : PerfonarFunctionColumn(definition, function)
{
    public new PerfonarFunction<T> Function { get; } = function;
}
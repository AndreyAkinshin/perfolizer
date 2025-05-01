using System.Diagnostics;
using Perfolizer.Extensions;
using Perfolizer.InfoModels;
using Perfolizer.Perfonar.Base;

namespace Perfolizer.Perfonar.Tables;

[DebuggerDisplay("{Selector}")]
public class PerfonarColumnDefinition(string selector) : AbstractInfo
{
    public string Title { get; set; } = "";
    public string Selector { get; set; } = selector;

    /// <summary>
    /// Group for shared cell clouds
    /// </summary>
    public string Cloud { get; set; } = "";

    /// <summary>
    /// If true, when presented in the shared section, the title is omitted.
    /// </summary>
    public bool? IsSelfExplanatory { get; set; }

    /// <summary>
    /// Controls whether the column can be presented in the shared section
    /// </summary>
    public bool? CanBeShared { get; set; }

    /// <summary>
    /// Controls whether the column can be compressed into an anchor cloudscape
    /// </summary>
    public bool? Compressed { get; set; }

    public bool? IsAtomic { get; set; }

    public PerfonarTextAlignment? Alignment { get; set; }

    public string ResolveTitle() => Title.IsNotBlank()
        ? Title
        : new PerfonarKey(Selector, typeof(object)).Name.CapitalizeFirst();

    public PerfonarColumnFlavor ResolveFlavor()
    {
        if (Selector.StartsWith(PerfonarSymbol.Attribute))
            return PerfonarColumnFlavor.Attribute;
        if (Selector.StartsWith(PerfonarSymbol.Function))
            return PerfonarColumnFlavor.Function;
        return PerfonarColumnFlavor.Unknown;
    }

    public PerfonarTextAlignment ResolveAlignment()
    {
        if (Alignment.HasValue && Alignment != PerfonarTextAlignment.Auto)
            return Alignment.Value;
        return ResolveFlavor() switch
        {
            PerfonarColumnFlavor.Attribute => Selector.StartsWith(PerfonarSymbol.Attribute + "parameters")
                ? PerfonarTextAlignment.Right
                : PerfonarTextAlignment.Left,
            PerfonarColumnFlavor.Function => PerfonarTextAlignment.Right,
            PerfonarColumnFlavor.Unknown => PerfonarTextAlignment.Left,
            _ => PerfonarTextAlignment.Left
        };
    }
}
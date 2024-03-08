using System.Diagnostics;
using Perfolizer.Extensions;
using Perfolizer.Phd.Base;
using Perfolizer.Phd.Dto;

namespace Perfolizer.Phd.Tables;

[DebuggerDisplay("{Selector}")]
public class PhdColumnDefinition(string selector) : PhdObject
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

    public PhdTextAlignment? Alignment { get; set; }

    public string ResolveTitle() => Title.IsNotBlank()
        ? Title
        : new PhdKey(Selector, typeof(object)).Name.CapitalizeFirst();

    public PhdColumnFlavor ResolveFlavor()
    {
        if (Selector.StartsWith(PhdSymbol.Attribute))
            return PhdColumnFlavor.Attribute;
        if (Selector.StartsWith(PhdSymbol.Function))
            return PhdColumnFlavor.Function;
        return PhdColumnFlavor.Unknown;
    }

    public PhdTextAlignment ResolveAlignment()
    {
        if (Alignment.HasValue && Alignment != PhdTextAlignment.Auto)
            return Alignment.Value;
        return ResolveFlavor() switch
        {
            PhdColumnFlavor.Attribute => Selector.StartsWith(PhdSymbol.Attribute + "parameters")
                ? PhdTextAlignment.Right
                : PhdTextAlignment.Left,
            PhdColumnFlavor.Function => PhdTextAlignment.Right,
            PhdColumnFlavor.Unknown => PhdTextAlignment.Left,
            _ => PhdTextAlignment.Left
        };
    }
}
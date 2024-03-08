using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Tables;

public class PhdTableConfig : PhdObject
{
    public List<PhdFilter> Filters { get; set; } = [];
    public List<PhdColumnDefinition> ColumnDefinitions { get; set; } = [];
    public List<PhdSortPolicy> SortPolicies { get; set; } = [];

    /// <summary>
    /// See <see cref="PhdAnchorGenerator"/>
    /// </summary>
    public int? MaxAnchorLength { get; set; }

    public bool IsMatched(PhdEntry entry) => Filters.All(filter => filter.IsMatched(entry));

    // public bool PrintUnitsInHeader { get; set; }
    // public bool PrintUnitsInContent { get; set; }
    // public bool PrintZeroValuesInContent { get; set; }
    // public int MaxParameterColumnWidth { get; set; }
    // public SizeUnit? SizeUnit { get; set; }
    // public TimeUnit? TimeUnit { get; set; }
    // public PhdRatioStyle PhdRatioStyle { get; set; }
}
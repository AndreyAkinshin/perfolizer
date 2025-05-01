using Perfolizer.Models;

namespace Perfolizer.Perfonar.Tables;

public class PerfonarTableConfig : AbstractInfo
{
    public List<PerfonarFilter> Filters { get; set; } = [];
    public List<PerfonarColumnDefinition> ColumnDefinitions { get; set; } = [];
    public List<PerfonarSortPolicy> SortPolicies { get; set; } = [];

    /// <summary>
    /// See <see cref="PerfonarAnchorGenerator"/>
    /// </summary>
    public int? MaxAnchorLength { get; set; }

    public bool IsMatched(EntryInfo entry) => Filters.All(filter => filter.IsMatched(entry));

    // public bool PrintUnitsInHeader { get; set; }
    // public bool PrintUnitsInContent { get; set; }
    // public bool PrintZeroValuesInContent { get; set; }
    // public int MaxParameterColumnWidth { get; set; }
    // public SizeUnit? SizeUnit { get; set; }
    // public TimeUnit? TimeUnit { get; set; }
    // public PerfonarRatioStyle PerfonarRatioStyle { get; set; }
}
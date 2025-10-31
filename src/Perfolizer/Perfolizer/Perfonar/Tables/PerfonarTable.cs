using Perfolizer.Collections;
using Perfolizer.Extensions;
using Perfolizer.Models;
using Perfolizer.Perfonar.Base;
using Perfolizer.Perfonar.Functions;
using Pragmastat.Metrology;

namespace Perfolizer.Perfonar.Tables;

public class PerfonarTable
{
    public EntryInfo RootEntry { get; }
    public PerfonarTableConfig Config { get; }
    public List<PerfonarColumn> Columns { get; }
    public IReadOnlyList<PerfonarRow> Rows { get; }
    public IReadOnlyList<PerfonarCloudscape> Cloudscapes { get; }

    public int RowCount => Rows.Count;
    public int ColumnCount => Columns.Count;
    public object? this[int row, int col] => Rows[row][Columns[col]].Value;

    public PerfonarTable(EntryInfo rootEntry, PerfonarTableConfig config)
    {
        RootEntry = rootEntry;
        Config = config;
        var index = new PerfonarIndex(rootEntry);
        Columns = BuildColumns(Config, index);
        var rows = BuildRows(rootEntry, Config, Columns, index);
        FillFunctionCells(rows, Columns);
        SortRows(Config, rows);
        Rows = rows;
        Cloudscapes = ExtractCloudscapes(Config, index, Rows, Columns);
    }

    private static List<PerfonarColumn> BuildColumns(PerfonarTableConfig config, PerfonarIndex index)
    {
        var columns = new List<PerfonarColumn>();
        var functionResolver = new PerfonarFunctionResolver().RegisterDefaults();
        foreach (var definition in config.ColumnDefinitions)
        {
            // PerfonarFunctionColumn
            if (definition.Selector.StartsWith(PerfonarSymbol.Function))
            {
                var function = functionResolver.Resolve(definition.Selector);
                if (function == null)
                    continue; // TODO

                var column = function switch
                {
                    PerfonarFunction<Measurement> measurementFunction =>
                        new PerfonarFunctionColumn<Measurement>(definition, measurementFunction),
                    _ => new PerfonarFunctionColumn(definition, function)
                };
                columns.Add(column);
            }

            // PerfonarAttributeColumn
            var columnKeys = index.Keys.Where(key => key.IsMatched(definition.Selector)).ToReadOnlyList();
            if (columnKeys.IsEmpty()) continue;

            var displayKey = columnKeys.FirstOrDefault(key => key.Path.EquationsIgnoreCase(definition.Selector));
            if (displayKey != null && definition.IsAtomic == true)
            {
                string title = definition.ResolveTitle();
                columns.Add(new PerfonarAttributeColumn(title, definition.Selector, definition, displayKey));
            }
            else
            {
                // TODO: Support display columns / composite keys

                var primitiveColumnKeys = columnKeys.Where(key => !key.IsComposite()).ToList();
                foreach (var columnKey in primitiveColumnKeys)
                {
                    if (columnKey.Name.EquationsIgnoreCase(nameof(AbstractInfo.Display)))
                        continue;
                    string title = columnKey.Name.CapitalizeFirst();
                    columns.Add(new PerfonarAttributeColumn(title, columnKey.Path, definition, columnKey));
                }
            }
        }
        return columns;
    }

    private static List<PerfonarRow> BuildRows(EntryInfo rootEntry, PerfonarTableConfig config, IReadOnlyList<PerfonarColumn> columns, PerfonarIndex index)
    {
        var rowMap = new Dictionary<string, PerfonarRow>();
        foreach (var entry in rootEntry.Traverse().Where(config.IsMatched))
        {
            var row = new PerfonarRow(entry);
            foreach (var column in columns.OfType<PerfonarAttributeColumn>())
            {
                object? value = index[entry][column.Key]?.Value;
                row[column] = new PerfonarCell(column, value);
            }
            string definitionId = row.BuildAttributeId();
            if (!rowMap.TryGetValue(definitionId, out var existingRow))
                rowMap[definitionId] = row;
            else
                row = existingRow;
            if (entry.Value != null)
                row.Measurements.Add(new Measurement(entry.Value.Value, entry.Unit));
        }
        return rowMap.Values.Where(row => row.Measurements.Any()).ToList();
    }

    private static void FillFunctionCells(IReadOnlyList<PerfonarRow> rows, IReadOnlyList<PerfonarColumn> columns)
    {
        foreach (var row in rows)
        foreach (var column in columns.OfType<PerfonarFunctionColumn>())
        {
            object? value = column.Function.Apply(row.ToSample());
            row[column] = new PerfonarCell(column, value);
        }
    }

    private static void SortRows(PerfonarTableConfig config, List<PerfonarRow> rows)
    {
        if (rows.IsEmpty() || config.SortPolicies.IsEmpty())
            return;

        rows.Sort(Compare);
        return;

        int Compare(PerfonarRow a, PerfonarRow b)
        {
            foreach (var sortPolicy in config.SortPolicies)
            {
                object? aCell = a[sortPolicy.Selector].Value;
                object? bCell = b[sortPolicy.Selector].Value;
                if (aCell == null && bCell == null)
                    return 0;
                if (aCell is IComparable aComparable && bCell is IComparable bComparable)
                    return aComparable.CompareTo(bComparable) * sortPolicy.Direction.ToSign();
            }
            return 0;
        }
    }

    private static IReadOnlyList<PerfonarCloudscape> ExtractCloudscapes(
        PerfonarTableConfig config,
        PerfonarIndex index,
        IReadOnlyList<PerfonarRow> rows,
        List<PerfonarColumn> columns)
    {
        List<PerfonarCloudscape> cloudscapes = [];
        cloudscapes.AddRange(ExtractSharedCloudscapes(rows, columns));
        cloudscapes.AddRange(ExtractAnchorCloudscapes(config, index, rows, columns));
        return cloudscapes.ToArray();
    }

    private static List<PerfonarCloudscape> ExtractSharedCloudscapes(IReadOnlyList<PerfonarRow> rows, List<PerfonarColumn> columns)
    {
        var sharedCells = new List<PerfonarCell>();
        var sharedColumns = new HashSet<PerfonarColumn>();
        foreach (var column in columns)
        {
            if (column.Definition.CanBeShared == false) continue;
            if (column.Definition.CanBeShared == null && column is PerfonarFunctionColumn) continue;

            var cells = rows.Select(row => row[column]).ToReadOnlyList();
            if (cells.Select(cell => cell.Value).Distinct().Count() == 1)
            {
                sharedColumns.Add(column);
                sharedCells.Add(cells.First());
            }
        }
        columns.RemoveAll(column => sharedColumns.Contains(column));

        return sharedCells
            .GroupBy(cell => cell.Column.Definition.Cloud)
            .Select(group => new PerfonarCloudscape().Add(new PerfonarCloud("", group.ToReadOnlyList()))).ToList();
    }

    private static List<PerfonarCloudscape> ExtractAnchorCloudscapes(
        PerfonarTableConfig config,
        PerfonarIndex index,
        IReadOnlyList<PerfonarRow> rows,
        List<PerfonarColumn> columns)
    {
        List<PerfonarCloudscape> cloudscapes = [];
        foreach (var definition in config.ColumnDefinitions.Where(definition => definition.Compressed ?? false))
        {
            var matchedColumns = columns.Where(column => column.Definition == definition).ToList();
            if (matchedColumns.Count > 1)
            {
                var cloudscape = new PerfonarCloudscape();
                var newColumn = new PerfonarAttributeColumn(
                    definition.ResolveTitle(),
                    definition.Selector,
                    definition,
                    new PerfonarKey(definition.Selector, typeof(object)));

                var selectorKey = index.Keys.FirstOrDefault(key => key.IsMatched($"{definition.Selector}.Id"));
                var anchorGenerator = new PerfonarAnchorGenerator(config.MaxAnchorLength);
                var anchors = new HashSet<string>();
                foreach (var row in rows)
                {
                    var subRow = new PerfonarRow(row.Entry);
                    foreach (var matchedColumn in matchedColumns)
                        subRow[matchedColumn] = row[matchedColumn];
                    string attributeId = subRow.BuildAttributeId();
                    string anchor = selectorKey != null
                        ? index[row.Entry][selectorKey]?.Value.ToString() ?? ""
                        : anchorGenerator.GetAnchor(attributeId);

                    if (anchors.Add(anchor))
                        cloudscape.Add(new PerfonarCloud(anchor, subRow.Cells.ToReadOnlyList()));
                    row[newColumn] = new PerfonarCell(newColumn, anchor);
                }

                int minIndex = matchedColumns.Min(columns.IndexOf);
                foreach (var matchedColumn in matchedColumns)
                    columns.Remove(matchedColumn);
                columns.Insert(minIndex, newColumn);
                cloudscapes.Add(cloudscape);
            }
        }
        return cloudscapes;
    }
}
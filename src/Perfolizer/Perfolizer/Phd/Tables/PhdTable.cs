using Perfolizer.Collections;
using Perfolizer.Extensions;
using Perfolizer.Metrology;
using Perfolizer.Phd.Base;
using Perfolizer.Phd.Functions;

namespace Perfolizer.Phd.Tables;

public class PhdTable
{
    public PhdEntry RootEntry { get; }
    public PhdTableConfig Config { get; }
    public List<PhdColumn> Columns { get; }
    public IReadOnlyList<PhdRow> Rows { get; }
    public IReadOnlyList<PhdCloudscape> Cloudscapes { get; }

    public int RowCount => Rows.Count;
    public int ColumnCount => Columns.Count;
    public object? this[int row, int col] => Rows[row][Columns[col]].Value;

    public PhdTable(PhdEntry rootEntry)
    {
        RootEntry = rootEntry;
        Config = rootEntry.ResolveMeta()?.Table ?? new PhdTableConfig();
        var index = new PhdIndex(rootEntry);
        Columns = BuildColumns(Config, index);
        var rows = BuildRows(rootEntry, Config, Columns, index);
        FillFunctionCells(rows, Columns);
        SortRows(Config, rows);
        Rows = rows;
        Cloudscapes = ExtractCloudscapes(Config, index, Rows, Columns);
    }

    private static List<PhdColumn> BuildColumns(PhdTableConfig config, PhdIndex index)
    {
        var columns = new List<PhdColumn>();
        var functionResolver = new PhdFunctionResolver().RegisterDefaults();
        foreach (var definition in config.ColumnDefinitions)
        {
            // PhdFunctionColumn
            if (definition.Selector.StartsWith(PhdSymbol.Function))
            {
                var function = functionResolver.Resolve(definition.Selector);
                if (function == null)
                    continue; // TODO

                var column = function switch
                {
                    PhdFunction<Measurement> measurementFunction =>
                        new PhdFunctionColumn<Measurement>(definition, measurementFunction),
                    _ => new PhdFunctionColumn(definition, function)
                };
                columns.Add(column);
            }

            // PhdAttributeColumn
            var columnKeys = index.Keys.Where(key => key.IsMatched(definition.Selector)).ToReadOnlyList();
            if (columnKeys.IsEmpty()) continue;

            var displayKey = columnKeys.FirstOrDefault(key => key.Path.EquationsIgnoreCase(definition.Selector));
            if (displayKey != null && definition.IsAtomic == true)
            {
                string title = definition.ResolveTitle();
                columns.Add(new PhdAttributeColumn(title, definition.Selector, definition, displayKey));
            }
            else
            {
                // TODO: Support display columns / composite keys

                var primitiveColumnKeys = columnKeys.Where(key => !key.IsComposite()).ToList();
                foreach (var columnKey in primitiveColumnKeys)
                {
                    if (columnKey.Name.EquationsIgnoreCase(nameof(PhdObject.Display)))
                        continue;
                    string title = columnKey.Name.CapitalizeFirst();
                    columns.Add(new PhdAttributeColumn(title, columnKey.Path, definition, columnKey));
                }
            }
        }
        return columns;
    }

    private static List<PhdRow> BuildRows(PhdEntry rootEntry, PhdTableConfig config, IReadOnlyList<PhdColumn> columns, PhdIndex index)
    {
        var rowMap = new Dictionary<string, PhdRow>();
        foreach (var entry in rootEntry.Traverse().Where(config.IsMatched))
        {
            var row = new PhdRow(entry);
            foreach (var column in columns.OfType<PhdAttributeColumn>())
            {
                object? value = index[entry][column.Key]?.Value;
                row[column] = new PhdCell(column, value);
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

    private static void FillFunctionCells(IReadOnlyList<PhdRow> rows, IReadOnlyList<PhdColumn> columns)
    {
        foreach (var row in rows)
        foreach (var column in columns.OfType<PhdFunctionColumn>())
        {
            object? value = column.Function.Apply(row.ToSample());
            row[column] = new PhdCell(column, value);
        }
    }

    private static void SortRows(PhdTableConfig config, List<PhdRow> rows)
    {
        if (rows.IsEmpty() || config.SortPolicies.IsEmpty())
            return;

        rows.Sort(Compare);
        return;

        int Compare(PhdRow a, PhdRow b)
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

    private static IReadOnlyList<PhdCloudscape> ExtractCloudscapes(
        PhdTableConfig config,
        PhdIndex index,
        IReadOnlyList<PhdRow> rows,
        List<PhdColumn> columns)
    {
        List<PhdCloudscape> cloudscapes = [];
        cloudscapes.AddRange(ExtractSharedCloudscapes(rows, columns));
        cloudscapes.AddRange(ExtractAnchorCloudscapes(config, index, rows, columns));
        return cloudscapes.ToArray();
    }

    private static List<PhdCloudscape> ExtractSharedCloudscapes(IReadOnlyList<PhdRow> rows, List<PhdColumn> columns)
    {
        var sharedCells = new List<PhdCell>();
        var sharedColumns = new HashSet<PhdColumn>();
        foreach (var column in columns)
        {
            if (column.Definition.CanBeShared == false) continue;
            if (column.Definition.CanBeShared == null && column is PhdFunctionColumn) continue;

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
            .Select(group => new PhdCloudscape().Add(new PhdCloud("", group.ToReadOnlyList()))).ToList();
    }

    private static List<PhdCloudscape> ExtractAnchorCloudscapes(
        PhdTableConfig config,
        PhdIndex index,
        IReadOnlyList<PhdRow> rows,
        List<PhdColumn> columns)
    {
        List<PhdCloudscape> cloudscapes = [];
        foreach (var definition in config.ColumnDefinitions.Where(definition => definition.Compressed ?? false))
        {
            var matchedColumns = columns.Where(column => column.Definition == definition).ToList();
            if (matchedColumns.Count > 1)
            {
                var cloudscape = new PhdCloudscape();
                var newColumn = new PhdAttributeColumn(
                    definition.ResolveTitle(),
                    definition.Selector,
                    definition,
                    new PhdKey(definition.Selector, typeof(object)));

                var selectorKey = index.Keys.FirstOrDefault(key => key.IsMatched($"{definition.Selector}.Id"));
                var anchorGenerator = new PhdAnchorGenerator(config.MaxAnchorLength);
                var anchors = new HashSet<string>();
                foreach (var row in rows)
                {
                    var subRow = new PhdRow(row.Entry);
                    foreach (var matchedColumn in matchedColumns)
                        subRow[matchedColumn] = row[matchedColumn];
                    string attributeId = subRow.BuildAttributeId();
                    string anchor = selectorKey != null
                        ? index[row.Entry][selectorKey]?.Value.ToString() ?? ""
                        : anchorGenerator.GetAnchor(attributeId);

                    if (anchors.Add(anchor))
                        cloudscape.Add(new PhdCloud(anchor, subRow.Cells.ToReadOnlyList()));
                    row[newColumn] = new PhdCell(newColumn, anchor);
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
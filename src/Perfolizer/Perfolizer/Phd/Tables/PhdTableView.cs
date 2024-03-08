using Perfolizer.Collections;
using Perfolizer.Extensions;
using Perfolizer.Horology;
using Perfolizer.Mathematics.Common;
using Perfolizer.Metrology;
using Perfolizer.Phd.Presenting;

namespace Perfolizer.Phd.Tables;

public class PhdTableView
{
    public PhdTable Table { get; }
    public PhdTableStyle Style { get; }
    public object?[,] CellObjects { get; }
    public string?[] ColumnFormats { get; }
    public string[,] Cells { get; }
    public int[] ColumnWidths { get; }

    public IReadOnlyList<PhdColumn> Columns => Table.Columns;
    public int RowCount => Table.RowCount;
    public int ColumnCount => Table.ColumnCount;
    public IReadOnlyList<PhdCloudscape> Cloudscapes => Table.Cloudscapes;

    public PhdTableView(PhdTable table, PhdTableStyle style)
    {
        Table = table;
        Style = style;
        CellObjects = BuildCellObjects(table);
        ColumnFormats = BuildColumnFormats(table);
        Cells = BuildCells(table, style, CellObjects, ColumnFormats);
        ColumnWidths = BuildColumnWidths(table, Cells);
    }

    private static object?[,] BuildCellObjects(PhdTable table)
    {
        object?[,] cellObjects = new object?[table.RowCount, table.ColumnCount];
        for (int row = 0; row < table.RowCount; row++)
        for (int col = 0; col < table.ColumnCount; col++)
            cellObjects[row, col] = table[row, col];
        return cellObjects;
    }

    private static string?[] BuildColumnFormats(PhdTable table)
    {
        string?[] columnFormats = new string?[table.ColumnCount];
        for (int col = 0; col < table.ColumnCount; col++)
        {
            var column = table.Columns[col];
            if (column is PhdFunctionColumn<Measurement> functionColumn)
            {
                var measurements = table.Rows
                    .Select(row => row[functionColumn].Value as Measurement)
                    .WhereNotNull()
                    .ToArray();
                var unit = measurements.First().Unit;

                if (unit is TimeUnit timeUnit)
                {
                    double[] timeIntervals = measurements
                        .Select(m => m.AsTimeInterval()!.Value.Nanoseconds)
                        .WhereNotNull()
                        .ToArray();
                    var bestUnit = TimeUnit.GetBestTimeUnit(timeIntervals);
                    for (int i = 0; i < measurements.Length; i++)
                    {
                        double nominalValue = TimeUnit.Convert(measurements[i].NominalValue, timeUnit, bestUnit);
                        measurements[i] = nominalValue.WithUnit(bestUnit);
                        timeIntervals[i] = nominalValue;
                    }
                    int precision = PrecisionHelper.GetOptimalPrecision(timeIntervals);
                    columnFormats[col] = $"F{precision}";
                }
            }
        }
        return columnFormats;
    }

    private static string[,] BuildCells(PhdTable table, PhdTableStyle style, object?[,] cellObjects, string?[] columnFormats)
    {
        var formatProvider = style.FormatProvider;
        string[,] cells = new string[table.RowCount, table.ColumnCount];
        for (int row = 0; row < table.RowCount; row++)
        for (int col = 0; col < table.ColumnCount; col++)
        {
            object? cellObject = cellObjects[row, col];
            cells[row, col] = cellObject switch
            {
                IWithUnits withUnit => withUnit.ToString(columnFormats[col], formatProvider, style.UnitPresentation),
                _ => cellObject?.ToString() ?? ""
            };
        }
        return cells;
    }

    private static int[] BuildColumnWidths(PhdTable table, string[,] cells)
    {
        int[] columnWidths = new int[table.ColumnCount];
        for (int col = 0; col < table.ColumnCount; col++)
        {
            columnWidths[col] = table.Columns[col].Title.Length;
            for (int row = 0; row < table.RowCount; row++)
                columnWidths[col] = Max(columnWidths[col], cells[row, col].Length);
        }
        return columnWidths;
    }
}
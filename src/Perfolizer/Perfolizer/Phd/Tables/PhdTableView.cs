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
        Cells = BuildCells(table, style);
        ColumnWidths = BuildColumnWidths(table, Cells);
    }

    private static string[,] BuildCells(PhdTable table, PhdTableStyle style)
    {
        string[,] cells = new string[table.RowCount, table.ColumnCount];
        var formatProvider = style.FormatProvider;

        for (int colIndex = 0; colIndex < table.ColumnCount; colIndex++)
        {
            var column = table.Columns[colIndex];
            if (column is PhdFunctionColumn<Measurement> functionColumn)
            {
                var measurements = table.Rows
                    .Select(row => row[functionColumn].Value as Measurement)
                    .WhereNotNull()
                    .ToArray();
                if (measurements.IsEmpty())
                    continue;
                var unit = measurements.First().Unit;

                if (unit is TimeUnit)
                {
                    double[] values = measurements
                        .Select(m => m.AsTimeInterval()!.Value.Nanoseconds)
                        .WhereNotNull()
                        .ToArray();
                    var bestUnit = TimeUnit.GetBestTimeUnit(values);
                    for (int rowIndex = 0; rowIndex < measurements.Length; rowIndex++)
                    {
                        var timeInterval = measurements[rowIndex].AsTimeInterval()!.Value;
                        double valueInBestUnit = timeInterval.ToUnit(bestUnit);
                        measurements[rowIndex] = valueInBestUnit.WithUnit(bestUnit);
                        values[rowIndex] = valueInBestUnit;
                    }
                    int precision = PrecisionHelper.GetOptimalPrecision(values);
                    string format = $"F{precision}";

                    for (int rowIndex = 0; rowIndex < measurements.Length; rowIndex++)
                        cells[rowIndex, colIndex] = measurements[rowIndex].ToString(format, formatProvider, style.UnitPresentation);
                    continue;
                }
            }

            for (int rowIndex = 0; rowIndex < table.RowCount; rowIndex++)
            {
                object? cellObject = table[rowIndex, colIndex];
                cells[rowIndex, colIndex] = cellObject switch
                {
                    IWithUnits withUnit => withUnit.ToString(null, formatProvider, style.UnitPresentation),
                    _ => cellObject?.ToString() ?? ""
                };
            }
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
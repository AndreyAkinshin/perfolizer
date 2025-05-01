using System.Text;
using JetBrains.Annotations;
using Perfolizer.Extensions;
using Perfolizer.Perfonar.Base;

namespace Perfolizer.Perfonar.Tables;

public class PerfonarMarkdownTableWriter(TextWriter writer, PerfonarTableStyle? style = null)
{
    private readonly PerfonarTableStyle style = style ?? new PerfonarTableStyle();

    [PublicAPI]
    public PerfonarMarkdownTableWriter(Stream stream, PerfonarTableStyle? style = null, Encoding? encoding = null)
        : this(new StreamWriter(stream, encoding ?? Encoding.UTF8, bufferSize: 4096, leaveOpen: true), style)
    {
    }

    [PublicAPI]
    public static string FormatToString(
        PerfonarTable table, PerfonarTableStyle? style = null)
    {
        var stringWriter = new StringWriter();
        var formatter = new PerfonarMarkdownTableWriter(stringWriter, style);
        formatter.WriteAsync(table).GetAwaiter().GetResult();
        return stringWriter.ToString();
    }

    [PublicAPI]
    public static async Task SaveToFile(
        string filePath, PerfonarTable table, PerfonarTableStyle? style = null, Encoding? encoding = null)
    {
        using var stream = File.Create(filePath);
        var formatter = new PerfonarMarkdownTableWriter(stream, style, encoding);
        await formatter.WriteAsync(table);
    }

    [PublicAPI]
    public async Task WriteAsync(PerfonarTable table)
    {
        var view = new PerfonarTableView(table, style);

        foreach (var cloudscape in view.Cloudscapes)
        {
            await WriteCloudscapeAsync(cloudscape);
        }

        await WriteTableAsync(view);

        await writer.FlushAsync();
    }

    [PublicAPI]
    public void Write(PerfonarTable table)
    {
        WriteAsync(table).GetAwaiter().GetResult();
    }

    private async Task WriteCloudscapeAsync(PerfonarCloudscape cloudscape)
    {
        int maxIdLength = cloudscape.Clouds.Max(cloud => cloud.Id.Length);

        foreach (var cloud in cloudscape.Clouds)
        {
            await WriteCloudAsync(cloud, maxIdLength);
        }

        await writer.WriteLineAsync();
    }

    private async Task WriteCloudAsync(PerfonarCloud cloud, int maxIdLength)
    {
        string idText = cloud.Id.IsNotBlank()
            ? $"{PerfonarSymbol.Anchor}{cloud.Id}: ".PadRight(maxIdLength + 3)
            : "";

        await writer.WriteAsync(idText);
        int currentLineLength = idText.Length;

        for (int i = 0; i < cloud.Cells.Count; i++)
        {
            var cell = cloud.Cells[i];
            string value = cell.Value?.ToString() ?? "";
            string content = cell.Column.Definition.IsSelfExplanatory ?? false
                ? value
                : $"{cell.Column.Title} = {value}";

            string separator = i == 0 ? "" : ", ";
            bool onSameLine = currentLineLength == 0 ||
                              currentLineLength + separator.Length + content.Length <= style.PreferredWidth;

            if (onSameLine)
            {
                await writer.WriteAsync(separator);
                await writer.WriteAsync(content);
                currentLineLength += separator.Length + content.Length;
            }
            else
            {
                if (currentLineLength > 0)
                    await writer.WriteLineAsync();
                await writer.WriteAsync(content);
                currentLineLength = content.Length;
            }
        }

        if (currentLineLength > 0)
            await writer.WriteLineAsync();
    }

    private async Task WriteTableAsync(PerfonarTableView view)
    {
        await WriteTableRowAsync(
            view,
            col => view.Table.Columns[col].Title,
            true);

        await WriteTableRowAsync(
            view,
            col => (view.Columns[col].Definition.ResolveAlignment() == PerfonarTextAlignment.Left ? ':' : '-') +
                   new string('-', view.ColumnWidths[col]) +
                   (view.Columns[col].Definition.ResolveAlignment() == PerfonarTextAlignment.Right ? ':' : '-'),
            false);

        for (int row = 0; row < view.RowCount; row++)
        {
            int currentRow = row; // Capture for lambda
            await WriteTableRowAsync(
                view,
                col => view.Cells[currentRow, col],
                true);
        }
    }

    private async Task WriteTableRowAsync(PerfonarTableView view, Func<int, string> getCellContent, bool addGaps)
    {
        for (int col = 0; col < view.ColumnCount; col++)
        {
            // Start with pipe for first column
            if (col == 0)
                await writer.WriteAsync('|');

            // Add space for formatting
            if (addGaps)
                await writer.WriteAsync(' ');

            // Format the cell content based on alignment
            var alignment = view.Columns[col].Definition.ResolveAlignment();
            string content = getCellContent(col);
            string presentation = alignment switch
            {
                PerfonarTextAlignment.Right => content.PadLeft(view.ColumnWidths[col]),
                _ => content.PadRight(view.ColumnWidths[col])
            };

            await writer.WriteAsync(presentation);

            // Add trailing space and pipe
            if (addGaps)
                await writer.WriteAsync(' ');

            await writer.WriteAsync('|');
        }

        await writer.WriteLineAsync();
    }
}
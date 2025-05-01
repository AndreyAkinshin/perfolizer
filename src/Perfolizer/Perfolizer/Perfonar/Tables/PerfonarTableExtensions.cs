using System.Text;

namespace Perfolizer.Perfonar.Tables;

public static class PerfonarTableExtensions
{
    public static string ToMarkdown(this PerfonarTable table, PerfonarTableStyle? style = null)
    {
        return PerfonarMarkdownTableWriter.FormatToString(table, style);
    }
    
    public static async Task SaveToMarkdownFileAsync(
        this PerfonarTable table, string filePath, PerfonarTableStyle? style = null, Encoding? encoding = null)
    {
        await PerfonarMarkdownTableWriter.SaveToFile(filePath, table, style, encoding);
    }
}
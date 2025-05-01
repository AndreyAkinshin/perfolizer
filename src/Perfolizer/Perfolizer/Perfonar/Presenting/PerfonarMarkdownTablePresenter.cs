using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Metrology;
using Perfolizer.Perfonar.Base;
using Perfolizer.Perfonar.Tables;
using Perfolizer.Presenting;

namespace Perfolizer.Perfonar.Presenting;

public class PerfonarTableStyle
{
    public IFormatProvider FormatProvider { get; set; } = DefaultCultureInfo.Instance;
    public UnitPresentation UnitPresentation { get; set; } = UnitPresentation.WithGap;
    public int PreferredWidth { get; set; } = 100;
}

public class PerfonarMarkdownTablePresenter(IPresenter presenter) : IPerfonarTablePresenter
{
    public void Present(PerfonarTable table, PerfonarTableStyle style)
    {
        var view = new PerfonarTableView(table, style);
        PresentClouds(view.Cloudscapes, style);
        PresentTable(view);
        presenter.Flush();
    }

    private void PresentClouds(IReadOnlyList<PerfonarCloudscape> cloudscapes, PerfonarTableStyle style)
    {
        foreach (var cloudscape in cloudscapes)
        {
            int maxIdLength = cloudscape.Clouds.Max(cloud => cloud.Id.Length);
            foreach (var cloud in cloudscape.Clouds)
                Present(cloud, style, maxIdLength);
            presenter.WriteLine();
        }
    }

    private void Present(PerfonarCloud cloud, PerfonarTableStyle style, int maxIdLength)
    {
        string idText = cloud.Id.IsNotBlank() ? $"{PerfonarSymbol.Anchor}{cloud.Id}: ".PadRight(maxIdLength + 3) : "";
        presenter.Write(idText);
        int currentLineLength = idText.Length;
        for (int i = 0; i < cloud.Cells.Count; i++)
        {
            var cell = cloud.Cells[i];
            string value = cell.Value?.ToString() ?? ""; // TODO: Present
            string content = cell.Column.Definition.IsSelfExplanatory ?? false
                ? value
                : $"{cell.Column.Title} = {value}";

            string separator = i == 0 ? "" : ", ";
            bool onSameLine = currentLineLength == 0 ||
                              currentLineLength + separator.Length + content.Length <= style.PreferredWidth;
            if (onSameLine)
            {
                presenter.Write(separator);
                presenter.Write(content);
                currentLineLength += separator.Length + content.Length;
            }
            else
            {
                if (currentLineLength > 0)
                    presenter.WriteLine();
                presenter.Write(content);
                currentLineLength = content.Length;
            }
        }

        if (currentLineLength > 0)
            presenter.WriteLine();
    }

    private void PresentTable(PerfonarTableView view)
    {
        PresentRow(col => view.Table.Columns[col].Title);
        PresentRow(col =>
                (view.Columns[col].Definition.ResolveAlignment() == PerfonarTextAlignment.Left ? ':' : '-') +
                new string('-', view.ColumnWidths[col]) +
                (view.Columns[col].Definition.ResolveAlignment() == PerfonarTextAlignment.Right ? ':' : '-')
            , false);
        for (int row = 0; row < view.RowCount; row++)
        {
            int currentRow = row;
            PresentRow(col => view.Cells[currentRow, col]);
        }
        return;

        void PresentRow(Func<int, string> presentCell, bool addGaps = true)
        {
            for (int col = 0; col < view.ColumnCount; col++)
            {
                if (col == 0) presenter.Write('|');
                if (addGaps) presenter.Write(' ');
                var alignment = view.Columns[col].Definition.ResolveAlignment();
                string presentation = alignment switch
                {
                    PerfonarTextAlignment.Right => presentCell(col).PadLeft(view.ColumnWidths[col]),
                    _ => presentCell(col).PadRight(view.ColumnWidths[col])
                };
                presenter.Write(presentation);
                if (addGaps) presenter.Write(' ');
                presenter.Write('|');
            }
            presenter.WriteLine();
        }
    }
}
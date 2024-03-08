namespace Perfolizer.Phd.Tables;

public enum PhdSortDirection
{
    Descending = -1,
    Ascending = 1
}

public static class PhdSortDirectionExtensions
{
    public static int ToSign(this PhdSortDirection direction) => (int)direction;
}
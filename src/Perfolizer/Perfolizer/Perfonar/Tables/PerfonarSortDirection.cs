namespace Perfolizer.Perfonar.Tables;

public enum PerfonarSortDirection
{
    Descending = -1,
    Ascending = 1
}

public static class PerfonarSortDirectionExtensions
{
    public static int ToSign(this PerfonarSortDirection direction) => (int)direction;
}
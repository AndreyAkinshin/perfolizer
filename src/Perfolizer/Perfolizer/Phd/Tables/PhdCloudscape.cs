namespace Perfolizer.Phd.Tables;

/// <summary>
/// A collection of shared cell values displayed above a table
/// </summary>
public class PhdCloudscape
{
    public List<PhdCloud> Clouds { get; } = [];

    public PhdCloudscape Add(PhdCloud cloud)
    {
        Clouds.Add(cloud);
        return this;
    }

    public PhdCloudscape AddRange(IEnumerable<PhdCloud> clouds)
    {
        Clouds.AddRange(clouds);
        return this;
    }
}
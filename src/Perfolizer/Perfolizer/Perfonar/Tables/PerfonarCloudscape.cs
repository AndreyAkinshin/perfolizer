namespace Perfolizer.Perfonar.Tables;

/// <summary>
/// A collection of shared cell values displayed above a table
/// </summary>
public class PerfonarCloudscape
{
    public List<PerfonarCloud> Clouds { get; } = [];

    public PerfonarCloudscape Add(PerfonarCloud cloud)
    {
        Clouds.Add(cloud);
        return this;
    }

    public PerfonarCloudscape AddRange(IEnumerable<PerfonarCloud> clouds)
    {
        Clouds.AddRange(clouds);
        return this;
    }
}
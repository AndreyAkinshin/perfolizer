using JetBrains.Annotations;

namespace Perfolizer.Models;

[PublicAPI]
public class IdentityInfo : AbstractInfo
{
    public string Title { get; set; } = "";
    public Guid RunId { get; set; }
    public long Timestamp { get; set; }
}
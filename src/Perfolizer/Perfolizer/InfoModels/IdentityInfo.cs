using JetBrains.Annotations;
using Perfolizer.Phd.Base;

namespace Perfolizer.InfoModels;

[PublicAPI]
public class IdentityInfo : AbstractInfo
{
    public string Title { get; set; } = "";
    public Guid RunId { get; set; }
    public long Timestamp { get; set; }
}
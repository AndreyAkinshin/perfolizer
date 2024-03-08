using JetBrains.Annotations;
using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Dto;

[PublicAPI]
public class PhdInfo : PhdObject
{
    public string Title { get; set; } = "";
    public Guid RunId { get; set; }
    public long Timestamp { get; set; }
}
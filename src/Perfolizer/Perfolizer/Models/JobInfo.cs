using JetBrains.Annotations;

namespace Perfolizer.Models;

[PublicAPI]
public class JobInfo : AbstractInfo
{
    public EnvironmentInfo? Environment { get; set; }
    public ExecutionInfo? Execution { get; set; }
}
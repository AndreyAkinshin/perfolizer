using JetBrains.Annotations;
using Perfolizer.Phd.Base;

namespace Perfolizer.InfoModels;

[PublicAPI]
public class JobInfo : AbstractInfo
{
    public EnvironmentInfo? Environment { get; set; }
    public ExecutionInfo? Execution { get; set; }
}
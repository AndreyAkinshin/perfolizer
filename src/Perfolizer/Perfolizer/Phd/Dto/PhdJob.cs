using JetBrains.Annotations;
using Perfolizer.Phd.Base;

namespace Perfolizer.Phd.Dto;

[PublicAPI]
public class PhdJob : PhdObject
{
    public PhdEnvironment? Environment { get; set; }
    public PhdExecution? Execution { get; set; }
}
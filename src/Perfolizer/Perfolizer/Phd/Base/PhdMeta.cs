using JetBrains.Annotations;
using Perfolizer.InfoModels;
using Perfolizer.Phd.Tables;

namespace Perfolizer.Phd.Base;

[PublicAPI]
public class PhdMeta: AbstractInfo
{
    public PhdTableConfig Table { get; set; } = new ();
}
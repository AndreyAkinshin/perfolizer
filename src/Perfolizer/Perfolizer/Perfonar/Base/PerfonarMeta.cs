using JetBrains.Annotations;
using Perfolizer.InfoModels;
using Perfolizer.Perfonar.Tables;

namespace Perfolizer.Perfonar.Base;

[PublicAPI]
public class PerfonarMeta: AbstractInfo
{
    public PerfonarTableConfig Table { get; set; } = new ();
}
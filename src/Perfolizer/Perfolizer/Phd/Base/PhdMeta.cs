using JetBrains.Annotations;
using Perfolizer.Phd.Tables;

namespace Perfolizer.Phd.Base;

[PublicAPI]
public class PhdMeta
{
    public PhdTableConfig Table { get; set; } = new ();
}
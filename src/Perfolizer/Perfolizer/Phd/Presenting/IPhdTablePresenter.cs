using Perfolizer.Phd.Tables;

namespace Perfolizer.Phd.Presenting;

public interface IPhdTablePresenter
{
    void Present(PhdTable table, PhdTableStyle style);
}
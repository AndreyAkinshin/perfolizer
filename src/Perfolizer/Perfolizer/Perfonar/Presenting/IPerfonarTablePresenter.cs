using Perfolizer.Perfonar.Tables;

namespace Perfolizer.Perfonar.Presenting;

public interface IPerfonarTablePresenter
{
    void Present(PerfonarTable table, PerfonarTableStyle style);
}
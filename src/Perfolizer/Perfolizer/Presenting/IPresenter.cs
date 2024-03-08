namespace Perfolizer.Presenting;

public interface IPresenter
{
    void Write(char c);
    void Write(string message);
    void WriteLine();
    void Flush();
}
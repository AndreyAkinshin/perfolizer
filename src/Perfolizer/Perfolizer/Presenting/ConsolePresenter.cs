namespace Perfolizer.Presenting;

public class ConsolePresenter : IPresenter
{
    public void Write(char c) => Console.Write(c);
    public void Write(string message) => Console.Write(message);
    public void WriteLine() => Console.WriteLine();
    public void Flush() { }
}
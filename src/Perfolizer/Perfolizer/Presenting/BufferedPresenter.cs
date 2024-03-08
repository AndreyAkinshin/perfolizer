using System.Text;

namespace Perfolizer.Presenting;

public abstract class BufferedPresenter : IPresenter
{
    protected readonly StringBuilder Builder = new ();

    public void Write(char c) => Builder.Append(c);
    public void Write(string message) => Builder.Append(message);
    public void WriteLine() => Builder.AppendLine();

    public virtual void Flush()
    {
        Flush(Builder.ToString());
        Builder.Clear();
    }

    protected abstract void Flush(string text);
}
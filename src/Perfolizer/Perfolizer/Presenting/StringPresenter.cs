namespace Perfolizer.Presenting;

public class StringPresenter : BufferedPresenter
{
    public override void Flush() { }
    protected override void Flush(string text) { }
    public string Dump() => Builder.ToString();
}
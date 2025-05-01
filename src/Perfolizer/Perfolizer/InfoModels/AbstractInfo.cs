namespace Perfolizer.InfoModels;

public abstract class AbstractInfo
{
    public string? Display { get; set; }

    public virtual string? GetDisplay() => Display;
}
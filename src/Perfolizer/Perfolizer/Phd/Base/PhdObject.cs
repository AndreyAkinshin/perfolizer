namespace Perfolizer.Phd.Base;

public class PhdObject
{
    public string? Display { get; set; }

    public virtual string? GetDisplay() => Display;
}
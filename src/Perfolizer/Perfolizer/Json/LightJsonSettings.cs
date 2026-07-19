namespace Perfolizer.Json;

public class LightJsonSettings
{
    public bool Indent { get; set; }

    public string NewLine
    {
        get => field ?? Environment.NewLine;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value != "\n" && value != "\r\n")
                throw new ArgumentOutOfRangeException(nameof(value));

            field = value;
        }
    }
}
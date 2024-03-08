namespace Perfolizer.Tests.Infra;

public static class VerifyHelper
{
    public static VerifySettings CreateSettings(string? typeName = null)
    {
        var settings = new VerifySettings();
        settings.UseDirectory("VerifiedFiles");
        settings.DisableDiff();
        if (typeName != null)
            settings.UseTypeName(typeName);

        return settings;
    }
}
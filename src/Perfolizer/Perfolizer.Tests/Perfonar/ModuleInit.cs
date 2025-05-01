using System.Runtime.CompilerServices;

namespace Perfolizer.Tests.Perfonar;

public static class ModuleInit
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.UseStrictJson();
        VerifierSettings.AutoVerify();
        VerifierSettings.DontScrubGuids();
        VerifierSettings.DontScrubDateTimes();
        VerifierSettings.DontSortDictionaries();
    }

    [ModuleInitializer]
    public static void InitDerivePathInfo() =>
        DerivePathInfo(
            (_, _, type, method) => new(AttributeReader.GetProjectDirectory(), typeName: type.Name, method.Name));
}
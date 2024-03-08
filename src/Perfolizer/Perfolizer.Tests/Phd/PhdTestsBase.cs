using Perfolizer.Json;
using Perfolizer.Phd.Base;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Phd;

public class PhdTestsBase
{
    protected static Task VerifyPhd(PhdEntry entry, PhdSchema schema)
    {
        // TODO
        // JsonSerializerOptions jsonOptions = new()
        // {
        //     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //     TypeInfoResolver = new PolymorphicTypeResolver(schema)
        // };
        // string json = JsonSerializer.Serialize(entry.Serialize(), jsonOptions);
        string json = LightJsonSerializer.Serialize(entry);
        return VerifyJson(json, CreateSettings());
    }

    protected static VerifySettings CreateSettings() => VerifyHelper.CreateSettings("Phd");
}
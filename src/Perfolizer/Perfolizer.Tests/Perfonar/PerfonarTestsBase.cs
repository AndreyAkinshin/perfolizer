using Perfolizer.InfoModels;
using Perfolizer.Json;
using Perfolizer.Perfonar.Base;
using Perfolizer.Tests.Infra;

namespace Perfolizer.Tests.Perfonar;

public class PerfonarTestsBase
{
    protected static Task VerifyPerfonar(EntryInfo entry, PerfonarSchema schema)
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

    protected static VerifySettings CreateSettings() => VerifyHelper.CreateSettings("Perfonar");
}
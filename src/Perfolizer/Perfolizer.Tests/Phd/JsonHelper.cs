using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Perfolizer.Phd;
using Perfolizer.Phd.Base;

namespace Perfolizer.Tests.Phd;

public class PolymorphicTypeResolver(PhdSchema schema) : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        foreach (var implementation in schema.Implementations)
        {
            if (jsonTypeInfo.Type == implementation.Base)
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "$type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    DerivedTypes =
                    {
                        new JsonDerivedType(implementation.Derived, schema.Name)
                    }
                };
            }
        }

        return jsonTypeInfo;
    }
}
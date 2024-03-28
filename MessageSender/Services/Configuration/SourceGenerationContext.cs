using System.Text.Json.Serialization;

namespace MessageSender.Services.Configuration;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ApplicationSettings))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}

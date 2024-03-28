using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MessageSender.Services.Configuration;

public static class AppSettingsService
{
    public const string StateFileName = "_appState.json";
    private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        ReferenceHandler = ReferenceHandler.Preserve,
    };

public static void SaveAppState(ApplicationSettings applicationSettings, string filePath = StateFileName)
    {
        string lines = JsonSerializer.Serialize(applicationSettings, typeof(ApplicationSettings), _jsonSerializerOptions);

        File.WriteAllText(filePath, lines);
    }

    public static ApplicationSettings LoadAppState(string filePath = StateFileName)
    {
        if (File.Exists(filePath))
        {
            string lines = File.ReadAllText(filePath);
            var appSettings = JsonSerializer.Deserialize<ApplicationSettings>(lines, _jsonSerializerOptions);

            return appSettings ?? new();
        }

        return new();
    }

}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Opus.Mobile.Shared;

public class JSONSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
    };

    public static T? Deserialize<T>(string json) => !string.IsNullOrEmpty(json) ?
        JsonSerializer.Deserialize<T>(json, Options) : default;

    public static T? Deserialize<T>(byte[] bytes) =>
        JsonSerializer.Deserialize<T>(bytes, Options);

    public static async Task<T?> Deserialize<T>(Stream stream) =>
        await JsonSerializer.DeserializeAsync<T>(stream, Options);

    public static string Serialize<T>(T data) =>
        JsonSerializer.Serialize(data, Options);
}

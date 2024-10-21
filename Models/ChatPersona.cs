using System.Text.Json.Serialization;

namespace AIQueryVisualizer.Models;

public class ChatPersona
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("systemDescription")]
    public required string SystemDescription { get; set; }

    [JsonPropertyName("safeSystemDescription")]
    public string SafeSystemDescription => SystemDescription;

    [JsonPropertyName("memoryBalance")]
    public double MemoryBalance { get; set; } = 0.5;

    [JsonPropertyName("enabledPlugins")]
    public string[] EnabledPlugins { get; set; } = Array.Empty<string>();

    [JsonPropertyName("version")]
    public string Version { get; set; } = "2.0";
}
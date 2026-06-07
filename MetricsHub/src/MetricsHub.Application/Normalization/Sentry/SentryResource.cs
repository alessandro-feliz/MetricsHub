using System.Text.Json.Serialization;

namespace MetricsHub.Application.Normalization.Sentry;

public class SentryResource
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("group")]
    public string? Group { get; set; }

    [JsonPropertyName("environment")]
    public string? Environment { get; set; }
}

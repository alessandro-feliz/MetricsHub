using System.Text.Json.Serialization;

namespace MetricsHub.Application.Normalization.Pulse;

public class PulsePayload
{
    [JsonPropertyName("pulse_id")]
    public string? PulseId { get; set; }

    [JsonPropertyName("ts")]
    public DateTimeOffset Ts { get; set; }

    [JsonPropertyName("node")]
    public string? Node { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("metrics")]
    public PulseMetrics? Metrics { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];
}

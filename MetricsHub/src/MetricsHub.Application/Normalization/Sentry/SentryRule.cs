using System.Text.Json.Serialization;

namespace MetricsHub.Application.Normalization.Sentry;

public class SentryRule
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("threshold")]
    public double Threshold { get; set; }

    [JsonPropertyName("actual")]
    public double Actual { get; set; }

    [JsonPropertyName("metric")]
    public string? Metric { get; set; }

    [JsonPropertyName("severity")]
    public string? Severity { get; set; }
}

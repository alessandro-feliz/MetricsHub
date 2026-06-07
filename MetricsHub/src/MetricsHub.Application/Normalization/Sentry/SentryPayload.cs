using System.Text.Json.Serialization;

namespace MetricsHub.Application.Normalization.Sentry;

public class SentryPayload
{
    [JsonPropertyName("alert_id")]
    public string? AlertId { get; set; }

    [JsonPropertyName("fired_at")]
    public DateTimeOffset FiredAt { get; set; }

    [JsonPropertyName("resource")]
    public SentryResource? Resource { get; set; }

    [JsonPropertyName("rule")]
    public SentryRule? Rule { get; set; }

    [JsonPropertyName("correlation_id")]
    public string? CorrelationId { get; set; }
}

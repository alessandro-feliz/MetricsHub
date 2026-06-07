using System.Text.Json;
using MetricsHub.Application.Normalization.Exceptions;
using MetricsHub.Domain;

namespace MetricsHub.Application.Normalization.Sentry;

public class SentryNormalizer : IEventNormalizer
{
    private static readonly JsonSerializerOptions DeserializeOptions = new() { PropertyNameCaseInsensitive = true };

    public string SourceName => "sentry";

    public NormalizedEvent Normalize(string rawPayload)
    {
        SentryPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<SentryPayload>(rawPayload, DeserializeOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidPayloadException($"Invalid JSON: {ex.Message}");
        }

        if (payload is null)
        {
            throw new InvalidPayloadException("Sentry payload deserialized to null.");
        }

        if (string.IsNullOrWhiteSpace(payload.AlertId))
        {
            throw new InvalidPayloadException("alert_id is required.");
        }
        if (payload.Resource is null)
        {
            throw new InvalidPayloadException("resource is required.");
        }
        if (string.IsNullOrWhiteSpace(payload.Resource.Name))
        {
            throw new InvalidPayloadException("resource.name is required.");
        }

        return new NormalizedEvent
        {
            Id = Guid.NewGuid(),
            Source = EventSource.Sentry,
            SourceEventId = payload.AlertId,
            OccurredAt = payload.FiredAt,
            IngestedAt = DateTimeOffset.UtcNow,
            Node = payload.Resource.Name,
            Region = payload.Resource.Group ?? string.Empty,
            Environment = payload.Resource.Environment ?? string.Empty,
            CorrelationId = string.IsNullOrWhiteSpace(payload.CorrelationId) ? null : payload.CorrelationId,
            RawPayload = rawPayload,
            Metrics = payload.Rule is not null
                ? JsonSerializer.SerializeToDocument(payload.Rule)
                : JsonDocument.Parse("{}")
        };
    }
}

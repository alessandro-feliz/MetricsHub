using System.Text.Json;
using MetricsHub.Application.Normalization.Exceptions;
using MetricsHub.Domain;

namespace MetricsHub.Application.Normalization.Pulse;

public class PulseNormalizer : IEventNormalizer
{
    private static readonly JsonSerializerOptions DeserializeOptions = new() { PropertyNameCaseInsensitive = true };

    public string SourceName => "pulse";

    public NormalizedEvent Normalize(string rawPayload)
    {
        PulsePayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<PulsePayload>(rawPayload, DeserializeOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidPayloadException($"Invalid JSON: {ex.Message}");
        }

        if (payload is null)
        {
            throw new InvalidPayloadException("Pulse payload deserialized to null.");
        }

        if (string.IsNullOrWhiteSpace(payload.PulseId))
        {
            throw new InvalidPayloadException("pulse_id is required.");
        }
        if (string.IsNullOrWhiteSpace(payload.Node))
        {
            throw new InvalidPayloadException("node is required.");
        }
        if (string.IsNullOrWhiteSpace(payload.Region))
        {
            throw new InvalidPayloadException("region is required.");
        }

        return new NormalizedEvent
        {
            Id = Guid.NewGuid(),
            Source = EventSource.Pulse,
            SourceEventId = payload.PulseId,
            OccurredAt = payload.Ts,
            IngestedAt = DateTimeOffset.UtcNow,
            Node = payload.Node,
            Region = payload.Region,
            Environment = payload.Tags.FirstOrDefault() ?? string.Empty, //TODO: validate how tags are represented/sent
            CorrelationId = null,
            RawPayload = rawPayload,
            Metrics = payload.Metrics is not null
                ? JsonSerializer.SerializeToDocument(payload.Metrics)
                : JsonDocument.Parse("{}")
        };
    }
}

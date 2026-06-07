using System.Text.Json;

namespace MetricsHub.Domain;

public class NormalizedEvent
{
    public Guid Id { get; set; }
    public EventSource Source { get; set; }
    public string SourceEventId { get; set; } = default!;
    public DateTimeOffset OccurredAt { get; set; }
    public DateTimeOffset IngestedAt { get; set; }
    public string Node { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Environment { get; set; } = default!;
    public string? CorrelationId { get; set; }
    public string RawPayload { get; set; } = default!;
    public JsonDocument Metrics { get; set; } = default!;
}

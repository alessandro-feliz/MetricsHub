using System.Text.Json;

namespace MetricsHub.Domain;

public class NormalizedEvent
{
    public required Guid Id { get; set; }
    public required EventSource Source { get; set; }
    public required string SourceEventId { get; set; }
    public required DateTimeOffset OccurredAt { get; set; }
    public required DateTimeOffset IngestedAt { get; set; }
    public required string Node { get; set; }
    public required string Region { get; set; }
    public required string Environment { get; set; }
    public required string? CorrelationId { get; set; }
    public required string RawPayload { get; set; }
    public required JsonDocument Metrics { get; set; }
}

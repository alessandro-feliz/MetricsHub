using System.Text.Json;
using MetricsHub.Domain;

namespace MetricsHub.Api.Controllers.Events;

public record EventResponse(Guid Id, EventSource Source, string SourceEventId, DateTimeOffset OccurredAt, DateTimeOffset IngestedAt, string Node, string Region, string Environment, string? CorrelationId, JsonDocument Metrics)
{
    public static EventResponse From(NormalizedEvent e) => new(e.Id, e.Source, e.SourceEventId, e.OccurredAt, e.IngestedAt, e.Node, e.Region, e.Environment, e.CorrelationId, e.Metrics);
}

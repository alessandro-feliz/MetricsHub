using MetricsHub.Domain;

namespace MetricsHub.Api.Controllers.Webhooks;

public record WebhookResponse(Guid Id, EventSource Source, string SourceEventId, DateTimeOffset OccurredAt, DateTimeOffset IngestedAt, string Node, string? CorrelationId)
{
    public static WebhookResponse From(NormalizedEvent e) => new(e.Id, e.Source, e.SourceEventId, e.OccurredAt, e.IngestedAt, e.Node, e.CorrelationId);
}

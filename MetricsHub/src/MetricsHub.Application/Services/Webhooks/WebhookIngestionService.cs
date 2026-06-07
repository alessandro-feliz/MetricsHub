using MetricsHub.Application.Normalization;
using MetricsHub.Domain;
using MetricsHub.Infrastructure;

namespace MetricsHub.Application.Services.Webhooks;

public class WebhookIngestionService(NormalizerStrategy normalizer, MetricsHubDbContext db)
{
    public async Task<NormalizedEvent> IngestAsync(string sourceName, string rawPayload)
    {
        var normalized = normalizer.Normalize(sourceName, rawPayload);

        db.Events.Add(normalized);
        await db.SaveChangesAsync();

        return normalized;
    }
}

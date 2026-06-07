using MetricsHub.Application.Normalization;
using MetricsHub.Application.Exceptions;
using MetricsHub.Domain;
using MetricsHub.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MetricsHub.Application.Services.Webhooks;

public class WebhookIngestionService(NormalizerStrategy normalizer, MetricsHubDbContext db, ILogger<WebhookIngestionService> logger)
{
    public async Task<NormalizedEvent> IngestAsync(string sourceName, string rawPayload, CancellationToken ct = default)
    {
        NormalizedEvent normalized;
        try
        {
            normalized = normalizer.Normalize(sourceName, rawPayload);
        }
        catch (MetricsHubException ex)
        {
            logger.LogWarning(ex, "Normalization failed for source {Source}", sourceName);
            throw;
        }

        try
        {
            db.Events.Add(normalized);
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to persist event {SourceEventId} from {Source}", normalized.SourceEventId, normalized.Source);
            throw;
        }

        logger.LogInformation("Ingested event {SourceEventId} from {Source}", normalized.SourceEventId, normalized.Source);

        return normalized;
    }
}

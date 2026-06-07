using MetricsHub.Application.Exceptions;
using MetricsHub.Application.Normalization;
using MetricsHub.Domain;
using MetricsHub.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

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

            logger.LogInformation("Ingested event {SourceEventId} from {Source}", normalized.SourceEventId, normalized.Source);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            logger.LogInformation("Duplicate event {SourceEventId} from {Source} — already ingested, skipping", normalized.SourceEventId, normalized.Source);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to persist event {SourceEventId} from {Source}", normalized.SourceEventId, normalized.Source);
            throw;
        }

        return normalized;
    }
}

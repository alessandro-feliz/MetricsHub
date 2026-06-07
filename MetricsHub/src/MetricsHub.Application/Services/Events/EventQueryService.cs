using MetricsHub.Domain;
using MetricsHub.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MetricsHub.Application.Services.Events;

public class EventQueryService(MetricsHubDbContext db, ILogger<EventQueryService> logger)
{
    public async Task<PagedResult<NormalizedEvent>> QueryAsync(EventQuery query, CancellationToken ct = default)
    {
        logger.LogInformation("Querying events: source={Source} node={Node} from={From} to={To} page={Page} pageSize={PageSize}",
            query.Source, query.Node, query.From, query.To, query.Page, query.PageSize);

        var events = db.Events.AsNoTracking().AsQueryable();

        if (query.From.HasValue)
        {
            events = events.Where(e => e.OccurredAt >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            events = events.Where(e => e.OccurredAt <= query.To.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Source) && Enum.TryParse<EventSource>(query.Source, ignoreCase: true, out var source))
        {
            events = events.Where(e => e.Source == source);
        }

        if (!string.IsNullOrWhiteSpace(query.Node))
        {
            events = events.Where(e => e.Node == query.Node);
        }

        var totalCount = await events.CountAsync(ct);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var page = Math.Max(query.Page, 1);

        var items = await events
            .OrderByDescending(e => e.OccurredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<NormalizedEvent>(items, totalCount, page, pageSize);
    }
}

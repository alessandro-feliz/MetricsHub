using MetricsHub.Domain;
using MetricsHub.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MetricsHub.Application.Services.Events;

public class EventQueryService(MetricsHubDbContext db)
{
    public async Task<PagedResult<NormalizedEvent>> QueryAsync(EventQuery query, CancellationToken ct = default)
    {
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

        if (!string.IsNullOrWhiteSpace(query.Resource))
        {
            events = events.Where(e => e.Node.Contains(query.Resource));
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

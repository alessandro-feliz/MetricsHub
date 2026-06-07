namespace MetricsHub.Application.Services.Events;

public class EventQuery
{
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public string? Source { get; set; }
    public string? Resource { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

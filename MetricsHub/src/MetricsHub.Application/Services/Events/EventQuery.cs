using System.ComponentModel.DataAnnotations;

namespace MetricsHub.Application.Services.Events;

public class EventQuery
{
    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }

    public string? Source { get; set; }

    public string? Node { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than or equal to 1.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = 20;
}

using MetricsHub.Application.Services.Events;
using Microsoft.AspNetCore.Mvc;

namespace MetricsHub.Api.Controllers.Events;

[ApiController]
[Route("events")]
public class EventsController(EventQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Query([FromQuery] EventQuery query, CancellationToken ct)
    {
        var result = await queryService.QueryAsync(query, ct);

        var response = new PagedResult<EventResponse>(result.Items.Select(EventResponse.From).ToList(), result.TotalCount, result.Page, result.PageSize);

        return Ok(response);
    }
}

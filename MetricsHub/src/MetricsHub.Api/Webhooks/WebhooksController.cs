using MetricsHub.Application.Services.Webhooks;
using Microsoft.AspNetCore.Mvc;

namespace MetricsHub.Api.Webhooks;

[ApiController]
[Route("webhooks")]
public class WebhooksController(WebhookIngestionService ingestionService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Ingest([FromHeader(Name = "X-Source")] string? source, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return BadRequest(new { error = "X-Source header is required." });
        }

        string rawPayload;
        using (var reader = new StreamReader(Request.Body))
        {
            rawPayload = await reader.ReadToEndAsync(ct);
        }

        var result = await ingestionService.IngestAsync(source, rawPayload, ct);

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                result.Id,
                result.Source,
                result.SourceEventId,
                result.OccurredAt,
                result.IngestedAt,
                result.Node,
                result.Region,
                result.Environment,
                result.CorrelationId,
            }
        );
    }
}

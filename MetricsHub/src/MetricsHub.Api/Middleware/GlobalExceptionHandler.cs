using MetricsHub.Application.Normalization.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace MetricsHub.Api.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is OperationCanceledException)
        {
            return true;
        }

        var (status, message) = exception switch
        {
            UnknownSourceException ex => (StatusCodes.Status400BadRequest, ex.Message),
            InvalidPayloadException ex => (StatusCodes.Status422UnprocessableEntity, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred."),
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new { error = message }, ct);
        return true;
    }
}

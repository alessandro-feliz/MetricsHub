using MetricsHub.Application.Exceptions;

namespace MetricsHub.Application.Normalization.Exceptions;

public class InvalidPayloadException(string message) : MetricsHubException(message);

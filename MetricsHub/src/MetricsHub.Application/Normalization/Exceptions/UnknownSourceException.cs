using MetricsHub.Application.Exceptions;

namespace MetricsHub.Application.Normalization.Exceptions;

public class UnknownSourceException(string sourceName) : MetricsHubException($"Unknown source: '{sourceName}'.")
{
    public string SourceName { get; } = sourceName;
}

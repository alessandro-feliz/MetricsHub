namespace MetricsHub.Application.Normalization.Exceptions;

public class UnknownSourceException(string sourceName)
    : Exception($"Unknown source: '{sourceName}'.")
{
    public string SourceName { get; } = sourceName;
}

using MetricsHub.Domain;

namespace MetricsHub.Application.Normalization;

public interface IEventNormalizer
{
    string SourceName { get; }

    NormalizedEvent Normalize(string rawPayload);
}

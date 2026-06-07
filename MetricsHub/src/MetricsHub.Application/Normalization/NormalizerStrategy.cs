using MetricsHub.Application.Normalization.Exceptions;
using MetricsHub.Domain;

namespace MetricsHub.Application.Normalization;

public class NormalizerStrategy(IEnumerable<IEventNormalizer> normalizers)
{
    private readonly Dictionary<string, IEventNormalizer> _normalizers = normalizers.ToDictionary(n => n.SourceName, StringComparer.OrdinalIgnoreCase);

    public NormalizedEvent Normalize(string sourceName, string rawPayload) =>
        _normalizers.TryGetValue(sourceName, out var normalizer)
            ? normalizer.Normalize(rawPayload)
            : throw new UnknownSourceException(sourceName);
}

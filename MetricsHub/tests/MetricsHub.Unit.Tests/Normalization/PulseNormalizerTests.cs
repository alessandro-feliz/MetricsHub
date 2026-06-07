using FluentAssertions;
using MetricsHub.Application.Normalization.Exceptions;
using MetricsHub.Application.Normalization.Pulse;
using MetricsHub.Domain;

namespace MetricsHub.Unit.Tests.Normalization;

public class PulseNormalizerTests
{
    private readonly PulseNormalizer _sut = new();

    private const string ValidPayload = """
        {
            "pulse_id": "p-88a1c3e7",
            "ts": "2026-05-07T14:32:01.442Z",
            "node": "worker-us-east-07",
            "region": "us-east-1",
            "metrics": {
                "cpu_pct": 72.4,
                "mem_mb": 3021,
                "active_conns": 148,
                "status": "healthy"
            },
            "tags": ["production", "tier-1"]
        }
        """;

    [Fact]
    public void Normalize_ValidPayload_MapsSourceAndSourceEventId()
    {
        var result = _sut.Normalize(ValidPayload);

        result.Source.Should().Be(EventSource.Pulse);
        result.SourceEventId.Should().Be("p-88a1c3e7");
    }

    [Fact]
    public void Normalize_ValidPayload_MapsNodeRegionEnvironment()
    {
        var result = _sut.Normalize(ValidPayload);

        result.Node.Should().Be("worker-us-east-07");
        result.Region.Should().Be("us-east-1");
        result.Environment.Should().Be("production");
    }

    [Fact]
    public void Normalize_ValidPayload_SetsCorrelationIdToNull()
    {
        var result = _sut.Normalize(ValidPayload);

        result.CorrelationId.Should().BeNull();
    }

    [Fact]
    public void Normalize_ValidPayload_MapsOccurredAt()
    {
        var result = _sut.Normalize(ValidPayload);

        result.OccurredAt.Should().Be(DateTimeOffset.Parse("2026-05-07T14:32:01.442Z"));
    }

    [Fact]
    public void Normalize_ValidPayload_StoresRawPayload()
    {
        var result = _sut.Normalize(ValidPayload);

        result.RawPayload.Should().Be(ValidPayload);
    }

    [Fact]
    public void Normalize_NoTags_SetsEnvironmentToEmptyString()
    {
        var payload = """{"pulse_id":"p-1","ts":"2026-01-01T00:00:00Z","node":"n1","region":"us-east-1"}""";

        var result = _sut.Normalize(payload);

        result.Environment.Should().BeEmpty();
    }

    [Fact]
    public void Normalize_MissingPulseId_ThrowsInvalidPayloadException()
    {
        var payload = """{"ts":"2026-01-01T00:00:00Z","node":"n1","region":"us-east-1"}""";

        var act = () => _sut.Normalize(payload);

        act.Should().Throw<InvalidPayloadException>().WithMessage("*pulse_id*");
    }

    [Fact]
    public void Normalize_MissingNode_ThrowsInvalidPayloadException()
    {
        var payload = """{"pulse_id":"p-1","ts":"2026-01-01T00:00:00Z","region":"us-east-1"}""";

        var act = () => _sut.Normalize(payload);

        act.Should().Throw<InvalidPayloadException>().WithMessage("*node*");
    }

    [Fact]
    public void Normalize_MissingRegion_ThrowsInvalidPayloadException()
    {
        var payload = """{"pulse_id":"p-1","ts":"2026-01-01T00:00:00Z","node":"n1"}""";

        var act = () => _sut.Normalize(payload);

        act.Should().Throw<InvalidPayloadException>().WithMessage("*region*");
    }

    [Fact]
    public void Normalize_MalformedJson_ThrowsException()
    {
        var act = () => _sut.Normalize("not-json");

        act.Should().Throw<Exception>();
    }
}

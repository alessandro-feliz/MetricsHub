using FluentAssertions;
using MetricsHub.Application.Normalization.Exceptions;
using MetricsHub.Application.Normalization.Sentry;
using MetricsHub.Domain;

namespace MetricsHub.Unit.Tests.Normalization;

public class SentryNormalizerTests
{
    private readonly SentryNormalizer _sut = new();

    private const string ValidPayload = """
        {
            "alert_id": "a-44f1-9bc2",
            "fired_at": "2026-05-07T14:32:18Z",
            "resource": {
                "name": "worker-us-east-07",
                "group": "us-east-1",
                "environment": "production"
            },
            "rule": {
                "name": "High CPU",
                "threshold": 70.0,
                "actual": 72.4,
                "metric": "cpu_pct",
                "severity": "warning"
            },
            "correlation_id": "p-88a1c3e7"
        }
        """;

    [Fact]
    public void Normalize_ValidPayload_MapsSourceAndSourceEventId()
    {
        var result = _sut.Normalize(ValidPayload);

        result.Source.Should().Be(EventSource.Sentry);
        result.SourceEventId.Should().Be("a-44f1-9bc2");
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
    public void Normalize_ValidPayload_MapsCorrelationId()
    {
        var result = _sut.Normalize(ValidPayload);

        result.CorrelationId.Should().Be("p-88a1c3e7");
    }

    [Fact]
    public void Normalize_ValidPayload_MapsOccurredAt()
    {
        var result = _sut.Normalize(ValidPayload);

        result.OccurredAt.Should().Be(DateTimeOffset.Parse("2026-05-07T14:32:18Z"));
    }

    [Fact]
    public void Normalize_ValidPayload_StoresRawPayload()
    {
        var result = _sut.Normalize(ValidPayload);

        result.RawPayload.Should().Be(ValidPayload);
    }

    [Fact]
    public void Normalize_NullCorrelationId_SetsCorrelationIdToNull()
    {
        var payload = """
            {
                "alert_id":"a-1","fired_at":"2026-01-01T00:00:00Z",
                "resource":{"name":"n1","group":"g1","environment":"prod"},
                "correlation_id": null
            }
            """;

        var result = _sut.Normalize(payload);

        result.CorrelationId.Should().BeNull();
    }

    [Fact]
    public void Normalize_EmptyCorrelationId_SetsCorrelationIdToNull()
    {
        var payload = """
            {
                "alert_id":"a-1","fired_at":"2026-01-01T00:00:00Z",
                "resource":{"name":"n1","group":"g1","environment":"prod"},
                "correlation_id": ""
            }
            """;

        var result = _sut.Normalize(payload);

        result.CorrelationId.Should().BeNull();
    }

    [Fact]
    public void Normalize_MissingAlertId_ThrowsInvalidPayloadException()
    {
        var payload = """
            {"fired_at":"2026-01-01T00:00:00Z","resource":{"name":"n1","group":"g1","environment":"prod"}}
            """;

        var act = () => _sut.Normalize(payload);

        act.Should().Throw<InvalidPayloadException>().WithMessage("*alert_id*");
    }

    [Fact]
    public void Normalize_MissingResource_ThrowsInvalidPayloadException()
    {
        var payload = """{"alert_id":"a-1","fired_at":"2026-01-01T00:00:00Z"}""";

        var act = () => _sut.Normalize(payload);

        act.Should().Throw<InvalidPayloadException>().WithMessage("*resource*");
    }

    [Fact]
    public void Normalize_MissingResourceName_ThrowsInvalidPayloadException()
    {
        var payload = """
            {"alert_id":"a-1","fired_at":"2026-01-01T00:00:00Z","resource":{"group":"g1","environment":"prod"}}
            """;

        var act = () => _sut.Normalize(payload);

        act.Should().Throw<InvalidPayloadException>().WithMessage("*resource.name*");
    }

    [Fact]
    public void Normalize_MalformedJson_ThrowsException()
    {
        var act = () => _sut.Normalize("not-json");

        act.Should().Throw<Exception>();
    }
}

using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace MetricsHub.Integration.Tests.Webhooks;

public class WebhooksControllerTests(MetricsHubWebApplicationFactory factory)
    : IClassFixture<MetricsHubWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private const string PulsePayload = """
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

    private const string SentryPayload = """
        {
            "alert_id": "sentry-alert-001",
            "fired_at": "2026-05-07T15:00:00Z",
            "resource": {
                "name": "api-gateway",
                "group": "us-west-2",
                "environment": "production"
            },
            "rule": {
                "name": "High Error Rate",
                "threshold": 5.0,
                "actual": 12.3,
                "metric": "error_rate",
                "severity": "critical"
            },
            "correlation_id": "corr-xyz-789"
        }
        """;

    private static StringContent JsonContent(string json) =>
        new(json, new MediaTypeHeaderValue("application/json"));

    [Fact]
    public async Task Post_ValidPulsePayload_Returns201()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
        {
            Content = JsonContent(PulsePayload),
            Headers = { { "X-Source", "pulse" } },
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_ValidSentryPayload_Returns201()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
        {
            Content = JsonContent(SentryPayload),
            Headers = { { "X-Source", "sentry" } },
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_MissingXSourceHeader_Returns400()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
        {
            Content = JsonContent(PulsePayload),
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_UnknownSource_Returns400()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
        {
            Content = JsonContent(PulsePayload),
            Headers = { { "X-Source", "unknown-source" } },
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_MalformedJson_Returns422()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
        {
            Content = JsonContent("not-valid-json"),
            Headers = { { "X-Source", "pulse" } },
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Post_MissingRequiredField_Returns422()
    {
        var payload = """{"ts":"2026-01-01T00:00:00Z","node":"n1","region":"us-east-1"}""";
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
        {
            Content = JsonContent(payload),
            Headers = { { "X-Source", "pulse" } },
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Post_DuplicateEvent_Returns201BothTimes()
    {
        var payload = """
            {
                "pulse_id": "p-duplicate-test",
                "ts": "2026-05-07T12:00:00Z",
                "node": "worker-dup-01",
                "region": "us-east-1",
                "tags": ["production"]
            }
            """;

        var first = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
        {
            Content = JsonContent(payload),
            Headers = { { "X-Source", "pulse" } },
        };
        var second = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
        {
            Content = JsonContent(payload),
            Headers = { { "X-Source", "pulse" } },
        };

        var firstResponse = await _client.SendAsync(first);
        var secondResponse = await _client.SendAsync(second);

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}

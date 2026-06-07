using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;

namespace MetricsHub.Integration.Tests.Events;

public class EventsControllerTests(MetricsHubWebApplicationFactory factory)
    : IClassFixture<MetricsHubWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static string PulsePayload(string pulseId, string node, string ts) => $$"""
        {
            "pulse_id": "{{pulseId}}",
            "ts": "{{ts}}",
            "node": "{{node}}",
            "region": "us-east-1",
            "tags": ["production"]
        }
        """;

    private static string SentryPayload(string alertId, string name, string firedAt) => $$"""
        {
            "alert_id": "{{alertId}}",
            "fired_at": "{{firedAt}}",
            "resource": { "name": "{{name}}", "group": "us-west-2", "environment": "production" },
            "correlation_id": ""
        }
        """;

    private async Task PostWebhookAsync(string source, string payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
        {
            Content = new StringContent(payload, new MediaTypeHeaderValue("application/json")),
            Headers = { { "X-Source", source } },
        };
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Get_NoFilters_ReturnsPagedResultStructure()
    {
        await PostWebhookAsync("pulse", PulsePayload("p-struct-1", "node-struct-01", "2026-05-01T10:00:00Z"));
        await PostWebhookAsync("pulse", PulsePayload("p-struct-2", "node-struct-02", "2026-05-01T11:00:00Z"));

        var response = await _client.GetAsync("/events");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        body.GetProperty("totalCount").GetInt32().Should().BeGreaterThanOrEqualTo(2);
        body.GetProperty("page").GetInt32().Should().Be(1);
        body.GetProperty("pageSize").GetInt32().Should().Be(20);
        body.GetProperty("items").GetArrayLength().Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Get_FilterBySourcePulse_ReturnsOnlyPulseEvents()
    {
        await PostWebhookAsync("pulse", PulsePayload("p-src-1", "node-srcfilter", "2026-05-02T10:00:00Z"));
        await PostWebhookAsync("sentry", SentryPayload("s-src-1", "node-srcfilter", "2026-05-02T10:00:00Z"));

        var response = await _client.GetAsync("/events?source=pulse&resource=node-srcfilter");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        foreach (var item in body.GetProperty("items").EnumerateArray())
        {
            item.GetProperty("source").GetString().Should().Be("Pulse");
        }
    }

    [Fact]
    public async Task Get_FilterByResource_ReturnsOnlyMatchingNode()
    {
        var uniqueNode = "node-res-unique-xyz";
        await PostWebhookAsync("pulse", PulsePayload("p-res-1", uniqueNode, "2026-05-03T10:00:00Z"));

        var response = await _client.GetAsync($"/events?resource={uniqueNode}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        body.GetProperty("totalCount").GetInt32().Should().Be(1);
        body.GetProperty("items").EnumerateArray().First()
            .GetProperty("node").GetString().Should().Be(uniqueNode);
    }

    [Fact]
    public async Task Get_FilterByTimeRange_NarrowsToMatchingEvents()
    {
        await PostWebhookAsync("pulse", PulsePayload("p-time-early", "node-timerange", "2024-01-01T00:00:00Z"));
        await PostWebhookAsync("pulse", PulsePayload("p-time-late", "node-timerange", "2026-06-01T00:00:00Z"));

        var response = await _client.GetAsync(
            "/events?resource=node-timerange&from=2026-01-01T00:00:00Z&to=2026-12-31T23:59:59Z");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        body.GetProperty("totalCount").GetInt32().Should().Be(1);
        body.GetProperty("items").EnumerateArray().First()
            .GetProperty("sourceEventId").GetString().Should().Be("p-time-late");
    }

    [Fact]
    public async Task Get_Pagination_ReturnsCorrectPageMetadata()
    {
        var nodePrefix = "node-page";
        await PostWebhookAsync("pulse", PulsePayload("p-page-1", $"{nodePrefix}-a", "2026-05-04T10:00:00Z"));
        await PostWebhookAsync("pulse", PulsePayload("p-page-2", $"{nodePrefix}-b", "2026-05-04T11:00:00Z"));
        await PostWebhookAsync("pulse", PulsePayload("p-page-3", $"{nodePrefix}-c", "2026-05-04T12:00:00Z"));

        var response = await _client.GetAsync($"/events?resource={nodePrefix}&pageSize=2&page=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        body.GetProperty("totalCount").GetInt32().Should().Be(3);
        body.GetProperty("page").GetInt32().Should().Be(1);
        body.GetProperty("pageSize").GetInt32().Should().Be(2);
        body.GetProperty("items").GetArrayLength().Should().Be(2);
    }

    [Fact]
    public async Task Get_SecondPage_ReturnsRemainingItems()
    {
        var nodePrefix = "node-page2";
        await PostWebhookAsync("pulse", PulsePayload("p-page2-1", $"{nodePrefix}-a", "2026-05-05T10:00:00Z"));
        await PostWebhookAsync("pulse", PulsePayload("p-page2-2", $"{nodePrefix}-b", "2026-05-05T11:00:00Z"));
        await PostWebhookAsync("pulse", PulsePayload("p-page2-3", $"{nodePrefix}-c", "2026-05-05T12:00:00Z"));

        var response = await _client.GetAsync($"/events?resource={nodePrefix}&pageSize=2&page=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        body.GetProperty("items").GetArrayLength().Should().Be(1);
    }
}

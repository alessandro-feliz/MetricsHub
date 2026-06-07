using System.Text.Json.Serialization;

namespace MetricsHub.Application.Normalization.Pulse;

public class PulseMetrics
{
    [JsonPropertyName("cpu_pct")]
    public double CpuPct { get; set; }

    [JsonPropertyName("mem_mb")]
    public int MemMb { get; set; }

    [JsonPropertyName("active_conns")]
    public int ActiveConns { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

using MetricsHub.Application.Normalization;
using MetricsHub.Application.Normalization.Pulse;
using MetricsHub.Application.Normalization.Sentry;
using MetricsHub.Application.Services.Events;
using MetricsHub.Application.Services.Webhooks;
using MetricsHub.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<MetricsHubDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<MetricsHubDbContext>();

builder.Services.AddScoped<IEventNormalizer, PulseNormalizer>();
builder.Services.AddScoped<IEventNormalizer, SentryNormalizer>();
builder.Services.AddScoped<NormalizerStrategy>();
builder.Services.AddScoped<WebhookIngestionService>();
builder.Services.AddScoped<EventQueryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MetricsHubDbContext>();
    await db.Database.MigrateAsync();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { } // required for integration tests

using Microsoft.EntityFrameworkCore;
using MetricsHub.Domain;

namespace MetricsHub.Infrastructure;

public class MetricsHubDbContext(DbContextOptions<MetricsHubDbContext> options) : DbContext(options)
{
    public DbSet<NormalizedEvent> Events => Set<NormalizedEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NormalizedEvent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Source)
                  .HasConversion<string>();

            entity.Property(e => e.Metrics)
                  .HasColumnType("jsonb");

            entity.HasIndex(e => new { e.Source, e.SourceEventId })
                  .IsUnique();
        });
    }
}

using System.Text.Json;
using CoinPulse.Core;
using Microsoft.EntityFrameworkCore;

namespace CoinPulse.Services.Data;

public class AppDbContext : DbContext
{
    public DbSet<Coin> Coins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=coinpulse.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Coin>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.SparklineIn7D)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<SparklineData>(v, (JsonSerializerOptions?)null)
                );
        });
    }
}
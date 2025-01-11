using DotnetAiApp.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetAiApp.Db;

public class DotentAiAppContext : DbContext
{
    public DbSet<GoldPrice> GoldPrices { get; set; }

    public DotentAiAppContext(DbContextOptions<DotentAiAppContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GoldPriceConfiguration());
    }
}
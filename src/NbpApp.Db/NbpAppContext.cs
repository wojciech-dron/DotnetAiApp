using Microsoft.EntityFrameworkCore;
using NbpApp.Db.Entities;

namespace NbpApp.Db;

public class NbpAppContext : DbContext
{
    public DbSet<GoldPrice> GoldPrices { get; set; }

    public NbpAppContext(DbContextOptions<NbpAppContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GoldPriceConfiguration());
    }
}
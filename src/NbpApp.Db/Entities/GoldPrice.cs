using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NbpApp.Db.Entities;

public class GoldPrice
{
    public DateOnly Date { get; set; }
    public decimal? Price { get; set; }
}

internal class GoldPriceConfiguration : IEntityTypeConfiguration<GoldPrice>
{
    public void Configure(EntityTypeBuilder<GoldPrice> builder)
    {
        builder.ToTable("GoldPrices");

        builder.HasKey(x => x.Date);
        builder.Property(x => x.Date)
            .HasColumnType("date");

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();
    }
}
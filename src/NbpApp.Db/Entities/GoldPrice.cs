using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NbpApp.Db.Entities;

public class GoldPrice
{
    public DateOnly Date { get; set; }

    /// <remarks> SQLite does not support decimal ORDER BY expressions </remarks>
    public double Price { get; set; }
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
            .HasColumnType("double")
            .IsRequired();
    }
}
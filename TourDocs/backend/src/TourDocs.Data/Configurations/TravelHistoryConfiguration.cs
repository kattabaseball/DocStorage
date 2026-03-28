using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class TravelHistoryConfiguration : IEntityTypeConfiguration<TravelHistory>
{
    public void Configure(EntityTypeBuilder<TravelHistory> builder)
    {
        builder.ToTable("TravelHistories");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.VisaType)
            .HasMaxLength(100);

        builder.Property(t => t.EntryDate)
            .HasColumnType("datetime2");

        builder.Property(t => t.ExitDate)
            .HasColumnType("datetime2");

        builder.Property(t => t.Purpose)
            .HasMaxLength(500);

        builder.Property(t => t.Notes)
            .HasMaxLength(1000);

        builder.HasOne(t => t.Member)
            .WithMany(m => m.TravelHistory)
            .HasForeignKey(t => t.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.MemberId);
    }
}

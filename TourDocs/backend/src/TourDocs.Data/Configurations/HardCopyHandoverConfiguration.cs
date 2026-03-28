using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class HardCopyHandoverConfiguration : IEntityTypeConfiguration<HardCopyHandover>
{
    public void Configure(EntityTypeBuilder<HardCopyHandover> builder)
    {
        builder.ToTable("HardCopyHandovers");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.FromRole)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.ToRole)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.HandoverType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.ConfirmationMethod)
            .HasMaxLength(50);

        builder.Property(h => h.ConfirmationData)
            .HasMaxLength(1000);

        builder.Property(h => h.Notes)
            .HasMaxLength(2000);

        builder.Property(h => h.RecordedAt)
            .HasColumnType("datetime2");

        builder.HasOne(h => h.HardCopyRequest)
            .WithMany(r => r.Handovers)
            .HasForeignKey(h => h.HardCopyRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.FromUser)
            .WithMany()
            .HasForeignKey(h => h.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.ToUser)
            .WithMany()
            .HasForeignKey(h => h.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => h.HardCopyRequestId);
    }
}

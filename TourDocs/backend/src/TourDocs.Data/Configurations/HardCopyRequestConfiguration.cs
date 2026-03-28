using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class HardCopyRequestConfiguration : IEntityTypeConfiguration<HardCopyRequest>
{
    public void Configure(EntityTypeBuilder<HardCopyRequest> builder)
    {
        builder.ToTable("HardCopyRequests");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(h => h.Urgency)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(h => h.Notes)
            .HasMaxLength(2000);

        builder.HasOne(h => h.Document)
            .WithMany()
            .HasForeignKey(h => h.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.Case)
            .WithMany(c => c.HardCopyRequests)
            .HasForeignKey(h => h.CaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.RequestedByUser)
            .WithMany()
            .HasForeignKey(h => h.RequestedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => h.DocumentId);
        builder.HasIndex(h => h.CaseId);
        builder.HasIndex(h => h.Status);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class DocumentRequestConfiguration : IEntityTypeConfiguration<DocumentRequest>
{
    public void Configure(EntityTypeBuilder<DocumentRequest> builder)
    {
        builder.ToTable("DocumentRequests");

        builder.HasKey(dr => dr.Id);

        builder.Property(dr => dr.DocumentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(dr => dr.FormatRequirements)
            .HasMaxLength(500);

        builder.Property(dr => dr.Urgency)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(dr => dr.Notes)
            .HasMaxLength(2000);

        builder.Property(dr => dr.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(dr => dr.DeclineReason)
            .HasMaxLength(1000);

        builder.HasOne(dr => dr.Case)
            .WithMany(c => c.DocumentRequests)
            .HasForeignKey(dr => dr.CaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(dr => dr.Member)
            .WithMany()
            .HasForeignKey(dr => dr.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dr => dr.RequestedByUser)
            .WithMany()
            .HasForeignKey(dr => dr.RequestedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dr => dr.FulfilledDocument)
            .WithMany()
            .HasForeignKey(dr => dr.FulfilledDocumentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(dr => dr.MemberId);
        builder.HasIndex(dr => dr.CaseId);
        builder.HasIndex(dr => dr.Status);
    }
}

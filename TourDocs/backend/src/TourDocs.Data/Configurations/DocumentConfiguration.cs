using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Category)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.DocumentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.ExpiryDate)
            .HasColumnType("datetime2");

        builder.Property(d => d.ExtractedData)
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.VerificationNotes)
            .HasMaxLength(2000);

        builder.Property(d => d.VerifiedAt)
            .HasColumnType("datetime2");

        builder.Property(d => d.DeletedAt)
            .HasColumnType("datetime2");

        builder.HasOne(d => d.Member)
            .WithMany(m => m.Documents)
            .HasForeignKey(d => d.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Organization)
            .WithMany()
            .HasForeignKey(d => d.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.CurrentVersion)
            .WithMany()
            .HasForeignKey(d => d.CurrentVersionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(d => d.MemberId);
        builder.HasIndex(d => d.OrganizationId);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.ExpiryDate);
        builder.HasIndex(d => new { d.MemberId, d.Category });
    }
}

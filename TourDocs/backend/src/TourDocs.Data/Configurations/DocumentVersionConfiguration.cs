using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("DocumentVersions");

        builder.HasKey(dv => dv.Id);

        builder.Property(dv => dv.FileName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(dv => dv.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(dv => dv.MimeType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(dv => dv.Checksum)
            .HasMaxLength(128);

        builder.Property(dv => dv.UploadedAt)
            .HasColumnType("datetime2");

        builder.HasOne(dv => dv.Document)
            .WithMany(d => d.Versions)
            .HasForeignKey(dv => dv.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(dv => dv.DocumentId);
        builder.HasIndex(dv => new { dv.DocumentId, dv.VersionNumber })
            .IsUnique();
    }
}

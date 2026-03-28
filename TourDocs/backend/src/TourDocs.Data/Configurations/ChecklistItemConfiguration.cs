using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
{
    public void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        builder.ToTable("ChecklistItems");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.DocumentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ci => ci.DocumentCategory)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ci => ci.Description)
            .HasMaxLength(500);

        builder.Property(ci => ci.FormatNotes)
            .HasMaxLength(500);

        builder.HasOne(ci => ci.Checklist)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.ChecklistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ci => ci.ChecklistId);
    }
}

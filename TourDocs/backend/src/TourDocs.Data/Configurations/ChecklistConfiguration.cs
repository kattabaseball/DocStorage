using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class ChecklistConfiguration : IEntityTypeConfiguration<Checklist>
{
    public void Configure(EntityTypeBuilder<Checklist> builder)
    {
        builder.ToTable("Checklists");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CountryCode)
            .HasMaxLength(10);

        builder.Property(c => c.CountryName)
            .HasMaxLength(100);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.ChecklistType)
            .HasMaxLength(100);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        builder.HasOne(c => c.Organization)
            .WithMany()
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.CountryCode);
        builder.HasIndex(c => c.OrganizationId);
    }
}

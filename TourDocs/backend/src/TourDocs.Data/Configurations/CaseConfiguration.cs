using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("Cases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(c => c.CaseType)
            .HasMaxLength(100);

        builder.Property(c => c.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(c => c.DestinationCountry)
            .HasMaxLength(100);

        builder.Property(c => c.DestinationCity)
            .HasMaxLength(100);

        builder.Property(c => c.Venue)
            .HasMaxLength(300);

        builder.Property(c => c.StartDate)
            .HasColumnType("datetime2");

        builder.Property(c => c.EndDate)
            .HasColumnType("datetime2");

        builder.Property(c => c.ContactName)
            .HasMaxLength(200);

        builder.Property(c => c.ContactEmail)
            .HasMaxLength(200);

        builder.Property(c => c.ContactPhone)
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        builder.Property(c => c.DeletedAt)
            .HasColumnType("datetime2");

        builder.HasOne(c => c.Organization)
            .WithMany(o => o.Cases)
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Checklist)
            .WithMany(cl => cl.Cases)
            .HasForeignKey(c => c.ChecklistId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.OrganizationId);
        builder.HasIndex(c => c.ReferenceNumber);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => new { c.OrganizationId, c.Status });
    }
}

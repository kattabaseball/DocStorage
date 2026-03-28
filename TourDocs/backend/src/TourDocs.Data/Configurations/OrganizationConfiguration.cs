using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.BusinessRegNo)
            .HasMaxLength(100);

        builder.Property(o => o.LogoUrl)
            .HasMaxLength(500);

        builder.Property(o => o.Address)
            .HasMaxLength(500);

        builder.Property(o => o.Phone)
            .HasMaxLength(50);

        builder.Property(o => o.Email)
            .HasMaxLength(200);

        builder.Property(o => o.Website)
            .HasMaxLength(300);

        builder.Property(o => o.Industry)
            .HasMaxLength(100);

        builder.Property(o => o.SubscriptionPlan)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.CreatedAt)
            .HasColumnType("datetime2");

        builder.Property(o => o.UpdatedAt)
            .HasColumnType("datetime2");

        builder.Property(o => o.Language)
            .HasMaxLength(10)
            .HasDefaultValue("en");

        builder.Property(o => o.Timezone)
            .HasMaxLength(100)
            .HasDefaultValue("UTC");

        builder.HasIndex(o => o.Slug)
            .IsUnique();

        builder.HasIndex(o => o.Email);
    }
}

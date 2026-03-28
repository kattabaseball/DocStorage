using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.LegalFirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.LegalLastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.DateOfBirth)
            .HasColumnType("datetime2");

        builder.Property(m => m.Nationality)
            .HasMaxLength(100);

        builder.Property(m => m.NicNumber)
            .HasMaxLength(50);

        builder.Property(m => m.PassportNumber)
            .HasMaxLength(50);

        builder.Property(m => m.Phone)
            .HasMaxLength(50);

        builder.Property(m => m.Email)
            .HasMaxLength(200);

        builder.Property(m => m.Address)
            .HasMaxLength(500);

        builder.Property(m => m.Title)
            .HasMaxLength(100);

        builder.Property(m => m.Department)
            .HasMaxLength(100);

        builder.Property(m => m.Specialization)
            .HasMaxLength(200);

        builder.Property(m => m.ExternalId)
            .HasMaxLength(100);

        builder.Property(m => m.CustomFields)
            .HasColumnType("nvarchar(max)");

        builder.Property(m => m.ProfilePhotoUrl)
            .HasMaxLength(500);

        builder.Property(m => m.Notes)
            .HasMaxLength(2000);

        builder.Property(m => m.DeletedAt)
            .HasColumnType("datetime2");

        builder.HasOne(m => m.Organization)
            .WithMany(o => o.Members)
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => m.OrganizationId);
        builder.HasIndex(m => m.Email);
        builder.HasIndex(m => m.PassportNumber);
        builder.HasIndex(m => new { m.OrganizationId, m.LegalFirstName, m.LegalLastName });
    }
}

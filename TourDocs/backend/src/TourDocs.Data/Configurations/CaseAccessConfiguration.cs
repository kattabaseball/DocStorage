using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class CaseAccessConfiguration : IEntityTypeConfiguration<CaseAccess>
{
    public void Configure(EntityTypeBuilder<CaseAccess> builder)
    {
        builder.ToTable("CaseAccesses");

        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.Role)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ca => ca.Permission)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ca => ca.GrantedAt)
            .HasColumnType("datetime2");

        builder.Property(ca => ca.ExpiresAt)
            .HasColumnType("datetime2");

        builder.Property(ca => ca.RevokedAt)
            .HasColumnType("datetime2");

        builder.HasOne(ca => ca.Case)
            .WithMany(c => c.CaseAccesses)
            .HasForeignKey(ca => ca.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ca => ca.User)
            .WithMany()
            .HasForeignKey(ca => ca.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ca => ca.GrantedByUser)
            .WithMany()
            .HasForeignKey(ca => ca.GrantedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ca => new { ca.CaseId, ca.UserId });
        builder.HasIndex(ca => ca.UserId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ToTable("OrganizationMembers");

        builder.HasKey(om => om.Id);

        builder.Property(om => om.Role)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(om => om.InvitedAt)
            .HasColumnType("datetime2");

        builder.Property(om => om.JoinedAt)
            .HasColumnType("datetime2");

        builder.HasOne(om => om.Organization)
            .WithMany(o => o.OrganizationMembers)
            .HasForeignKey(om => om.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(om => om.User)
            .WithMany(u => u.OrganizationMemberships)
            .HasForeignKey(om => om.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(om => new { om.OrganizationId, om.UserId })
            .IsUnique();
    }
}

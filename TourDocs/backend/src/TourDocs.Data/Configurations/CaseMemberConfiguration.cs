using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class CaseMemberConfiguration : IEntityTypeConfiguration<CaseMember>
{
    public void Configure(EntityTypeBuilder<CaseMember> builder)
    {
        builder.ToTable("CaseMembers");

        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.Status)
            .HasMaxLength(50);

        builder.Property(cm => cm.Notes)
            .HasMaxLength(1000);

        builder.Property(cm => cm.AddedAt)
            .HasColumnType("datetime2");

        builder.HasOne(cm => cm.Case)
            .WithMany(c => c.CaseMembers)
            .HasForeignKey(cm => cm.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.Member)
            .WithMany(m => m.CaseMembers)
            .HasForeignKey(cm => cm.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(cm => new { cm.CaseId, cm.MemberId })
            .IsUnique();
    }
}

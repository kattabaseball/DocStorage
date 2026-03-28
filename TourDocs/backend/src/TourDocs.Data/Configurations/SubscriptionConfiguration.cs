using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Plan)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.PaymentGatewayCustomerId)
            .HasMaxLength(200);

        builder.Property(s => s.PaymentGatewaySubscriptionId)
            .HasMaxLength(200);

        builder.Property(s => s.CurrentPeriodStart)
            .HasColumnType("datetime2");

        builder.Property(s => s.CurrentPeriodEnd)
            .HasColumnType("datetime2");

        builder.HasOne(s => s.Organization)
            .WithOne(o => o.Subscription)
            .HasForeignKey<Subscription>(s => s.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.OrganizationId)
            .IsUnique();
    }
}

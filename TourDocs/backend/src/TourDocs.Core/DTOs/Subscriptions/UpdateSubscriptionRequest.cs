using System.ComponentModel.DataAnnotations;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Subscriptions;

/// <summary>
/// Request DTO for updating an organization's subscription plan.
/// </summary>
public class UpdateSubscriptionRequest
{
    [Required]
    public SubscriptionPlan Plan { get; set; }
}

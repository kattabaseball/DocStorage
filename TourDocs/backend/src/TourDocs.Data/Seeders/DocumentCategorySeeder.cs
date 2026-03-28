using TourDocs.Data.Context;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Seeders;

/// <summary>
/// Provides static reference data for document categories and their associated document types.
/// This is an in-memory reference catalog — no separate database table is needed.
/// The SeedAsync method is a no-op placeholder to keep the seeder contract consistent.
/// </summary>
public static class DocumentCategorySeeder
{
    /// <summary>
    /// Document types organized by category. Each category maps to an array of
    /// standard document type names recognized throughout the system.
    /// </summary>
    public static readonly IReadOnlyDictionary<DocumentCategory, string[]> CategoryDocumentTypes =
        new Dictionary<DocumentCategory, string[]>
        {
            {
                DocumentCategory.Identity, new[]
                {
                    "Passport",
                    "NIC",
                    "Birth Certificate",
                    "National ID Card"
                }
            },
            {
                DocumentCategory.Financial, new[]
                {
                    "Bank Statement",
                    "Tax Return",
                    "Salary Letter",
                    "Income Certificate",
                    "Financial Guarantee"
                }
            },
            {
                DocumentCategory.Legal, new[]
                {
                    "Police Clearance",
                    "Court Clearance",
                    "Character Certificate",
                    "Power of Attorney"
                }
            },
            {
                DocumentCategory.Professional, new[]
                {
                    "Employment Letter",
                    "Invitation Letter",
                    "Contract",
                    "Business Registration",
                    "Professional License"
                }
            },
            {
                DocumentCategory.Travel, new[]
                {
                    "Previous Visa Copy",
                    "Travel Insurance",
                    "Flight Booking",
                    "Hotel Reservation",
                    "Travel Itinerary"
                }
            },
            {
                DocumentCategory.Medical, new[]
                {
                    "Medical Certificate",
                    "Vaccination Record",
                    "Health Insurance",
                    "Fitness Certificate"
                }
            },
            {
                DocumentCategory.Photos, new[]
                {
                    "Passport Photo (35x45mm)",
                    "Passport Photo (51x51mm)",
                    "Digital Photo"
                }
            }
        };

    /// <summary>
    /// Returns all known document type names across all categories.
    /// </summary>
    public static IReadOnlyList<string> AllDocumentTypes =>
        CategoryDocumentTypes.Values.SelectMany(v => v).Distinct().ToList();

    /// <summary>
    /// Returns the category for a given document type name, or null if not found.
    /// </summary>
    public static DocumentCategory? GetCategoryForType(string documentType)
    {
        foreach (var (category, types) in CategoryDocumentTypes)
        {
            if (types.Contains(documentType, StringComparer.OrdinalIgnoreCase))
            {
                return category;
            }
        }

        return null;
    }

    /// <summary>
    /// Placeholder to satisfy the master seeder contract. Reference data is static
    /// and does not require database persistence.
    /// </summary>
    public static Task SeedAsync(ApplicationDbContext context)
    {
        // Reference data is maintained in-memory via the static dictionaries above.
        // No database operations required.
        return Task.CompletedTask;
    }
}

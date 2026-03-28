using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Seeders;

/// <summary>
/// Seeds comprehensive visa checklists for major destination countries.
/// Each checklist contains detailed document requirements with format notes,
/// validity periods, and original/copy requirements.
/// </summary>
public static class CountryChecklistSeeder
{
    // Deterministic GUIDs for checklists so foreign keys work consistently
    public static readonly Guid SchengenChecklistId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
    public static readonly Guid UkChecklistId = Guid.Parse("a1000000-0000-0000-0000-000000000002");
    public static readonly Guid UsChecklistId = Guid.Parse("a1000000-0000-0000-0000-000000000003");
    public static readonly Guid CanadaChecklistId = Guid.Parse("a1000000-0000-0000-0000-000000000004");
    public static readonly Guid AustraliaChecklistId = Guid.Parse("a1000000-0000-0000-0000-000000000005");
    public static readonly Guid JapanChecklistId = Guid.Parse("a1000000-0000-0000-0000-000000000006");
    public static readonly Guid SouthKoreaChecklistId = Guid.Parse("a1000000-0000-0000-0000-000000000007");
    public static readonly Guid UaeChecklistId = Guid.Parse("a1000000-0000-0000-0000-000000000008");

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Checklists.AnyAsync(c => c.IsSystem))
        {
            return; // Already seeded
        }

        var checklists = new List<Checklist>();
        var allItems = new List<ChecklistItem>();

        // --- Schengen Visa Checklist ---
        var schengen = CreateChecklist(SchengenChecklistId, "SCHENGEN", "Schengen Area",
            "Schengen Short-Stay Visa (Type C)", "tourist_visa",
            "Applies to all 27 Schengen member states. Processing typically takes 15 calendar days.");
        checklists.Add(schengen);
        allItems.AddRange(CreateSchengenItems(SchengenChecklistId));

        // --- United Kingdom Checklist ---
        var uk = CreateChecklist(UkChecklistId, "GB", "United Kingdom",
            "UK Standard Visitor Visa", "tourist_visa",
            "Standard Visitor visa for tourism, business, or study up to 6 months. Apply online via GOV.UK.");
        checklists.Add(uk);
        allItems.AddRange(CreateUkItems(UkChecklistId));

        // --- United States Checklist ---
        var us = CreateChecklist(UsChecklistId, "US", "United States",
            "US B1/B2 Visitor Visa", "tourist_visa",
            "B1 (Business) / B2 (Tourism) nonimmigrant visa. Requires DS-160 and consular interview.");
        checklists.Add(us);
        allItems.AddRange(CreateUsItems(UsChecklistId));

        // --- Canada Checklist ---
        var canada = CreateChecklist(CanadaChecklistId, "CA", "Canada",
            "Canada Temporary Resident Visa (TRV)", "tourist_visa",
            "Temporary Resident Visa for tourism, business visits, or family visits. Apply online via IRCC.");
        checklists.Add(canada);
        allItems.AddRange(CreateCanadaItems(CanadaChecklistId));

        // --- Australia Checklist ---
        var australia = CreateChecklist(AustraliaChecklistId, "AU", "Australia",
            "Australia Visitor Visa (Subclass 600)", "tourist_visa",
            "Visitor visa for tourism or business visitor stream. Apply via ImmiAccount online portal.");
        checklists.Add(australia);
        allItems.AddRange(CreateAustraliaItems(AustraliaChecklistId));

        // --- Japan Checklist ---
        var japan = CreateChecklist(JapanChecklistId, "JP", "Japan",
            "Japan Short-Term Stay Visa", "tourist_visa",
            "Single/multiple entry visa for tourism, business, or cultural activities. Apply via designated travel agency or embassy.");
        checklists.Add(japan);
        allItems.AddRange(CreateJapanItems(JapanChecklistId));

        // --- South Korea Checklist ---
        var southKorea = CreateChecklist(SouthKoreaChecklistId, "KR", "South Korea",
            "South Korea Short-Term Visit Visa (C-3)", "tourist_visa",
            "C-3 visa for short-term visits including tourism, business, and cultural activities up to 90 days.");
        checklists.Add(southKorea);
        allItems.AddRange(CreateSouthKoreaItems(SouthKoreaChecklistId));

        // --- UAE Checklist ---
        var uae = CreateChecklist(UaeChecklistId, "AE", "United Arab Emirates",
            "UAE Tourist / Visit Visa", "tourist_visa",
            "Tourist or visit visa sponsored by airline, hotel, or UAE-based sponsor. 30 or 90 day duration.");
        checklists.Add(uae);
        allItems.AddRange(CreateUaeItems(UaeChecklistId));

        await context.Checklists.AddRangeAsync(checklists);
        await context.ChecklistItems.AddRangeAsync(allItems);
        await context.SaveChangesAsync();
    }

    private static Checklist CreateChecklist(Guid id, string countryCode, string countryName,
        string name, string checklistType, string notes)
    {
        return new Checklist
        {
            Id = id,
            CountryCode = countryCode,
            CountryName = countryName,
            Name = name,
            ChecklistType = checklistType,
            Version = 1,
            IsSystem = true,
            OrganizationId = null,
            Notes = notes,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
    }

    private static ChecklistItem Item(Guid checklistId, int sort, string docType, DocumentCategory category,
        string description, string? formatNotes, bool required, bool original, int? validityDays)
    {
        return new ChecklistItem
        {
            Id = Guid.NewGuid(),
            ChecklistId = checklistId,
            DocumentType = docType,
            DocumentCategory = category,
            Description = description,
            FormatNotes = formatNotes,
            IsRequired = required,
            RequiresOriginal = original,
            ValidityDays = validityDays,
            SortOrder = sort,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
    }

    // ===================================================================
    // SCHENGEN
    // ===================================================================
    private static List<ChecklistItem> CreateSchengenItems(Guid cId)
    {
        var i = 0;
        return new List<ChecklistItem>
        {
            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Valid passport with at least 6 months validity beyond intended stay and minimum 2 blank pages.",
                "Must not be older than 10 years. Passport must be issued within the last 10 years on the date of departure from the Schengen area.",
                true, true, 180),

            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Photocopy of passport bio-data page and all previously stamped pages.",
                "A4-size clear color copies of all relevant pages.",
                true, false, null),

            Item(cId, ++i, "Passport Photo (35x45mm)", DocumentCategory.Photos,
                "Two recent passport-size photographs taken within the last 6 months.",
                "35mm x 45mm, white background, 70-80% face coverage, no glasses, neutral expression. ICAO compliant.",
                true, true, 180),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Completed and signed Schengen visa application form.",
                "Download from the consulate website of the main destination country. Must be signed by the applicant in two places.",
                true, true, null),

            Item(cId, ++i, "Travel Insurance", DocumentCategory.Travel,
                "Travel medical insurance valid for all Schengen states with minimum EUR 30,000 coverage.",
                "Must cover emergency medical expenses, hospitalization, and repatriation. Valid for the entire duration of stay plus 15-day buffer.",
                true, false, null),

            Item(cId, ++i, "Flight Booking", DocumentCategory.Travel,
                "Round-trip flight reservation showing entry and exit from the Schengen area.",
                "Confirmed booking with PNR number. Do NOT purchase non-refundable tickets before visa approval.",
                true, false, null),

            Item(cId, ++i, "Hotel Reservation", DocumentCategory.Travel,
                "Proof of accommodation for the entire duration of the stay.",
                "Hotel booking confirmations with applicant's name, dates, and hotel address. If staying with a host, provide host's invitation letter with proof of legal residence.",
                true, false, null),

            Item(cId, ++i, "Bank Statement", DocumentCategory.Financial,
                "Bank statements for the last 3 months showing sufficient funds.",
                "Official bank statements with bank stamp/seal. Minimum balance equivalent to EUR 50-100 per day of stay. Must show regular income and healthy transaction history.",
                true, true, 90),

            Item(cId, ++i, "Employment Letter", DocumentCategory.Professional,
                "Employment letter from current employer confirming position, salary, and approved leave.",
                "On company letterhead with company registration number, signatory's name and designation, HR contact details. Must state applicant's position, monthly salary, employment start date, and approved leave dates.",
                true, true, 30),

            Item(cId, ++i, "Tax Return", DocumentCategory.Financial,
                "Income tax returns for the last financial year.",
                "Certified copy from the tax authority or signed by a chartered accountant.",
                true, false, 365),

            Item(cId, ++i, "Police Clearance", DocumentCategory.Legal,
                "Police clearance certificate issued within the last 6 months.",
                "Must be apostilled or authenticated by the Ministry of Foreign Affairs. Some consulates accept original plus one copy.",
                true, true, 180),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Cover letter explaining the purpose, itinerary, and duration of travel.",
                "Addressed to the consulate/embassy. Include personal details, travel dates, purpose, accommodation details, funding source, and ties to home country.",
                true, true, null),

            Item(cId, ++i, "Invitation Letter", DocumentCategory.Professional,
                "Invitation letter from host in the Schengen country (if applicable).",
                "Must include host's full name, address, contact details, relationship to applicant, and proof of legal residence. Some countries require notarized invitation.",
                false, false, 90),

            Item(cId, ++i, "Travel Itinerary", DocumentCategory.Travel,
                "Detailed day-by-day travel itinerary.",
                "Include cities to visit, transport between cities, planned activities, and event/performance dates if applicable.",
                true, false, null),

            Item(cId, ++i, "Previous Visa Copy", DocumentCategory.Travel,
                "Copies of previous Schengen and other international visas.",
                "Color copies of all previous visa stickers and entry/exit stamps from the last 3 years.",
                false, false, null),
        };
    }

    // ===================================================================
    // UNITED KINGDOM
    // ===================================================================
    private static List<ChecklistItem> CreateUkItems(Guid cId)
    {
        var i = 0;
        return new List<ChecklistItem>
        {
            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Valid passport with at least 6 months validity and one blank page for the visa vignette.",
                "Include all expired passports showing previous travel history.",
                true, true, 180),

            Item(cId, ++i, "Passport Photo (35x45mm)", DocumentCategory.Photos,
                "Two passport-size photographs meeting UK visa photo requirements.",
                "45mm x 35mm, light grey or cream background, no head coverings unless for religious reasons. Taken within the last 6 months.",
                true, true, 180),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Completed online visa application (VAF) form — printed confirmation page.",
                "Apply online at GOV.UK. Print the completed form, the appointment confirmation, and the GWF reference number page.",
                true, true, null),

            Item(cId, ++i, "Bank Statement", DocumentCategory.Financial,
                "Bank statements for the last 6 months showing maintenance funds of at least GBP 1,890 held for 28 consecutive days.",
                "Official bank statements or bank letter on letterhead. Must show account holder name, account number, and each transaction. Closing balance must demonstrate GBP 1,890 minimum for the 28 days ending no more than 31 days before application date.",
                true, true, 31),

            Item(cId, ++i, "Employment Letter", DocumentCategory.Professional,
                "Employment letter from current employer on company letterhead.",
                "Must state: job title, annual salary, length of employment, approved leave dates, and confirmation that the position will be held. Include employer's contact details and company registration number.",
                true, true, 30),

            Item(cId, ++i, "Tax Return", DocumentCategory.Financial,
                "Income tax return documents for the last 2 financial years.",
                "Tax assessment notice or certified tax return. Demonstrates regular income and tax compliance.",
                true, false, 365),

            Item(cId, ++i, "Medical Certificate", DocumentCategory.Medical,
                "Tuberculosis (TB) test certificate from an approved clinic.",
                "Required for applicants from TB-listed countries including Sri Lanka, India, Pakistan, and Bangladesh. Must be from a UKVI-approved clinic. Valid for 6 months from the test date.",
                true, true, 180),

            Item(cId, ++i, "Hotel Reservation", DocumentCategory.Travel,
                "Accommodation details for the entire stay in the UK.",
                "Hotel booking confirmations OR sponsor's letter with proof of address (utility bill or council tax statement). If staying with a sponsor, include their passport/BRP copy.",
                true, false, null),

            Item(cId, ++i, "Flight Booking", DocumentCategory.Travel,
                "Flight itinerary showing entry and departure from the UK.",
                "Booking confirmation with dates. Refundable booking recommended until visa is issued.",
                true, false, null),

            Item(cId, ++i, "Invitation Letter", DocumentCategory.Professional,
                "Sponsor/invitation letter from UK-based host (if applicable).",
                "Must include: sponsor's full name, date of birth, passport/BRP number, address, immigration status in the UK, relationship to applicant, details of financial support offered, and a declaration of accommodation.",
                false, false, 90),

            Item(cId, ++i, "Travel Itinerary", DocumentCategory.Travel,
                "Detailed travel plan showing purpose and activities in the UK.",
                "Day-by-day plan including event schedules, meeting details, or tour dates. Clearly state the main purpose of the visit.",
                true, false, null),

            Item(cId, ++i, "Travel Insurance", DocumentCategory.Travel,
                "Comprehensive travel and medical insurance for the UK stay.",
                "Not mandatory but strongly recommended. Cover medical expenses, trip cancellation, and personal belongings.",
                false, false, null),

            Item(cId, ++i, "Previous Visa Copy", DocumentCategory.Travel,
                "Copies of all previous UK visas and stamps, plus visas for other countries.",
                "Demonstrates travel history and compliance with previous visa conditions.",
                false, false, null),

            Item(cId, ++i, "Business Registration", DocumentCategory.Professional,
                "Business registration documents (if self-employed).",
                "Certificate of registration, articles of association, audited financial statements for the last 2 years, and proof of business income.",
                false, true, 365),

            Item(cId, ++i, "Police Clearance", DocumentCategory.Legal,
                "Police clearance certificate (may be requested during processing).",
                "Not always required for Standard Visitor visa, but useful to include proactively. Must be recent (within 6 months).",
                false, true, 180),
        };
    }

    // ===================================================================
    // UNITED STATES
    // ===================================================================
    private static List<ChecklistItem> CreateUsItems(Guid cId)
    {
        var i = 0;
        return new List<ChecklistItem>
        {
            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Valid passport with at least 6 months validity beyond the period of intended stay.",
                "If you have had previous passports, bring those as well to demonstrate travel history.",
                true, true, 180),

            Item(cId, ++i, "Passport Photo (51x51mm)", DocumentCategory.Photos,
                "One recent photograph meeting US visa photo requirements.",
                "2 inches x 2 inches (51mm x 51mm), white background, taken within the last 6 months. Photo must also be uploaded digitally during DS-160 completion (600x600 to 1200x1200 pixels, JPEG format, max 240KB).",
                true, true, 180),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "DS-160 Online Nonimmigrant Visa Application confirmation page.",
                "Complete the DS-160 at ceac.state.gov. Print the confirmation page with the barcode. The barcode number (AA followed by 8-10 digits) is required for the interview appointment.",
                true, true, null),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Interview appointment confirmation letter.",
                "Schedule interview via ustraveldocs.com after completing DS-160. Print the confirmation with the appointment date, time, and location.",
                true, true, null),

            Item(cId, ++i, "Bank Statement", DocumentCategory.Financial,
                "Bank statements for the last 6 months demonstrating sufficient financial means.",
                "Official bank statements showing regular deposits, current balance, and transaction history. Include fixed deposits, savings certificates, and investment statements to show strong financial standing.",
                true, true, 30),

            Item(cId, ++i, "Employment Letter", DocumentCategory.Professional,
                "Employment verification letter from current employer.",
                "On company letterhead. State: position, salary, length of employment, nature of business, and approved leave period. Must include HR contact information for verification.",
                true, true, 30),

            Item(cId, ++i, "Tax Return", DocumentCategory.Financial,
                "Income tax returns for the last 3 years.",
                "Demonstrates consistent income and tax compliance. Include W-2 equivalent and annual tax assessment.",
                true, false, 365),

            Item(cId, ++i, "Financial Guarantee", DocumentCategory.Financial,
                "Property ownership documents and asset evidence.",
                "Land deeds, vehicle registrations, company shares, fixed deposit certificates. Demonstrates strong ties to home country and financial stability.",
                false, false, null),

            Item(cId, ++i, "Invitation Letter", DocumentCategory.Professional,
                "Invitation letter from US-based host or organization (if applicable).",
                "Include: host's full name, US address, immigration status, relationship to applicant, purpose and duration of visit, and who bears travel costs. If attending events, include event registration.",
                false, false, 90),

            Item(cId, ++i, "Travel Itinerary", DocumentCategory.Travel,
                "Intended travel plan including destinations and duration.",
                "Outline planned activities, cities to visit, and event schedules. The consular officer will assess whether the plan is consistent with B1/B2 visa purposes.",
                true, false, null),

            Item(cId, ++i, "Previous Visa Copy", DocumentCategory.Travel,
                "Previous US visa stickers and I-94 records (if any prior US travel).",
                "Demonstrates compliance with previous US visa terms. Include copies from expired passports if applicable.",
                false, false, null),

            Item(cId, ++i, "Contract", DocumentCategory.Professional,
                "Evidence of ties to home country — family, employment, property, business.",
                "Marriage certificate, children's birth certificates, property deeds, business registration. Critical for demonstrating non-immigrant intent.",
                true, false, null),

            Item(cId, ++i, "Hotel Reservation", DocumentCategory.Travel,
                "Accommodation arrangements in the United States.",
                "Hotel reservations or letter from US-based host confirming accommodation. Do not purchase non-refundable arrangements before visa approval.",
                false, false, null),

            Item(cId, ++i, "Travel Insurance", DocumentCategory.Travel,
                "Travel and medical insurance for the US visit.",
                "Not mandatory but recommended. US healthcare is expensive; insurance should cover at least USD 100,000 in medical expenses.",
                false, false, null),
        };
    }

    // ===================================================================
    // CANADA
    // ===================================================================
    private static List<ChecklistItem> CreateCanadaItems(Guid cId)
    {
        var i = 0;
        return new List<ChecklistItem>
        {
            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Valid passport that does not expire for the duration of your expected stay.",
                "If your passport will expire within 6 months of your planned arrival date, consider renewing it before applying. Include all previous passports.",
                true, true, 180),

            Item(cId, ++i, "Passport Photo (35x45mm)", DocumentCategory.Photos,
                "Two recent passport photographs meeting IRCC specifications.",
                "35mm x 45mm, taken within the last 6 months, white or light-coloured background, neutral expression, eyes open and clearly visible, no eyeglasses. Digital version also required for online application (minimum 420x540 pixels, JPEG/PNG, max 4MB).",
                true, true, 180),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Completed IMM 5257 Application for Temporary Resident Visa.",
                "Complete online through IRCC portal or download PDF form. All sections must be filled; use 'N/A' where not applicable. Sign and date the form.",
                true, true, null),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Family Information Form (IMM 5645).",
                "List all family members including those not travelling. Required for applicants from certain countries.",
                true, true, null),

            Item(cId, ++i, "Bank Statement", DocumentCategory.Financial,
                "Proof of financial support — bank statements for the last 4 months.",
                "Minimum funds of CAD 10,000 or equivalent. Include savings accounts, term deposits, and investment accounts. A bank letter confirming average balance is recommended alongside statements.",
                true, true, 120),

            Item(cId, ++i, "Employment Letter", DocumentCategory.Professional,
                "Proof of current employment or business ownership.",
                "Employment letter on letterhead stating: position, salary, years of service, and approved leave. For self-employed: business registration, tax filings, and revenue proof.",
                true, true, 30),

            Item(cId, ++i, "Tax Return", DocumentCategory.Financial,
                "Notice of tax assessment or income tax returns for the last 2 years.",
                "Government-issued tax assessment notices or accountant-certified returns showing declared income.",
                true, false, 365),

            Item(cId, ++i, "Travel Itinerary", DocumentCategory.Travel,
                "Purpose of travel document with detailed itinerary.",
                "Written statement of purpose. Include: reason for visiting Canada, intended activities, planned cities, travel dates, and how the trip will be funded.",
                true, false, null),

            Item(cId, ++i, "Invitation Letter", DocumentCategory.Professional,
                "Letter of invitation from Canadian host (if applicable).",
                "Include host's full name, date of birth, address, phone number, citizenship/PR status, and declaration of financial responsibility if hosting. Include a copy of host's PR card or citizenship certificate.",
                false, false, 90),

            Item(cId, ++i, "Flight Booking", DocumentCategory.Travel,
                "Flight itinerary or booking confirmation.",
                "Round-trip reservation preferred. Refundable booking recommended until visa is approved.",
                true, false, null),

            Item(cId, ++i, "Hotel Reservation", DocumentCategory.Travel,
                "Proof of accommodation in Canada.",
                "Hotel confirmations or host's letter with proof of address. Must cover the full duration of the planned stay.",
                true, false, null),

            Item(cId, ++i, "Travel Insurance", DocumentCategory.Travel,
                "Travel and medical insurance for Canada.",
                "Canada does not provide free healthcare to visitors. Insurance should cover at least CAD 100,000 in emergency medical expenses.",
                false, false, null),

            Item(cId, ++i, "Previous Visa Copy", DocumentCategory.Travel,
                "Travel history — copies of previous visas and passports.",
                "All previous Canadian visas, US visas, Schengen visas, and other travel stamps. Demonstrates compliance and travel experience.",
                false, false, null),

            Item(cId, ++i, "Police Clearance", DocumentCategory.Legal,
                "Police clearance certificate for applicants from certain countries.",
                "Not always required for visitor visa, but may be requested. Must be recent (within 6 months) and apostilled.",
                false, true, 180),
        };
    }

    // ===================================================================
    // AUSTRALIA
    // ===================================================================
    private static List<ChecklistItem> CreateAustraliaItems(Guid cId)
    {
        var i = 0;
        return new List<ChecklistItem>
        {
            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Current valid passport.",
                "Must be valid for the duration of the intended stay. Provide certified copies of the bio-data page.",
                true, true, 180),

            Item(cId, ++i, "Digital Photo", DocumentCategory.Photos,
                "Recent digital passport photograph for online application.",
                "45mm x 35mm or digital equivalent (min 600x500 pixels). Plain light-coloured background, neutral expression, taken within the last 6 months.",
                true, false, 180),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Online visa application lodged through ImmiAccount — confirmation and Transaction Reference Number (TRN).",
                "Create an ImmiAccount at online.immi.gov.au. Complete Form 1419 online for the Visitor visa (subclass 600). Save the TRN for tracking.",
                true, false, null),

            Item(cId, ++i, "Bank Statement", DocumentCategory.Financial,
                "Evidence of financial capacity — bank statements for the last 3 months.",
                "Minimum funds of AUD 5,000 or equivalent per applicant for a short stay. Include all bank accounts, fixed deposits, and investment evidence. A sponsorship declaration (Form 1149) can supplement if an Australian sponsor provides financial support.",
                true, true, 90),

            Item(cId, ++i, "Employment Letter", DocumentCategory.Professional,
                "Evidence of current employment or economic ties.",
                "Employment letter on letterhead. For self-employed: business registration, ABN equivalent, financial statements. For students: enrolment letter from educational institution.",
                true, true, 30),

            Item(cId, ++i, "Medical Certificate", DocumentCategory.Medical,
                "Health examination by a Bupa Medical Visa Services (BVMS) panel physician.",
                "Required for stays longer than 3 months or for applicants from high-risk countries. Book via Bupa's HAP (Health Assessment Portal) using the HAP ID generated in ImmiAccount. Results sent directly to the department.",
                false, true, 365),

            Item(cId, ++i, "Police Clearance", DocumentCategory.Legal,
                "Character evidence — police clearance certificate from each country lived in for 12+ months in the last 10 years.",
                "National-level police check, apostilled or authenticated. Required for stays longer than 3 months and for applicants aged 16 and above.",
                false, true, 180),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Genuine Temporary Entrant (GTE) statement.",
                "Written statement explaining: reason for visiting Australia, ties to home country, purpose of travel, intended duration, and why you will return home. Critical for visa approval.",
                true, true, null),

            Item(cId, ++i, "Travel Insurance", DocumentCategory.Travel,
                "Overseas Visitor Health Cover (OVHC) or travel insurance.",
                "Not mandatory for Visitor visa but strongly recommended. Australia does not provide free healthcare to visitors unless covered by a reciprocal health care agreement.",
                false, false, null),

            Item(cId, ++i, "Flight Booking", DocumentCategory.Travel,
                "Flight booking or itinerary showing round-trip travel.",
                "Confirmed booking preferred. Must show entry and departure from Australia.",
                true, false, null),

            Item(cId, ++i, "Hotel Reservation", DocumentCategory.Travel,
                "Accommodation details for the duration of stay.",
                "Hotel bookings, Airbnb confirmations, or invitation from Australian-based host with proof of address and citizenship/visa status.",
                true, false, null),

            Item(cId, ++i, "Invitation Letter", DocumentCategory.Professional,
                "Invitation letter from Australian host or organization (if applicable).",
                "Include host's full name, address, date of birth, Australian citizenship/PR details, and purpose of the visit. For business: company details and nature of business activities.",
                false, false, 90),

            Item(cId, ++i, "Previous Visa Copy", DocumentCategory.Travel,
                "Travel history documentation.",
                "Copies of all previous Australian visas, US/UK/Schengen visas. Demonstrates travel pattern and compliance.",
                false, false, null),

            Item(cId, ++i, "Tax Return", DocumentCategory.Financial,
                "Tax returns for the last 2 years.",
                "Demonstrates consistent income and economic establishment in home country.",
                false, false, 365),
        };
    }

    // ===================================================================
    // JAPAN
    // ===================================================================
    private static List<ChecklistItem> CreateJapanItems(Guid cId)
    {
        var i = 0;
        return new List<ChecklistItem>
        {
            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Valid passport with at least 6 months validity and 2 blank visa pages.",
                "Must have been issued within the last 10 years.",
                true, true, 180),

            Item(cId, ++i, "Passport Photo (35x45mm)", DocumentCategory.Photos,
                "One recent passport-size photograph.",
                "45mm x 35mm, white background, taken within the last 6 months. Head must be centered and face must occupy 70-80% of the photo. No hats, no tinted glasses.",
                true, true, 180),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Completed visa application form (available from the embassy/consulate).",
                "Download from the Japanese embassy website. Complete in English or Japanese. Must be signed by the applicant.",
                true, true, null),

            Item(cId, ++i, "Travel Itinerary", DocumentCategory.Travel,
                "Detailed schedule of stay in Japan (day-by-day itinerary).",
                "Include: dates, cities, hotels, activities, transport between cities, performance/event venues and dates. Japanese immigration requires specific itineraries.",
                true, true, null),

            Item(cId, ++i, "Flight Booking", DocumentCategory.Travel,
                "Round-trip flight reservation.",
                "Confirmed booking showing entry to and departure from Japan.",
                true, false, null),

            Item(cId, ++i, "Hotel Reservation", DocumentCategory.Travel,
                "Hotel reservations for the entire stay.",
                "Include hotel name, address, phone number, and confirmation number. Must cover every night of the stay.",
                true, false, null),

            Item(cId, ++i, "Invitation Letter", DocumentCategory.Professional,
                "Invitation letter from the Japanese host organization or guarantor.",
                "Must be on the organization's letterhead. Include: organization name, address, representative's name and title, purpose of invitation, relationship to applicant, and details of financial support. A Certificate of Eligibility is NOT required for short-term stays but may expedite processing.",
                true, true, 90),

            Item(cId, ++i, "Employment Letter", DocumentCategory.Professional,
                "Guarantor information — letter from Japanese guarantor or self-guarantee with financial proof.",
                "If the Japanese host serves as guarantor: guarantor's tax certificate (Kazei Shomeisho) and residence certificate (Juminhyo). If self-guaranteeing: provide comprehensive financial evidence.",
                true, true, 30),

            Item(cId, ++i, "Bank Statement", DocumentCategory.Financial,
                "Bank statements for the last 3 months showing sufficient funds.",
                "Must demonstrate ability to cover all expenses during stay in Japan. Average daily cost in Japan is approximately JPY 15,000-20,000 (USD 100-135).",
                true, true, 90),

            Item(cId, ++i, "Tax Return", DocumentCategory.Financial,
                "Tax payment certificate or income tax return.",
                "Applicant's own tax certificate from the government. If guarantor is in Japan, the guarantor's tax certificate (Nozei Shomeisho) is needed.",
                true, false, 365),

            Item(cId, ++i, "Contract", DocumentCategory.Professional,
                "Proof of professional purpose — performance contract, event agreement, or business meeting details.",
                "For performers: contract with event organizer, performance schedule. For business: meeting agenda, company letter. Must demonstrate the specific purpose of the visit.",
                false, true, null),

            Item(cId, ++i, "Previous Visa Copy", DocumentCategory.Travel,
                "Copies of previous visas (all countries).",
                "Japan considers travel history favorably. Include all Japan visas and stamps from previous travels.",
                false, false, null),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Certificate of Employment or enrollment from home country.",
                "Confirms current status (employed, student, or self-employed). Helps establish ties to home country.",
                true, true, 30),
        };
    }

    // ===================================================================
    // SOUTH KOREA
    // ===================================================================
    private static List<ChecklistItem> CreateSouthKoreaItems(Guid cId)
    {
        var i = 0;
        return new List<ChecklistItem>
        {
            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Valid passport with at least 6 months validity beyond the planned stay.",
                "Must have at least 2 blank pages. Include all previous passports.",
                true, true, 180),

            Item(cId, ++i, "Passport Photo (35x45mm)", DocumentCategory.Photos,
                "One passport-size photograph meeting Korean visa requirements.",
                "35mm x 45mm, white background, taken within the last 6 months. Face must occupy 25-35mm of the photo height. No glasses.",
                true, true, 180),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Completed visa application form.",
                "Download from the Korean embassy/consulate website. Complete in English or Korean. All fields mandatory — use 'N/A' where not applicable.",
                true, true, null),

            Item(cId, ++i, "Employment Letter", DocumentCategory.Professional,
                "Employment certificate from current employer.",
                "On company letterhead stating: position, monthly/annual salary, date of joining, and approved leave period. Include company registration certificate.",
                true, true, 30),

            Item(cId, ++i, "Bank Statement", DocumentCategory.Financial,
                "Bank statements for the last 3 months.",
                "Official statements with bank seal. Demonstrate sufficient funds for the trip (minimum equivalent of KRW 3,000,000 or USD 2,500). Include savings and fixed deposits.",
                true, true, 90),

            Item(cId, ++i, "Tax Return", DocumentCategory.Financial,
                "Income tax returns for the last year.",
                "Certified by tax authority or chartered accountant.",
                true, false, 365),

            Item(cId, ++i, "Flight Booking", DocumentCategory.Travel,
                "Confirmed round-trip flight booking.",
                "Must show entry and exit from South Korea with PNR number.",
                true, false, null),

            Item(cId, ++i, "Hotel Reservation", DocumentCategory.Travel,
                "Accommodation booking for the duration of stay.",
                "Hotel name, address, confirmation number, and dates. If staying with a Korean national: invitation letter with host's ID copy.",
                true, false, null),

            Item(cId, ++i, "Travel Itinerary", DocumentCategory.Travel,
                "Travel plan showing purpose and daily activities.",
                "Day-by-day schedule including cities, transport, and planned activities.",
                true, false, null),

            Item(cId, ++i, "Invitation Letter", DocumentCategory.Professional,
                "Invitation letter from Korean organization (for performers, business visitors, or event participants).",
                "On organization letterhead: purpose of invitation, event details, dates, financial support details, and signatory's contact information. Include business registration certificate of the Korean company.",
                false, true, 90),

            Item(cId, ++i, "Contract", DocumentCategory.Professional,
                "Performance or business contract (for performers/artists).",
                "Signed contract between the applicant (or their agency) and the Korean event organizer. Include performance schedule, venues, and compensation details.",
                false, true, null),

            Item(cId, ++i, "Travel Insurance", DocumentCategory.Travel,
                "Travel and medical insurance.",
                "Recommended but not mandatory. Should cover medical expenses of at least USD 30,000.",
                false, false, null),

            Item(cId, ++i, "Previous Visa Copy", DocumentCategory.Travel,
                "Previous Korean visas and stamps from other countries.",
                "Positive travel history (especially previous Korean or OECD country visits) strengthens the application.",
                false, false, null),
        };
    }

    // ===================================================================
    // UNITED ARAB EMIRATES
    // ===================================================================
    private static List<ChecklistItem> CreateUaeItems(Guid cId)
    {
        var i = 0;
        return new List<ChecklistItem>
        {
            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Valid passport with at least 6 months validity.",
                "Clear, undamaged passport. Handwritten passports may not be accepted. Must have at least 2 blank pages.",
                true, true, 180),

            Item(cId, ++i, "Passport", DocumentCategory.Identity,
                "Color scan/copy of the passport bio-data page.",
                "High-resolution color scan (300 DPI minimum). Must clearly show all details, photo, and MRZ lines.",
                true, false, null),

            Item(cId, ++i, "Passport Photo (35x45mm)", DocumentCategory.Photos,
                "Recent passport photograph meeting UAE specifications.",
                "4.3cm x 5.5cm (or digital equivalent), white background, matte finish. No glasses, no head coverings unless for religious reasons. Taken within the last 3 months.",
                true, true, 90),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "Completed UAE visa application form.",
                "Submitted through the sponsor (airline, hotel, travel agency, or UAE-based company/individual). Some nationalities can apply for visa on arrival or e-visa.",
                true, false, null),

            Item(cId, ++i, "Bank Statement", DocumentCategory.Financial,
                "Bank statements for the last 3 months.",
                "Official bank statements showing sufficient funds to cover the stay. No specific minimum but healthy balance and regular transactions expected.",
                true, true, 90),

            Item(cId, ++i, "Employment Letter", DocumentCategory.Professional,
                "Proof of employment or source of income.",
                "Employment letter on company letterhead OR business registration certificate. For students: enrollment letter.",
                true, true, 30),

            Item(cId, ++i, "Invitation Letter", DocumentCategory.Professional,
                "Sponsor letter from UAE-based sponsor (required for sponsored visas).",
                "From the sponsoring entity (company, hotel, or individual). Include: sponsor's name, trade license number, address, phone, and relationship to applicant. For individual sponsors: passport and UAE residence visa copy.",
                true, true, 90),

            Item(cId, ++i, "Hotel Reservation", DocumentCategory.Travel,
                "Confirmed hotel booking for the stay in the UAE.",
                "Booking confirmation from the hotel with dates, applicant's name, and hotel address. For hotel-sponsored visas, the hotel itself processes the visa.",
                true, false, null),

            Item(cId, ++i, "Flight Booking", DocumentCategory.Travel,
                "Confirmed round-trip flight booking.",
                "Airline booking reference (PNR). For airline-sponsored visas (e.g., Emirates, Etihad, flydubai), the airline may process the visa as part of the ticket purchase.",
                true, false, null),

            Item(cId, ++i, "Travel Insurance", DocumentCategory.Travel,
                "Travel and medical insurance for the UAE.",
                "Mandatory for some visa types. Must cover medical emergencies for the duration of the stay.",
                false, false, null),

            Item(cId, ++i, "NIC", DocumentCategory.Identity,
                "OK to Board (OTB) approval — required for some nationalities.",
                "Airlines may require an OK to Board clearance from UAE immigration before allowing boarding. The sponsor must arrange this through GDRFA (General Directorate of Residency and Foreigners Affairs).",
                false, false, null),

            Item(cId, ++i, "Previous Visa Copy", DocumentCategory.Travel,
                "Previous UAE visa copies and stamps from other GCC or international destinations.",
                "Demonstrates travel history. Previous UAE visa compliance is viewed favorably.",
                false, false, null),

            Item(cId, ++i, "Contract", DocumentCategory.Professional,
                "Event contract or performance agreement (for artists/performers).",
                "Signed agreement with the UAE event organizer. Include: event details, dates, venues, and compensation. May be required for obtaining a performance permit from the National Media Council.",
                false, true, null),
        };
    }
}

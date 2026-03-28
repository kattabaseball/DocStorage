using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Seeders;

/// <summary>
/// Seeds realistic demo data for development environments. Creates an organization,
/// users, members, documents, cases, hard copy requests, notifications, and audit logs.
/// This seeder should only be invoked in Development environments.
/// </summary>
public static class DemoDataSeeder
{
    // ═══════════════════════════════════════════════════════════════════
    // Deterministic GUIDs — all foreign keys reference these values
    // ═══════════════════════════════════════════════════════════════════

    // Organization
    private static readonly Guid OrgId = Guid.Parse("b0000000-0000-0000-0000-000000000001");

    // Users (ApplicationUser / IdentityUser)
    private static readonly Guid UserKamalId = Guid.Parse("c0000000-0000-0000-0000-000000000001");
    private static readonly Guid UserNishaniId = Guid.Parse("c0000000-0000-0000-0000-000000000002");
    private static readonly Guid UserSamanId = Guid.Parse("c0000000-0000-0000-0000-000000000003");
    private static readonly Guid UserLankaEventsId = Guid.Parse("c0000000-0000-0000-0000-000000000004");
    private static readonly Guid UserAmalId = Guid.Parse("c0000000-0000-0000-0000-000000000005");

    // Members
    private static readonly Guid MemberAmalId = Guid.Parse("d0000000-0000-0000-0000-000000000001");
    private static readonly Guid MemberChamindaId = Guid.Parse("d0000000-0000-0000-0000-000000000002");
    private static readonly Guid MemberDiliniId = Guid.Parse("d0000000-0000-0000-0000-000000000003");
    private static readonly Guid MemberEshanId = Guid.Parse("d0000000-0000-0000-0000-000000000004");
    private static readonly Guid MemberFathimaId = Guid.Parse("d0000000-0000-0000-0000-000000000005");
    private static readonly Guid MemberGayanId = Guid.Parse("d0000000-0000-0000-0000-000000000006");
    private static readonly Guid MemberHiruniId = Guid.Parse("d0000000-0000-0000-0000-000000000007");
    private static readonly Guid MemberIndikaId = Guid.Parse("d0000000-0000-0000-0000-000000000008");
    private static readonly Guid MemberJanithId = Guid.Parse("d0000000-0000-0000-0000-000000000009");
    private static readonly Guid MemberKumariId = Guid.Parse("d0000000-0000-0000-0000-000000000010");

    // Cases
    private static readonly Guid CaseEuropeId = Guid.Parse("e0000000-0000-0000-0000-000000000001");
    private static readonly Guid CaseDubaiId = Guid.Parse("e0000000-0000-0000-0000-000000000002");
    private static readonly Guid CaseJapanId = Guid.Parse("e0000000-0000-0000-0000-000000000003");

    // Subscription
    private static readonly Guid SubscriptionId = Guid.Parse("f0000000-0000-0000-0000-000000000001");

    // Base timestamp for all demo data
    private static readonly DateTime BaseDate = new(2026, 3, 1, 9, 0, 0, DateTimeKind.Utc);

    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        // Safety: never seed demo data outside the Development environment
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env != "Development")
        {
            return; // Never seed demo data outside development
        }

        if (await context.Organizations.AnyAsync(o => o.Id == OrgId))
        {
            return; // Already seeded
        }

        // 1. Organization
        await SeedOrganizationAsync(context);

        // 2. Users
        await SeedUsersAsync(userManager);

        // 3. Organization memberships
        await SeedOrganizationMembersAsync(context);

        // 4. Members
        await SeedMembersAsync(context);

        // 5. Documents (3-5 per member)
        await SeedDocumentsAsync(context);

        // 6. Cases
        await SeedCasesAsync(context);

        // 7. Case member assignments
        await SeedCaseMembersAsync(context);

        // 8. Case access grants
        await SeedCaseAccessAsync(context);

        // 9. Hard copy requests
        await SeedHardCopyRequestsAsync(context);

        // 10. Document requests
        await SeedDocumentRequestsAsync(context);

        // 11. Notifications
        await SeedNotificationsAsync(context);

        // 12. Audit logs
        await SeedAuditLogsAsync(context);

        // 13. Subscription
        await SeedSubscriptionAsync(context);

        await context.SaveChangesAsync();
    }

    // ───────────────────────────────────────────────────────────────────
    // Organization
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedOrganizationAsync(ApplicationDbContext context)
    {
        var org = new Organization
        {
            Id = OrgId,
            Name = "Rhythm & Routes Entertainment",
            Slug = "rhythm-routes",
            BusinessRegNo = "PV00087456",
            LogoUrl = null,
            Address = "45/2, Galle Road, Colombo 03, Sri Lanka",
            Phone = "+94 11 234 5678",
            Email = "info@rhythmroutes.lk",
            Website = "https://www.rhythmroutes.lk",
            Industry = "entertainment",
            SubscriptionPlan = SubscriptionPlan.Professional,
            IsActive = true,
            CreatedAt = BaseDate.AddDays(-60),
            UpdatedAt = BaseDate
        };

        await context.Organizations.AddAsync(org);
    }

    // ───────────────────────────────────────────────────────────────────
    // Users
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var users = new (Guid Id, string FullName, string Email, string Role)[]
        {
            (UserKamalId, "Kamal Perera", "kamal@rhythmroutes.lk", "OrgOwner"),
            (UserNishaniId, "Nishani Fernando", "nishani@rhythmroutes.lk", "OrgMember"),
            (UserSamanId, "Saman Visa Services", "saman@visaservices.lk", "DocumentHandler"),
            (UserLankaEventsId, "Lanka Events", "events@lankaevents.lk", "CaseManager"),
            (UserAmalId, "Amal Silva", "amal@email.com", "Member"),
        };

        foreach (var (id, fullName, email, role) in users)
        {
            if (await userManager.FindByIdAsync(id.ToString()) != null)
            {
                continue;
            }

            var user = new ApplicationUser
            {
                Id = id,
                FullName = fullName,
                UserName = email,
                NormalizedUserName = email.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = BaseDate.AddDays(-55),
                LastLoginAt = BaseDate.AddDays(-1)
            };

            var result = await userManager.CreateAsync(user, "Demo@123");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create user '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await userManager.AddToRoleAsync(user, role);
        }
    }

    // ───────────────────────────────────────────────────────────────────
    // Organization memberships
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedOrganizationMembersAsync(ApplicationDbContext context)
    {
        var memberships = new[]
        {
            new OrganizationMember
            {
                Id = Guid.NewGuid(), OrganizationId = OrgId, UserId = UserKamalId,
                Role = UserRole.OrgOwner, InvitedAt = BaseDate.AddDays(-55),
                JoinedAt = BaseDate.AddDays(-55), IsActive = true,
                CreatedAt = BaseDate.AddDays(-55), UpdatedAt = BaseDate.AddDays(-55)
            },
            new OrganizationMember
            {
                Id = Guid.NewGuid(), OrganizationId = OrgId, UserId = UserNishaniId,
                Role = UserRole.OrgMember, InvitedAt = BaseDate.AddDays(-50),
                JoinedAt = BaseDate.AddDays(-49), IsActive = true,
                CreatedAt = BaseDate.AddDays(-50), UpdatedAt = BaseDate.AddDays(-49)
            },
            new OrganizationMember
            {
                Id = Guid.NewGuid(), OrganizationId = OrgId, UserId = UserSamanId,
                Role = UserRole.DocumentHandler, InvitedAt = BaseDate.AddDays(-45),
                JoinedAt = BaseDate.AddDays(-44), IsActive = true,
                CreatedAt = BaseDate.AddDays(-45), UpdatedAt = BaseDate.AddDays(-44)
            },
            new OrganizationMember
            {
                Id = Guid.NewGuid(), OrganizationId = OrgId, UserId = UserLankaEventsId,
                Role = UserRole.CaseManager, InvitedAt = BaseDate.AddDays(-40),
                JoinedAt = BaseDate.AddDays(-39), IsActive = true,
                CreatedAt = BaseDate.AddDays(-40), UpdatedAt = BaseDate.AddDays(-39)
            },
            new OrganizationMember
            {
                Id = Guid.NewGuid(), OrganizationId = OrgId, UserId = UserAmalId,
                Role = UserRole.Member, InvitedAt = BaseDate.AddDays(-35),
                JoinedAt = BaseDate.AddDays(-34), IsActive = true,
                CreatedAt = BaseDate.AddDays(-35), UpdatedAt = BaseDate.AddDays(-34)
            },
        };

        await context.OrganizationMembers.AddRangeAsync(memberships);
    }

    // ───────────────────────────────────────────────────────────────────
    // Members
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedMembersAsync(ApplicationDbContext context)
    {
        var members = new[]
        {
            CreateMember(MemberAmalId, UserAmalId, "Amal", "Silva",
                new DateTime(1992, 5, 14), "Sri Lankan", "921340567V", "N5672345",
                "+94 77 123 4567", "amal@email.com",
                "12, Temple Road, Dehiwala, Sri Lanka",
                "The Voice", "Baila", "Vocalist",
                "{\"facebook\":\"fb.com/amalsilva\",\"instagram\":\"@amalsilvamusic\",\"youtube\":\"AmalSilvaOfficial\"}"),

            CreateMember(MemberChamindaId, null, "Chaminda", "Bandara",
                new DateTime(1988, 11, 3), "Sri Lankan", "883070912V", "N4891023",
                "+94 71 234 5678", "chaminda.bandara@gmail.com",
                "78, Kandy Road, Peradeniya, Sri Lanka",
                "Chami Beats", "Traditional", "Drummer",
                "{\"facebook\":\"fb.com/chamindadrums\",\"instagram\":\"@chamibeats\"}"),

            CreateMember(MemberDiliniId, null, "Dilini", "Jayawardena",
                new DateTime(1995, 8, 22), "Sri Lankan", "957230456V", "N6123890",
                "+94 76 345 6789", "dilini.dance@yahoo.com",
                "33/A, Hill Street, Kandy, Sri Lanka",
                "Dilini J", "Kandyan", "Dancer",
                "{\"instagram\":\"@dilinidance\",\"tiktok\":\"@dilini.kandyan\"}"),

            CreateMember(MemberEshanId, null, "Eshan", "Rodrigo",
                new DateTime(1990, 2, 18), "Sri Lankan", "900490234V", "N4567891",
                "+94 70 456 7890", "eshan.rodrigo@hotmail.com",
                "5, Beach Road, Negombo, Sri Lanka",
                "Eshan R", "Pop/Rock", "Guitarist",
                "{\"facebook\":\"fb.com/eshanrodrigomusic\",\"spotify\":\"EshanRodrigo\",\"youtube\":\"EshanRGuitar\"}"),

            CreateMember(MemberFathimaId, null, "Fathima", "Nazeer",
                new DateTime(1993, 12, 1), "Sri Lankan", "933350189V", "N7234561",
                "+94 75 567 8901", "fathima.n@gmail.com",
                "21, Mosque Road, Beruwala, Sri Lanka",
                "Fathi", "Arabic/Baila", "Vocalist",
                "{\"instagram\":\"@fathinazeer\",\"soundcloud\":\"fathivocals\"}"),

            CreateMember(MemberGayanId, null, "Gayan", "Wickramasinghe",
                new DateTime(1987, 7, 9), "Sri Lankan", "871920567V", "N3456789",
                "+94 72 678 9012", "gayan.keys@gmail.com",
                "90, Galle Face Terrace, Colombo 03, Sri Lanka",
                "G-Keys", "Jazz", "Keyboardist",
                "{\"facebook\":\"fb.com/gayankeys\",\"instagram\":\"@gkeysmusic\",\"bandcamp\":\"gkeysmusic\"}"),

            CreateMember(MemberHiruniId, null, "Hiruni", "Perera",
                new DateTime(1996, 4, 25), "Sri Lankan", "961160890V", "N8901234",
                "+94 78 789 0123", "hiruni.violin@outlook.com",
                "15, Park Avenue, Nugegoda, Sri Lanka",
                "Hiruni P", "Classical", "Violinist",
                "{\"instagram\":\"@hiruniviolin\",\"youtube\":\"HiruniPereraViolin\"}"),

            CreateMember(MemberIndikaId, null, "Indika", "Samarawickrama",
                new DateTime(1991, 9, 30), "Sri Lankan", "912730345V", "N2345678",
                "+94 77 890 1234", "indika.bass@gmail.com",
                "67, Station Road, Panadura, Sri Lanka",
                "Indie Bass", "Reggae", "Bassist",
                "{\"instagram\":\"@indiebassmusic\",\"spotify\":\"IndieBass\"}"),

            CreateMember(MemberJanithId, null, "Janith", "De Silva",
                new DateTime(1994, 1, 12), "Sri Lankan", "940120678V", "N9012345",
                "+94 76 901 2345", "janith.ds@gmail.com",
                "42, Lake Drive, Beira Lake, Colombo 02, Sri Lanka",
                "J-Rhythm", "Traditional", "Percussionist",
                "{\"facebook\":\"fb.com/janithrhythm\",\"instagram\":\"@jrhythm\"}"),

            CreateMember(MemberKumariId, null, "Kumari", "Ranasinghe",
                new DateTime(1997, 6, 8), "Sri Lankan", "976580123V", "N0123456",
                "+94 71 012 3456", "kumari.dance@gmail.com",
                "8/1, Flower Road, Colombo 07, Sri Lanka",
                "KR Contemporary", "Contemporary", "Dancer",
                "{\"instagram\":\"@kumaridance\",\"tiktok\":\"@kr.contemporary\",\"youtube\":\"KumariDance\"}"),
        };

        await context.Members.AddRangeAsync(members);
    }

    private static Member CreateMember(Guid id, Guid? userId, string first, string last,
        DateTime dob, string nationality, string nic, string passport,
        string phone, string email, string address,
        string title, string department, string specialization,
        string customFields)
    {
        return new Member
        {
            Id = id,
            OrganizationId = OrgId,
            UserId = userId,
            LegalFirstName = first,
            LegalLastName = last,
            DateOfBirth = dob,
            Nationality = nationality,
            NicNumber = nic,
            PassportNumber = passport,
            Phone = phone,
            Email = email,
            Address = address,
            Title = title,
            Department = department,
            Specialization = specialization,
            CustomFields = customFields,
            IsActive = true,
            IsDeleted = false,
            CreatedBy = UserKamalId,
            CreatedAt = BaseDate.AddDays(-30),
            UpdatedAt = BaseDate.AddDays(-5)
        };
    }

    // ───────────────────────────────────────────────────────────────────
    // Documents — 3-5 per member with varied statuses
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedDocumentsAsync(ApplicationDbContext context)
    {
        var docs = new List<Document>();
        var versions = new List<DocumentVersion>();
        var docVersionMap = new Dictionary<Guid, Guid>(); // docId -> versionId

        void AddDoc(Guid memberId, string docType, DocumentCategory cat, string title,
            DocumentStatus status, DateTime? expiry, bool hardCopy,
            string? verificationNotes = null, DateTime? verifiedAt = null)
        {
            var docId = Guid.NewGuid();
            var versionId = Guid.NewGuid();
            var uploadDate = BaseDate.AddDays(-Random.Shared.Next(5, 25));
            docVersionMap[docId] = versionId;

            docs.Add(new Document
            {
                Id = docId,
                MemberId = memberId,
                OrganizationId = OrgId,
                Category = cat,
                DocumentType = docType,
                Title = title,
                Status = status,
                CurrentVersionId = null, // Set after versions are inserted
                ExpiryDate = expiry,
                IsHardCopyNeeded = hardCopy,
                VerificationNotes = verificationNotes,
                VerifiedBy = status == DocumentStatus.Verified ? UserKamalId : null,
                VerifiedAt = verifiedAt,
                IsDeleted = false,
                CreatedBy = UserKamalId,
                CreatedAt = uploadDate,
                UpdatedAt = verifiedAt ?? uploadDate
            });

            versions.Add(new DocumentVersion
            {
                Id = versionId,
                DocumentId = docId,
                VersionNumber = 1,
                FileName = $"{title.Replace(" ", "_").ToLowerInvariant()}.pdf",
                FilePath = $"storage/{OrgId}/{memberId}/{cat}/{docId}_v1_{title.Replace(" ", "_").ToLowerInvariant()}.pdf",
                FileSize = Random.Shared.Next(50_000, 5_000_000),
                MimeType = "application/pdf",
                UploadedBy = UserKamalId,
                UploadedAt = uploadDate,
                CreatedAt = uploadDate,
                UpdatedAt = uploadDate
            });
        }

        // ---- Amal Silva (5 docs) ----
        AddDoc(MemberAmalId, "Passport", DocumentCategory.Identity,
            "Amal Silva — Passport", DocumentStatus.Verified,
            new DateTime(2028, 6, 15), true,
            "Verified against original. Valid until June 2028.", BaseDate.AddDays(-18));

        AddDoc(MemberAmalId, "NIC", DocumentCategory.Identity,
            "Amal Silva — NIC", DocumentStatus.Verified,
            null, false,
            "NIC verified. No expiry.", BaseDate.AddDays(-18));

        AddDoc(MemberAmalId, "Police Clearance", DocumentCategory.Legal,
            "Amal Silva — Police Clearance", DocumentStatus.Verified,
            new DateTime(2026, 7, 1), false,
            "Expires July 2026 — expiring soon alert expected.", BaseDate.AddDays(-15));

        AddDoc(MemberAmalId, "Bank Statement", DocumentCategory.Financial,
            "Amal Silva — Bank Statement March 2026", DocumentStatus.Uploaded,
            null, false);

        AddDoc(MemberAmalId, "Passport Photo (35x45mm)", DocumentCategory.Photos,
            "Amal Silva — Passport Photo", DocumentStatus.Verified,
            new DateTime(2027, 1, 1), false,
            "35x45mm, white background, ICAO compliant.", BaseDate.AddDays(-12));

        // ---- Chaminda Bandara (4 docs) ----
        AddDoc(MemberChamindaId, "Passport", DocumentCategory.Identity,
            "Chaminda Bandara — Passport", DocumentStatus.Verified,
            new DateTime(2029, 3, 20), true,
            "Passport verified. Valid until March 2029.", BaseDate.AddDays(-20));

        AddDoc(MemberChamindaId, "NIC", DocumentCategory.Identity,
            "Chaminda Bandara — NIC", DocumentStatus.Verified,
            null, false,
            "NIC verified.", BaseDate.AddDays(-20));

        AddDoc(MemberChamindaId, "Employment Letter", DocumentCategory.Professional,
            "Chaminda Bandara — Employment Letter", DocumentStatus.UnderReview,
            null, false);

        AddDoc(MemberChamindaId, "Bank Statement", DocumentCategory.Financial,
            "Chaminda Bandara — Bank Statement Feb 2026", DocumentStatus.Uploaded,
            null, false);

        // ---- Dilini Jayawardena (4 docs) ----
        AddDoc(MemberDiliniId, "Passport", DocumentCategory.Identity,
            "Dilini Jayawardena — Passport", DocumentStatus.Verified,
            new DateTime(2027, 11, 8), true,
            "Verified. Valid until Nov 2027.", BaseDate.AddDays(-22));

        AddDoc(MemberDiliniId, "NIC", DocumentCategory.Identity,
            "Dilini Jayawardena — NIC", DocumentStatus.Verified,
            null, false,
            "NIC verified.", BaseDate.AddDays(-22));

        AddDoc(MemberDiliniId, "Police Clearance", DocumentCategory.Legal,
            "Dilini Jayawardena — Police Clearance", DocumentStatus.Rejected,
            new DateTime(2026, 9, 30), false,
            "Rejected: Certificate not apostilled. Please resubmit with apostille.", null);

        AddDoc(MemberDiliniId, "Passport Photo (35x45mm)", DocumentCategory.Photos,
            "Dilini Jayawardena — Passport Photo", DocumentStatus.Verified,
            new DateTime(2026, 12, 15), false,
            "Photo meets ICAO standards.", BaseDate.AddDays(-19));

        // ---- Eshan Rodrigo (3 docs) ----
        AddDoc(MemberEshanId, "Passport", DocumentCategory.Identity,
            "Eshan Rodrigo — Passport", DocumentStatus.Verified,
            new DateTime(2026, 5, 1), true,
            "WARNING: Passport expires May 2026 — renewal required before travel.", BaseDate.AddDays(-25));

        AddDoc(MemberEshanId, "NIC", DocumentCategory.Identity,
            "Eshan Rodrigo — NIC", DocumentStatus.Uploaded,
            null, false);

        AddDoc(MemberEshanId, "Bank Statement", DocumentCategory.Financial,
            "Eshan Rodrigo — Bank Statement Jan 2026", DocumentStatus.Expired,
            new DateTime(2026, 2, 28), false);

        // ---- Fathima Nazeer (4 docs) ----
        AddDoc(MemberFathimaId, "Passport", DocumentCategory.Identity,
            "Fathima Nazeer — Passport", DocumentStatus.Verified,
            new DateTime(2030, 1, 10), true,
            "Newly issued passport. Valid until Jan 2030.", BaseDate.AddDays(-14));

        AddDoc(MemberFathimaId, "NIC", DocumentCategory.Identity,
            "Fathima Nazeer — NIC", DocumentStatus.Verified,
            null, false,
            "NIC verified.", BaseDate.AddDays(-14));

        AddDoc(MemberFathimaId, "Travel Insurance", DocumentCategory.Travel,
            "Fathima Nazeer — Travel Insurance (Schengen)", DocumentStatus.Verified,
            new DateTime(2026, 8, 15), false,
            "EUR 30,000 coverage. Allianz policy.", BaseDate.AddDays(-10));

        AddDoc(MemberFathimaId, "Police Clearance", DocumentCategory.Legal,
            "Fathima Nazeer — Police Clearance", DocumentStatus.Verified,
            new DateTime(2026, 8, 20), false,
            "Apostilled and verified.", BaseDate.AddDays(-12));

        // ---- Gayan Wickramasinghe (4 docs) ----
        AddDoc(MemberGayanId, "Passport", DocumentCategory.Identity,
            "Gayan Wickramasinghe — Passport", DocumentStatus.Verified,
            new DateTime(2028, 9, 5), true,
            "Verified. Valid until Sep 2028.", BaseDate.AddDays(-18));

        AddDoc(MemberGayanId, "NIC", DocumentCategory.Identity,
            "Gayan Wickramasinghe — NIC", DocumentStatus.Verified,
            null, false,
            "NIC verified.", BaseDate.AddDays(-18));

        AddDoc(MemberGayanId, "Employment Letter", DocumentCategory.Professional,
            "Gayan Wickramasinghe — Freelance Registration", DocumentStatus.Verified,
            null, false,
            "Self-employed freelance musician. Business registration verified.", BaseDate.AddDays(-16));

        AddDoc(MemberGayanId, "Bank Statement", DocumentCategory.Financial,
            "Gayan Wickramasinghe — Bank Statement March 2026", DocumentStatus.UnderReview,
            null, false);

        // ---- Hiruni Perera (4 docs) ----
        AddDoc(MemberHiruniId, "Passport", DocumentCategory.Identity,
            "Hiruni Perera — Passport", DocumentStatus.Verified,
            new DateTime(2029, 7, 22), true,
            "Verified.", BaseDate.AddDays(-17));

        AddDoc(MemberHiruniId, "NIC", DocumentCategory.Identity,
            "Hiruni Perera — NIC", DocumentStatus.Verified,
            null, false,
            "NIC verified.", BaseDate.AddDays(-17));

        AddDoc(MemberHiruniId, "Vaccination Record", DocumentCategory.Medical,
            "Hiruni Perera — Vaccination Record", DocumentStatus.Verified,
            null, false,
            "COVID-19 and standard vaccinations record.", BaseDate.AddDays(-15));

        AddDoc(MemberHiruniId, "Passport Photo (35x45mm)", DocumentCategory.Photos,
            "Hiruni Perera — Passport Photo", DocumentStatus.Uploaded,
            new DateTime(2027, 3, 1), false);

        // ---- Indika Samarawickrama (3 docs) ----
        AddDoc(MemberIndikaId, "Passport", DocumentCategory.Identity,
            "Indika Samarawickrama — Passport", DocumentStatus.Verified,
            new DateTime(2027, 12, 18), true,
            "Verified. Valid until Dec 2027.", BaseDate.AddDays(-21));

        AddDoc(MemberIndikaId, "NIC", DocumentCategory.Identity,
            "Indika Samarawickrama — NIC", DocumentStatus.Verified,
            null, false,
            "NIC verified.", BaseDate.AddDays(-21));

        AddDoc(MemberIndikaId, "Previous Visa Copy", DocumentCategory.Travel,
            "Indika Samarawickrama — Previous Indian Visa", DocumentStatus.Verified,
            null, false,
            "Previous Indian visa (2024). Tourist visa, complied with terms.", BaseDate.AddDays(-19));

        // ---- Janith De Silva (4 docs) ----
        AddDoc(MemberJanithId, "Passport", DocumentCategory.Identity,
            "Janith De Silva — Passport", DocumentStatus.Verified,
            new DateTime(2028, 4, 30), true,
            "Verified.", BaseDate.AddDays(-16));

        AddDoc(MemberJanithId, "NIC", DocumentCategory.Identity,
            "Janith De Silva — NIC", DocumentStatus.Verified,
            null, false,
            "NIC verified.", BaseDate.AddDays(-16));

        AddDoc(MemberJanithId, "Police Clearance", DocumentCategory.Legal,
            "Janith De Silva — Police Clearance", DocumentStatus.Uploaded,
            new DateTime(2026, 9, 15), false);

        AddDoc(MemberJanithId, "Bank Statement", DocumentCategory.Financial,
            "Janith De Silva — Bank Statement March 2026", DocumentStatus.Verified,
            null, false,
            "Statement shows healthy balance. Verified.", BaseDate.AddDays(-8));

        // ---- Kumari Ranasinghe (3 docs) ----
        AddDoc(MemberKumariId, "Passport", DocumentCategory.Identity,
            "Kumari Ranasinghe — Passport", DocumentStatus.Verified,
            new DateTime(2029, 2, 14), true,
            "Verified.", BaseDate.AddDays(-13));

        AddDoc(MemberKumariId, "NIC", DocumentCategory.Identity,
            "Kumari Ranasinghe — NIC", DocumentStatus.Verified,
            null, false,
            "NIC verified.", BaseDate.AddDays(-13));

        AddDoc(MemberKumariId, "Hotel Reservation", DocumentCategory.Travel,
            "Kumari Ranasinghe — Hotel Booking (Berlin)", DocumentStatus.Uploaded,
            null, false);

        // Step 1: Insert documents without CurrentVersionId
        await context.Documents.AddRangeAsync(docs);
        await context.SaveChangesAsync();

        // Step 2: Insert document versions
        await context.DocumentVersions.AddRangeAsync(versions);
        await context.SaveChangesAsync();

        // Step 3: Update documents with CurrentVersionId
        foreach (var doc in docs)
        {
            if (docVersionMap.TryGetValue(doc.Id, out var versionId))
            {
                doc.CurrentVersionId = versionId;
            }
        }
        await context.SaveChangesAsync();
    }

    // ───────────────────────────────────────────────────────────────────
    // Cases
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedCasesAsync(ApplicationDbContext context)
    {
        var cases = new[]
        {
            new Case
            {
                Id = CaseEuropeId,
                OrganizationId = OrgId,
                Name = "European Summer Tour 2026",
                CaseType = "tour",
                ReferenceNumber = "CASE-20260215-EU2026",
                DestinationCountry = "Germany",
                DestinationCity = "Berlin, Munich, Frankfurt",
                Venue = "Multiple venues — see itinerary",
                StartDate = new DateTime(2026, 6, 15),
                EndDate = new DateTime(2026, 7, 5),
                ContactName = "Hans Mueller",
                ContactEmail = "hans.mueller@berlinfest.de",
                ContactPhone = "+49 30 1234 5678",
                ChecklistId = CountryChecklistSeeder.SchengenChecklistId,
                Status = CaseStatus.Active,
                Description = "Three-week European tour covering Berlin, Munich, and Frankfurt. Schengen Type C visa required for all performers.",
                Notes = "Priority processing needed — departure in ~3 months.",
                IsDeleted = false,
                CreatedBy = UserKamalId,
                CreatedAt = BaseDate.AddDays(-20),
                UpdatedAt = BaseDate.AddDays(-2)
            },
            new Case
            {
                Id = CaseDubaiId,
                OrganizationId = OrgId,
                Name = "Dubai Music Festival",
                CaseType = "visa_application",
                ReferenceNumber = "CASE-20260301-DXB01",
                DestinationCountry = "UAE",
                DestinationCity = "Dubai",
                Venue = "Dubai World Trade Centre",
                StartDate = new DateTime(2026, 8, 10),
                EndDate = new DateTime(2026, 8, 15),
                ContactName = "Ahmed Al Maktoum",
                ContactEmail = "ahmed@dubaimusicfest.ae",
                ContactPhone = "+971 4 567 8901",
                ChecklistId = CountryChecklistSeeder.UaeChecklistId,
                Status = CaseStatus.Draft,
                Description = "Five-day music festival appearance in Dubai. UAE tourist visa via sponsor (festival organizer).",
                Notes = "Festival organizer will serve as sponsor for visa applications.",
                IsDeleted = false,
                CreatedBy = UserKamalId,
                CreatedAt = BaseDate.AddDays(-10),
                UpdatedAt = BaseDate.AddDays(-10)
            },
            new Case
            {
                Id = CaseJapanId,
                OrganizationId = OrgId,
                Name = "Japan Cultural Exchange",
                CaseType = "tour",
                ReferenceNumber = "CASE-20260305-JP01",
                DestinationCountry = "Japan",
                DestinationCity = "Tokyo, Osaka, Kyoto",
                Venue = "NHK Hall, Osaka Festival Hall",
                StartDate = new DateTime(2026, 10, 1),
                EndDate = new DateTime(2026, 10, 14),
                ContactName = "Yuki Tanaka",
                ContactEmail = "y.tanaka@japancultural.jp",
                ContactPhone = "+81 3 1234 5678",
                ChecklistId = CountryChecklistSeeder.JapanChecklistId,
                Status = CaseStatus.Active,
                Description = "Two-week cultural exchange program featuring traditional and contemporary Sri Lankan music and dance in Tokyo, Osaka, and Kyoto.",
                Notes = "Japanese guarantor letter pending from Tanaka-san's organization.",
                IsDeleted = false,
                CreatedBy = UserNishaniId,
                CreatedAt = BaseDate.AddDays(-5),
                UpdatedAt = BaseDate.AddDays(-3)
            },
        };

        await context.Cases.AddRangeAsync(cases);
    }

    // ───────────────────────────────────────────────────────────────────
    // Case members
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedCaseMembersAsync(ApplicationDbContext context)
    {
        var assignments = new List<CaseMember>();

        // European tour — 6 members
        var euroMembers = new[] { MemberAmalId, MemberChamindaId, MemberDiliniId,
                                  MemberFathimaId, MemberGayanId, MemberHiruniId };
        foreach (var mId in euroMembers)
        {
            assignments.Add(new CaseMember
            {
                Id = Guid.NewGuid(), CaseId = CaseEuropeId, MemberId = mId,
                Status = "Assigned", AddedAt = BaseDate.AddDays(-18),
                CreatedAt = BaseDate.AddDays(-18), UpdatedAt = BaseDate.AddDays(-18)
            });
        }

        // Dubai festival — 4 members
        var dubaiMembers = new[] { MemberAmalId, MemberEshanId, MemberFathimaId, MemberIndikaId };
        foreach (var mId in dubaiMembers)
        {
            assignments.Add(new CaseMember
            {
                Id = Guid.NewGuid(), CaseId = CaseDubaiId, MemberId = mId,
                Status = "Assigned", AddedAt = BaseDate.AddDays(-9),
                CreatedAt = BaseDate.AddDays(-9), UpdatedAt = BaseDate.AddDays(-9)
            });
        }

        // Japan exchange — 5 members
        var japanMembers = new[] { MemberChamindaId, MemberDiliniId, MemberJanithId,
                                   MemberKumariId, MemberHiruniId };
        foreach (var mId in japanMembers)
        {
            assignments.Add(new CaseMember
            {
                Id = Guid.NewGuid(), CaseId = CaseJapanId, MemberId = mId,
                Status = "Assigned", AddedAt = BaseDate.AddDays(-4),
                CreatedAt = BaseDate.AddDays(-4), UpdatedAt = BaseDate.AddDays(-4)
            });
        }

        await context.CaseMembers.AddRangeAsync(assignments);
    }

    // ───────────────────────────────────────────────────────────────────
    // Case access
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedCaseAccessAsync(ApplicationDbContext context)
    {
        var accesses = new[]
        {
            // Saman (visa handler) — ViewDownload on European tour
            new CaseAccess
            {
                Id = Guid.NewGuid(), CaseId = CaseEuropeId, UserId = UserSamanId,
                Role = UserRole.DocumentHandler, Permission = CaseAccessPermission.ViewDownload,
                GrantedBy = UserKamalId, GrantedAt = BaseDate.AddDays(-17),
                ExpiresAt = new DateTime(2026, 7, 31), IsActive = true,
                CreatedAt = BaseDate.AddDays(-17), UpdatedAt = BaseDate.AddDays(-17)
            },
            // Lanka Events — View on European tour
            new CaseAccess
            {
                Id = Guid.NewGuid(), CaseId = CaseEuropeId, UserId = UserLankaEventsId,
                Role = UserRole.CaseManager, Permission = CaseAccessPermission.View,
                GrantedBy = UserKamalId, GrantedAt = BaseDate.AddDays(-16),
                ExpiresAt = new DateTime(2026, 7, 31), IsActive = true,
                CreatedAt = BaseDate.AddDays(-16), UpdatedAt = BaseDate.AddDays(-16)
            },
            // Saman — ViewDownload on Japan tour
            new CaseAccess
            {
                Id = Guid.NewGuid(), CaseId = CaseJapanId, UserId = UserSamanId,
                Role = UserRole.DocumentHandler, Permission = CaseAccessPermission.ViewDownload,
                GrantedBy = UserNishaniId, GrantedAt = BaseDate.AddDays(-3),
                ExpiresAt = new DateTime(2026, 11, 30), IsActive = true,
                CreatedAt = BaseDate.AddDays(-3), UpdatedAt = BaseDate.AddDays(-3)
            },
        };

        await context.CaseAccesses.AddRangeAsync(accesses);
    }

    // ───────────────────────────────────────────────────────────────────
    // Hard copy requests
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedHardCopyRequestsAsync(ApplicationDbContext context)
    {
        // Get first few document IDs — we'll pick passport docs for hard copy
        // We need documents that have IsHardCopyNeeded = true (passports)
        // Since we add docs above in the same SaveChanges, we reference them by known member IDs

        var hcr1Id = Guid.Parse("e1000000-0000-0000-0000-000000000001");
        var hcr2Id = Guid.Parse("e1000000-0000-0000-0000-000000000002");
        var hcr3Id = Guid.Parse("e1000000-0000-0000-0000-000000000003");

        // We need document IDs, but they are generated with Guid.NewGuid() above.
        // For determinism, let's add the hard copy requests referencing documents
        // from context after they're saved. However since we're in the same SaveChanges,
        // we'll use a workaround: query the local change tracker.

        // Since documents aren't saved yet, use the change tracker
        var amalPassport = context.ChangeTracker.Entries<Document>()
            .FirstOrDefault(e => e.Entity.MemberId == MemberAmalId && e.Entity.DocumentType == "Passport")?.Entity;
        var chamindaPassport = context.ChangeTracker.Entries<Document>()
            .FirstOrDefault(e => e.Entity.MemberId == MemberChamindaId && e.Entity.DocumentType == "Passport")?.Entity;
        var diliniPassport = context.ChangeTracker.Entries<Document>()
            .FirstOrDefault(e => e.Entity.MemberId == MemberDiliniId && e.Entity.DocumentType == "Passport")?.Entity;

        if (amalPassport == null || chamindaPassport == null || diliniPassport == null)
        {
            return; // Safety check
        }

        var requests = new[]
        {
            // 1. HandedToHandler — Amal's passport for European tour
            new HardCopyRequest
            {
                Id = hcr1Id,
                DocumentId = amalPassport.Id,
                CaseId = CaseEuropeId,
                RequestedBy = UserKamalId,
                Status = HardCopyStatus.HandedToHandler,
                Urgency = Urgency.High,
                Notes = "Passport needed for Schengen visa submission. Saman has the original.",
                CreatedAt = BaseDate.AddDays(-15),
                UpdatedAt = BaseDate.AddDays(-8)
            },
            // 2. Requested — Chaminda's passport for Japan tour
            new HardCopyRequest
            {
                Id = hcr2Id,
                DocumentId = chamindaPassport.Id,
                CaseId = CaseJapanId,
                RequestedBy = UserNishaniId,
                Status = HardCopyStatus.Requested,
                Urgency = Urgency.Normal,
                Notes = "Passport original needed for Japan visa application.",
                CreatedAt = BaseDate.AddDays(-3),
                UpdatedAt = BaseDate.AddDays(-3)
            },
            // 3. ReturnedToMember — Dilini's passport (completed cycle)
            new HardCopyRequest
            {
                Id = hcr3Id,
                DocumentId = diliniPassport.Id,
                CaseId = CaseEuropeId,
                RequestedBy = UserKamalId,
                Status = HardCopyStatus.ReturnedToMember,
                Urgency = Urgency.Normal,
                Notes = "Passport returned after Schengen visa stamp received.",
                CreatedAt = BaseDate.AddDays(-20),
                UpdatedAt = BaseDate.AddDays(-2)
            },
        };

        await context.HardCopyRequests.AddRangeAsync(requests);

        // Handover chain for the completed request (hcr3)
        var handovers = new[]
        {
            new HardCopyHandover
            {
                Id = Guid.NewGuid(),
                HardCopyRequestId = hcr3Id,
                FromUserId = UserAmalId, // Pretend Dilini's user if she had one — using Amal's for demo
                ToUserId = UserKamalId,
                FromRole = "Member",
                ToRole = "OrgOwner",
                HandoverType = "CollectedByManager",
                ConfirmationMethod = "OTP",
                ConfirmationData = "847291",
                Notes = "Collected passport original from Dilini at office.",
                RecordedAt = BaseDate.AddDays(-18),
                RecordedBy = UserKamalId,
                CreatedAt = BaseDate.AddDays(-18),
                UpdatedAt = BaseDate.AddDays(-18)
            },
            new HardCopyHandover
            {
                Id = Guid.NewGuid(),
                HardCopyRequestId = hcr3Id,
                FromUserId = UserKamalId,
                ToUserId = UserSamanId,
                FromRole = "OrgOwner",
                ToRole = "DocumentHandler",
                HandoverType = "HandedToHandler",
                ConfirmationMethod = "OTP",
                ConfirmationData = "193456",
                Notes = "Handed passport to Saman for visa processing.",
                RecordedAt = BaseDate.AddDays(-16),
                RecordedBy = UserKamalId,
                CreatedAt = BaseDate.AddDays(-16),
                UpdatedAt = BaseDate.AddDays(-16)
            },
            new HardCopyHandover
            {
                Id = Guid.NewGuid(),
                HardCopyRequestId = hcr3Id,
                FromUserId = UserSamanId,
                ToUserId = UserKamalId,
                FromRole = "DocumentHandler",
                ToRole = "OrgOwner",
                HandoverType = "ReturnedToManager",
                ConfirmationMethod = "OTP",
                ConfirmationData = "562078",
                Notes = "Visa approved and stamped. Passport returned to Kamal.",
                RecordedAt = BaseDate.AddDays(-5),
                RecordedBy = UserSamanId,
                CreatedAt = BaseDate.AddDays(-5),
                UpdatedAt = BaseDate.AddDays(-5)
            },
            new HardCopyHandover
            {
                Id = Guid.NewGuid(),
                HardCopyRequestId = hcr3Id,
                FromUserId = UserKamalId,
                ToUserId = UserAmalId, // Dilini placeholder
                FromRole = "OrgOwner",
                ToRole = "Member",
                HandoverType = "ReturnedToMember",
                ConfirmationMethod = "OTP",
                ConfirmationData = "738412",
                Notes = "Passport with Schengen visa stamp returned to Dilini.",
                RecordedAt = BaseDate.AddDays(-2),
                RecordedBy = UserKamalId,
                CreatedAt = BaseDate.AddDays(-2),
                UpdatedAt = BaseDate.AddDays(-2)
            },
        };

        // Handover chain for hcr1 (HandedToHandler — partial)
        var hcr1Handovers = new[]
        {
            new HardCopyHandover
            {
                Id = Guid.NewGuid(),
                HardCopyRequestId = hcr1Id,
                FromUserId = UserAmalId,
                ToUserId = UserKamalId,
                FromRole = "Member",
                ToRole = "OrgOwner",
                HandoverType = "CollectedByManager",
                ConfirmationMethod = "OTP",
                ConfirmationData = "451829",
                Notes = "Collected Amal's passport for European tour visa submission.",
                RecordedAt = BaseDate.AddDays(-12),
                RecordedBy = UserKamalId,
                CreatedAt = BaseDate.AddDays(-12),
                UpdatedAt = BaseDate.AddDays(-12)
            },
            new HardCopyHandover
            {
                Id = Guid.NewGuid(),
                HardCopyRequestId = hcr1Id,
                FromUserId = UserKamalId,
                ToUserId = UserSamanId,
                FromRole = "OrgOwner",
                ToRole = "DocumentHandler",
                HandoverType = "HandedToHandler",
                ConfirmationMethod = "OTP",
                ConfirmationData = "267934",
                Notes = "Passport handed to Saman Visa Services for Schengen visa processing.",
                RecordedAt = BaseDate.AddDays(-8),
                RecordedBy = UserKamalId,
                CreatedAt = BaseDate.AddDays(-8),
                UpdatedAt = BaseDate.AddDays(-8)
            },
        };

        await context.HardCopyHandovers.AddRangeAsync(handovers);
        await context.HardCopyHandovers.AddRangeAsync(hcr1Handovers);
    }

    // ───────────────────────────────────────────────────────────────────
    // Document requests
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedDocumentRequestsAsync(ApplicationDbContext context)
    {
        var requests = new[]
        {
            // 1. Requested — new, waiting
            new DocumentRequest
            {
                Id = Guid.NewGuid(),
                CaseId = CaseEuropeId,
                MemberId = MemberEshanId,
                RequestedBy = UserKamalId,
                DocumentType = "Police Clearance",
                FormatRequirements = "Must be apostilled by Ministry of Foreign Affairs. Less than 6 months old.",
                Urgency = Urgency.High,
                Notes = "Eshan needs a police clearance for the Schengen visa application. Deadline: May 15, 2026.",
                Status = DocumentRequestStatus.Requested,
                CreatedAt = BaseDate.AddDays(-3),
                UpdatedAt = BaseDate.AddDays(-3)
            },
            // 2. InProgress — acknowledged
            new DocumentRequest
            {
                Id = Guid.NewGuid(),
                CaseId = CaseJapanId,
                MemberId = MemberJanithId,
                RequestedBy = UserNishaniId,
                DocumentType = "Employment Letter",
                FormatRequirements = "On company/group letterhead. Must state position, salary, and approved leave period for October 2026.",
                Urgency = Urgency.Normal,
                Notes = "Janith acknowledged and is getting the letter from Rhythm & Routes management.",
                Status = DocumentRequestStatus.InProgress,
                CreatedAt = BaseDate.AddDays(-4),
                UpdatedAt = BaseDate.AddDays(-2)
            },
            // 3. Fulfilled — completed
            new DocumentRequest
            {
                Id = Guid.NewGuid(),
                CaseId = CaseEuropeId,
                MemberId = MemberFathimaId,
                RequestedBy = UserKamalId,
                DocumentType = "Travel Insurance",
                FormatRequirements = "Schengen-compliant: minimum EUR 30,000 coverage, valid for all Schengen states.",
                Urgency = Urgency.Normal,
                Notes = "Fathima submitted Allianz travel insurance policy. Verified and linked.",
                Status = DocumentRequestStatus.Fulfilled,
                FulfilledDocumentId = context.ChangeTracker.Entries<Document>()
                    .FirstOrDefault(e => e.Entity.MemberId == MemberFathimaId && e.Entity.DocumentType == "Travel Insurance")?.Entity.Id,
                CreatedAt = BaseDate.AddDays(-12),
                UpdatedAt = BaseDate.AddDays(-10)
            },
        };

        await context.DocumentRequests.AddRangeAsync(requests);
    }

    // ───────────────────────────────────────────────────────────────────
    // Notifications
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedNotificationsAsync(ApplicationDbContext context)
    {
        var notifications = new[]
        {
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserKamalId, OrganizationId = OrgId,
                Type = NotificationType.DocumentExpiring,
                Title = "Document Expiring Soon",
                Message = "Amal Silva's Police Clearance expires on July 1, 2026 (in ~3 months). Request renewal to avoid delays for European Summer Tour 2026.",
                EntityType = "Document", EntityId = null,
                Channel = "InApp", IsRead = false,
                SentAt = BaseDate.AddDays(-1), CreatedAt = BaseDate.AddDays(-1), UpdatedAt = BaseDate.AddDays(-1)
            },
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserKamalId, OrganizationId = OrgId,
                Type = NotificationType.DocumentExpiring,
                Title = "URGENT: Passport Expiring",
                Message = "Eshan Rodrigo's Passport expires on May 1, 2026. This passport will NOT meet the 6-month validity requirement for any visa application. Immediate renewal required.",
                EntityType = "Document", EntityId = null,
                Channel = "InApp", IsRead = true, ReadAt = BaseDate.AddHours(-5),
                SentAt = BaseDate.AddDays(-2), CreatedAt = BaseDate.AddDays(-2), UpdatedAt = BaseDate.AddHours(-5)
            },
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserKamalId, OrganizationId = OrgId,
                Type = NotificationType.DocumentUploaded,
                Title = "New Document Uploaded",
                Message = "Amal Silva uploaded a new Bank Statement (March 2026). Review and verify the document.",
                EntityType = "Document", EntityId = null,
                Channel = "InApp", IsRead = false,
                SentAt = BaseDate.AddDays(-1), CreatedAt = BaseDate.AddDays(-1), UpdatedAt = BaseDate.AddDays(-1)
            },
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserNishaniId, OrganizationId = OrgId,
                Type = NotificationType.CaseCreated,
                Title = "New Case Created",
                Message = "Kamal Perera created a new case: 'Dubai Music Festival' (CASE-20260301-DXB01). Destination: UAE, Dates: Aug 10-15, 2026.",
                EntityType = "Case", EntityId = CaseDubaiId,
                Channel = "InApp", IsRead = true, ReadAt = BaseDate.AddDays(-9),
                SentAt = BaseDate.AddDays(-10), CreatedAt = BaseDate.AddDays(-10), UpdatedAt = BaseDate.AddDays(-9)
            },
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserSamanId, OrganizationId = OrgId,
                Type = NotificationType.AccessGranted,
                Title = "Case Access Granted",
                Message = "You have been granted ViewDownload access to 'European Summer Tour 2026'. You can now view and download documents for all assigned members.",
                EntityType = "Case", EntityId = CaseEuropeId,
                Channel = "InApp", IsRead = true, ReadAt = BaseDate.AddDays(-16),
                SentAt = BaseDate.AddDays(-17), CreatedAt = BaseDate.AddDays(-17), UpdatedAt = BaseDate.AddDays(-16)
            },
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserSamanId, OrganizationId = OrgId,
                Type = NotificationType.HardCopyStatusChanged,
                Title = "Hard Copy Received",
                Message = "You received Amal Silva's Passport (original) from Kamal Perera. OTP confirmation: 267934. Please process the Schengen visa application.",
                EntityType = "HardCopyRequest", EntityId = null,
                Channel = "InApp", IsRead = true, ReadAt = BaseDate.AddDays(-7),
                SentAt = BaseDate.AddDays(-8), CreatedAt = BaseDate.AddDays(-8), UpdatedAt = BaseDate.AddDays(-7)
            },
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserAmalId, OrganizationId = OrgId,
                Type = NotificationType.DocumentRequested,
                Title = "Document Requested",
                Message = "Your organization has requested a Police Clearance for the European Summer Tour 2026. Please upload an apostilled certificate less than 6 months old.",
                EntityType = "DocumentRequest", EntityId = null,
                Channel = "InApp", IsRead = false,
                SentAt = BaseDate.AddDays(-3), CreatedAt = BaseDate.AddDays(-3), UpdatedAt = BaseDate.AddDays(-3)
            },
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserKamalId, OrganizationId = OrgId,
                Type = NotificationType.DocumentRejected,
                Title = "Document Rejected",
                Message = "Dilini Jayawardena's Police Clearance was rejected. Reason: Certificate not apostilled. Member has been notified to resubmit.",
                EntityType = "Document", EntityId = null,
                Channel = "InApp", IsRead = true, ReadAt = BaseDate.AddDays(-5),
                SentAt = BaseDate.AddDays(-6), CreatedAt = BaseDate.AddDays(-6), UpdatedAt = BaseDate.AddDays(-5)
            },
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserLankaEventsId, OrganizationId = OrgId,
                Type = NotificationType.AccessGranted,
                Title = "Case Access Granted",
                Message = "You have been granted View access to 'European Summer Tour 2026'. You can view case details and member document readiness.",
                EntityType = "Case", EntityId = CaseEuropeId,
                Channel = "InApp", IsRead = false,
                SentAt = BaseDate.AddDays(-16), CreatedAt = BaseDate.AddDays(-16), UpdatedAt = BaseDate.AddDays(-16)
            },
            new Notification
            {
                Id = Guid.NewGuid(), UserId = UserNishaniId, OrganizationId = OrgId,
                Type = NotificationType.DocumentVerified,
                Title = "Document Verified",
                Message = "Janith De Silva's Bank Statement (March 2026) has been verified by Kamal Perera.",
                EntityType = "Document", EntityId = null,
                Channel = "InApp", IsRead = false,
                SentAt = BaseDate.AddDays(-8), CreatedAt = BaseDate.AddDays(-8), UpdatedAt = BaseDate.AddDays(-8)
            },
        };

        await context.Notifications.AddRangeAsync(notifications);
    }

    // ───────────────────────────────────────────────────────────────────
    // Audit logs — 20 entries spanning the last 30 days
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedAuditLogsAsync(ApplicationDbContext context)
    {
        var logs = new[]
        {
            AuditEntry(-28, UserKamalId, "Organization.Created", "Organization", OrgId,
                "Created organization: Rhythm & Routes Entertainment"),
            AuditEntry(-27, UserKamalId, "User.Invited", "OrganizationMember", null,
                "Invited Nishani Fernando (nishani@rhythmroutes.lk) as OrgMember"),
            AuditEntry(-25, UserKamalId, "Member.Created", "Member", MemberAmalId,
                "Created member: Amal Silva (Vocalist, Baila)"),
            AuditEntry(-25, UserKamalId, "Member.Created", "Member", MemberChamindaId,
                "Created member: Chaminda Bandara (Drummer, Traditional)"),
            AuditEntry(-24, UserKamalId, "Member.Created", "Member", MemberDiliniId,
                "Created member: Dilini Jayawardena (Dancer, Kandyan)"),
            AuditEntry(-23, UserKamalId, "Document.Uploaded", "Document", null,
                "Uploaded document: Amal Silva — Passport (v1)"),
            AuditEntry(-22, UserKamalId, "Document.Uploaded", "Document", null,
                "Uploaded document: Chaminda Bandara — Passport (v1)"),
            AuditEntry(-20, UserKamalId, "Case.Created", "Case", CaseEuropeId,
                "Created case: European Summer Tour 2026 (CASE-20260215-EU2026)"),
            AuditEntry(-19, UserKamalId, "Case.MembersAssigned", "Case", CaseEuropeId,
                "Assigned 6 members to case: European Summer Tour 2026"),
            AuditEntry(-18, UserKamalId, "Document.Verified", "Document", null,
                "Verified document: Amal Silva — Passport"),
            AuditEntry(-17, UserKamalId, "Case.AccessGranted", "CaseAccess", null,
                "Granted ViewDownload access to Saman Visa Services for European Summer Tour 2026"),
            AuditEntry(-15, UserKamalId, "HardCopy.Requested", "HardCopyRequest", null,
                "Requested hard copy: Amal Silva's Passport for European Summer Tour 2026"),
            AuditEntry(-12, UserKamalId, "HardCopy.Collected", "HardCopyHandover", null,
                "Collected Amal Silva's passport original. OTP: 451829"),
            AuditEntry(-10, UserKamalId, "Case.Created", "Case", CaseDubaiId,
                "Created case: Dubai Music Festival (CASE-20260301-DXB01)"),
            AuditEntry(-8, UserKamalId, "HardCopy.HandedToHandler", "HardCopyHandover", null,
                "Handed Amal Silva's passport to Saman Visa Services. OTP: 267934"),
            AuditEntry(-8, UserKamalId, "Document.Verified", "Document", null,
                "Verified document: Janith De Silva — Bank Statement March 2026"),
            AuditEntry(-6, UserKamalId, "Document.Rejected", "Document", null,
                "Rejected document: Dilini Jayawardena — Police Clearance. Reason: Not apostilled"),
            AuditEntry(-5, UserNishaniId, "Case.Created", "Case", CaseJapanId,
                "Created case: Japan Cultural Exchange (CASE-20260305-JP01)"),
            AuditEntry(-3, UserSamanId, "Document.Downloaded", "Document", null,
                "Downloaded document: Fathima Nazeer — Passport (for Schengen visa processing)"),
            AuditEntry(-1, UserKamalId, "User.Login", null, null,
                "User logged in from 203.115.24.56"),
        };

        await context.AuditLogs.AddRangeAsync(logs);
    }

    private static AuditLog AuditEntry(int daysOffset, Guid userId, string action,
        string? entityType, Guid? entityId, string details)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            OrganizationId = OrgId,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IpAddress = "203.115.24.56",
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/122.0.0.0 Safari/537.36",
            CreatedAt = BaseDate.AddDays(daysOffset)
        };
    }

    // ───────────────────────────────────────────────────────────────────
    // Subscription
    // ───────────────────────────────────────────────────────────────────
    private static async Task SeedSubscriptionAsync(ApplicationDbContext context)
    {
        var subscription = new Subscription
        {
            Id = SubscriptionId,
            OrganizationId = OrgId,
            Plan = SubscriptionPlan.Professional,
            Status = "Active",
            MaxMembers = 50,
            MaxCasesMonthly = 10,
            MaxExternalUsers = 10,
            MaxStorageBytes = 26_843_545_600, // 25 GB
            PaymentGatewayCustomerId = "cus_demo_rhythmroutes",
            PaymentGatewaySubscriptionId = "sub_demo_professional",
            CurrentPeriodStart = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
            CurrentPeriodEnd = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = BaseDate.AddDays(-60),
            UpdatedAt = BaseDate
        };

        await context.Subscriptions.AddAsync(subscription);
    }
}

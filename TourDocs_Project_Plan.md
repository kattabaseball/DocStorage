# TourDocs - Full Project Plan & Technical Specification

**Version:** 2.0
**Date:** March 25, 2026
**Stack:** Angular 18 + Angular Material + .NET Core 8 Web API (N-Tier)
**Database:** Microsoft SQL Server
**UI Reference:** Primer Angular Material Design Admin Template (Sidenav + Toolbar + Card-based layout)

---

## 1. Architecture Overview

### 1.1 High-Level Architecture (N-Tier)

```
┌──────────────────────────────────────────────────────────────┐
│                   PRESENTATION LAYER                         │
│            Angular 18 + Angular Material + SCSS              │
│        Lazy-loaded modules, Standalone components            │
├──────────────────────────────────────────────────────────────┤
│                      API LAYER                               │
│               .NET Core 8 Web API                            │
│     Controllers │ Filters │ Middleware │ Hubs                │
│     JWT Auth │ RBAC │ Rate Limiting │ API Versioning         │
├──────────────────────────────────────────────────────────────┤
│                   BUSINESS LOGIC LAYER                       │
│                   Service Classes                            │
│  OrganizationService │ MemberService │ DocumentService       │
│  CaseService │ HardCopyService │ NotificationService         │
│  ChecklistService │ AuditService │ AuthService               │
├──────────────────────────────────────────────────────────────┤
│                   DATA ACCESS LAYER                          │
│          EF Core 8 │ Repositories │ Unit of Work             │
├──────────────────────────────────────────────────────────────┤
│                    DATA STORES                               │
│     MS SQL Server │ Local File Storage │ Redis Cache          │
└──────────────────────────────────────────────────────────────┘
```

### 1.2 Technology Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **Frontend** | Angular 18, Angular Material 18, SCSS | SPA with Material Design UI |
| **State Management** | NgRx (Store, Effects, Entity) | Predictable state management |
| **Backend API** | .NET Core 8 Web API | RESTful API |
| **Architecture** | N-Tier (API → Service → Repository → Data) | Clean separation of concerns |
| **ORM** | Entity Framework Core 8 | Database access |
| **Database** | Microsoft SQL Server 2022 | Primary data store |
| **File Storage** | Local file system (cloud-ready interface) | Document storage with IFileStorageService abstraction |
| **Caching** | Redis | Session cache, frequently accessed data |
| **Background Jobs** | Hangfire | Expiry alerts, notifications, scheduled tasks |
| **Authentication** | ASP.NET Core Identity + JWT | User auth with refresh tokens |
| **Real-time** | SignalR | Live notifications, status updates |
| **Email** | SendGrid | Transactional emails |
| **WhatsApp** | Twilio WhatsApp Business API | Member notifications |
| **OCR/MRZ** | Tesseract OCR / Mindee API | Passport data extraction |
| **Logging** | Serilog + Seq | Structured logging |
| **API Docs** | Swagger / NSwag | Auto-generated API docs + Angular client |

### 1.3 Generic Domain Model

TourDocs is designed to be **domain-agnostic**. The core concepts are generic and configurable per organization:

| Core Concept | Description | Examples |
|--------------|-------------|---------|
| **Organization** | Any entity managing people and their documents | Artist agency, travel agency, HR department, university, immigration consultancy |
| **Member** | Any person whose documents are managed | Performing artist, employee, student, athlete, work-permit applicant |
| **Case** | Any scenario requiring a set of documents | Outbound tour, visa application, work permit, scholarship, contract renewal |
| **Document** | Any file with metadata, versioning, and expiry tracking | Passport, bank statement, police clearance, employment letter |
| **Checklist** | A template of required documents for a case type | Schengen visa, UK work permit, student visa, sports tour |
| **Hard Copy** | Physical original document with chain-of-custody tracking | Passport, birth certificate, police clearance |

This means a **sports agency** managing athletes' visa docs, an **HR department** handling employee work permits, or a **university** processing student exchange applications can all use TourDocs without modification.

---

## 2. Solution Structure

### 2.1 Backend - .NET Core N-Tier Solution

```
TourDocs/
├── src/
│   ├── TourDocs.API/                              # API Layer (Presentation)
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs                  # Login, register, tokens
│   │   │   ├── OrganizationsController.cs         # Org CRUD, settings
│   │   │   ├── MembersController.cs               # Member management
│   │   │   ├── DocumentsController.cs             # Upload, verify, download
│   │   │   ├── CasesController.cs                 # Case lifecycle
│   │   │   ├── ChecklistsController.cs            # Visa/doc checklists
│   │   │   ├── HardCopyRequestsController.cs      # Physical doc tracking
│   │   │   ├── DocumentRequestsController.cs      # Additional doc requests
│   │   │   ├── NotificationsController.cs         # User notifications
│   │   │   ├── AuditController.cs                 # Audit log viewer
│   │   │   ├── DashboardController.cs             # Stats & widgets
│   │   │   └── SubscriptionsController.cs         # Billing & plans
│   │   ├── Filters/
│   │   │   ├── ApiExceptionFilter.cs              # Global error handling
│   │   │   └── AuditActionFilter.cs               # Auto audit logging
│   │   ├── Middleware/
│   │   │   ├── TenantMiddleware.cs                # Org context from JWT
│   │   │   ├── RateLimitingMiddleware.cs
│   │   │   └── RequestLoggingMiddleware.cs
│   │   ├── Hubs/
│   │   │   └── NotificationHub.cs                 # SignalR hub
│   │   ├── Extensions/
│   │   │   ├── ServiceCollectionExtensions.cs      # DI registration
│   │   │   └── ApplicationBuilderExtensions.cs     # Middleware pipeline
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Program.cs
│   │
│   ├── TourDocs.Core/                             # Business Logic Layer
│   │   ├── Interfaces/                            # Service contracts
│   │   │   ├── IOrganizationService.cs
│   │   │   ├── IMemberService.cs
│   │   │   ├── IDocumentService.cs
│   │   │   ├── ICaseService.cs
│   │   │   ├── IChecklistService.cs
│   │   │   ├── IHardCopyService.cs
│   │   │   ├── IDocumentRequestService.cs
│   │   │   ├── INotificationService.cs
│   │   │   ├── IAuditService.cs
│   │   │   ├── IAuthService.cs
│   │   │   ├── IDashboardService.cs
│   │   │   ├── ISubscriptionService.cs
│   │   │   ├── IFileStorageService.cs             # Abstraction (local now, cloud later)
│   │   │   └── ICurrentUserService.cs
│   │   ├── Services/                              # Service implementations
│   │   │   ├── OrganizationService.cs
│   │   │   ├── MemberService.cs
│   │   │   ├── DocumentService.cs
│   │   │   ├── CaseService.cs
│   │   │   ├── ChecklistService.cs
│   │   │   ├── HardCopyService.cs
│   │   │   ├── DocumentRequestService.cs
│   │   │   ├── NotificationService.cs
│   │   │   ├── AuditService.cs
│   │   │   ├── AuthService.cs
│   │   │   ├── DashboardService.cs
│   │   │   └── SubscriptionService.cs
│   │   ├── DTOs/                                  # Data Transfer Objects
│   │   │   ├── Auth/
│   │   │   │   ├── LoginDto.cs
│   │   │   │   ├── RegisterDto.cs
│   │   │   │   ├── TokenResponseDto.cs
│   │   │   │   ├── RefreshTokenDto.cs
│   │   │   │   └── UserProfileDto.cs
│   │   │   ├── Organizations/
│   │   │   │   ├── OrganizationDto.cs
│   │   │   │   ├── CreateOrganizationDto.cs
│   │   │   │   ├── UpdateOrganizationDto.cs
│   │   │   │   ├── OrganizationDashboardDto.cs
│   │   │   │   ├── TeamMemberDto.cs
│   │   │   │   └── InviteMemberDto.cs
│   │   │   ├── Members/
│   │   │   │   ├── MemberDto.cs
│   │   │   │   ├── MemberListDto.cs
│   │   │   │   ├── CreateMemberDto.cs
│   │   │   │   ├── UpdateMemberDto.cs
│   │   │   │   ├── MemberDocumentSummaryDto.cs
│   │   │   │   └── TravelHistoryDto.cs
│   │   │   ├── Documents/
│   │   │   │   ├── DocumentDto.cs
│   │   │   │   ├── DocumentListDto.cs
│   │   │   │   ├── UploadDocumentDto.cs
│   │   │   │   ├── VerifyDocumentDto.cs
│   │   │   │   ├── DocumentVersionDto.cs
│   │   │   │   └── ExpiringDocumentDto.cs
│   │   │   ├── Cases/
│   │   │   │   ├── CaseDto.cs
│   │   │   │   ├── CaseListDto.cs
│   │   │   │   ├── CreateCaseDto.cs
│   │   │   │   ├── UpdateCaseDto.cs
│   │   │   │   ├── CaseReadinessDto.cs
│   │   │   │   ├── CaseAccessDto.cs
│   │   │   │   └── GrantAccessDto.cs
│   │   │   ├── HardCopies/
│   │   │   │   ├── HardCopyRequestDto.cs
│   │   │   │   ├── CreateHardCopyRequestDto.cs
│   │   │   │   ├── HandoverDto.cs
│   │   │   │   └── HardCopyTimelineDto.cs
│   │   │   ├── Checklists/
│   │   │   │   ├── ChecklistDto.cs
│   │   │   │   ├── ChecklistItemDto.cs
│   │   │   │   ├── CreateChecklistDto.cs
│   │   │   │   └── UpdateChecklistDto.cs
│   │   │   ├── Notifications/
│   │   │   │   ├── NotificationDto.cs
│   │   │   │   └── NotificationPreferenceDto.cs
│   │   │   ├── Dashboard/
│   │   │   │   ├── DashboardStatsDto.cs
│   │   │   │   ├── DocumentHealthDto.cs
│   │   │   │   └── CaseReadinessWidgetDto.cs
│   │   │   └── Common/
│   │   │       ├── PaginatedResultDto.cs
│   │   │       ├── ApiResponseDto.cs
│   │   │       └── FileUploadResultDto.cs
│   │   ├── Mappings/
│   │   │   └── AutoMapperProfile.cs               # Entity ↔ DTO mappings
│   │   └── Exceptions/
│   │       ├── NotFoundException.cs
│   │       ├── ForbiddenException.cs
│   │       ├── ValidationException.cs
│   │       └── BusinessRuleException.cs
│   │
│   ├── TourDocs.Data/                             # Data Access Layer
│   │   ├── Context/
│   │   │   └── ApplicationDbContext.cs            # EF Core DbContext
│   │   ├── Configurations/                        # EF Core Fluent API
│   │   │   ├── OrganizationConfiguration.cs
│   │   │   ├── OrganizationMemberConfiguration.cs
│   │   │   ├── MemberConfiguration.cs
│   │   │   ├── DocumentConfiguration.cs
│   │   │   ├── DocumentVersionConfiguration.cs
│   │   │   ├── DocumentRequestConfiguration.cs
│   │   │   ├── CaseConfiguration.cs
│   │   │   ├── CaseMemberConfiguration.cs
│   │   │   ├── CaseAccessConfiguration.cs
│   │   │   ├── HardCopyRequestConfiguration.cs
│   │   │   ├── HardCopyHandoverConfiguration.cs
│   │   │   ├── ChecklistConfiguration.cs
│   │   │   ├── ChecklistItemConfiguration.cs
│   │   │   ├── AuditLogConfiguration.cs
│   │   │   ├── NotificationConfiguration.cs
│   │   │   └── SubscriptionConfiguration.cs
│   │   ├── Repositories/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IRepository.cs                 # Generic repository
│   │   │   │   ├── IOrganizationRepository.cs
│   │   │   │   ├── IMemberRepository.cs
│   │   │   │   ├── IDocumentRepository.cs
│   │   │   │   ├── ICaseRepository.cs
│   │   │   │   ├── IChecklistRepository.cs
│   │   │   │   ├── IHardCopyRepository.cs
│   │   │   │   ├── IDocumentRequestRepository.cs
│   │   │   │   ├── IAuditLogRepository.cs
│   │   │   │   ├── INotificationRepository.cs
│   │   │   │   └── ISubscriptionRepository.cs
│   │   │   └── Implementations/
│   │   │       ├── Repository.cs                  # Generic base implementation
│   │   │       ├── OrganizationRepository.cs
│   │   │       ├── MemberRepository.cs
│   │   │       ├── DocumentRepository.cs
│   │   │       ├── CaseRepository.cs
│   │   │       ├── ChecklistRepository.cs
│   │   │       ├── HardCopyRepository.cs
│   │   │       ├── DocumentRequestRepository.cs
│   │   │       ├── AuditLogRepository.cs
│   │   │       ├── NotificationRepository.cs
│   │   │       └── SubscriptionRepository.cs
│   │   ├── UnitOfWork/
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── UnitOfWork.cs
│   │   ├── Migrations/
│   │   ├── Seeders/
│   │   │   ├── CountryChecklistSeeder.cs          # Built-in visa checklists
│   │   │   ├── RoleSeeder.cs                      # Default roles
│   │   │   └── DocumentCategorySeeder.cs          # Default categories
│   │   └── DependencyInjection.cs                 # Data layer DI registration
│   │
│   ├── TourDocs.Domain/                           # Domain Entities & Enums
│   │   ├── Entities/
│   │   │   ├── Organization.cs
│   │   │   ├── OrganizationMember.cs
│   │   │   ├── Member.cs
│   │   │   ├── MemberProfile.cs
│   │   │   ├── TravelHistory.cs
│   │   │   ├── Document.cs
│   │   │   ├── DocumentVersion.cs
│   │   │   ├── DocumentRequest.cs
│   │   │   ├── Case.cs
│   │   │   ├── CaseMember.cs
│   │   │   ├── CaseAccess.cs
│   │   │   ├── HardCopyRequest.cs
│   │   │   ├── HardCopyHandover.cs
│   │   │   ├── Checklist.cs
│   │   │   ├── ChecklistItem.cs
│   │   │   ├── Notification.cs
│   │   │   ├── AuditLog.cs
│   │   │   ├── Subscription.cs
│   │   │   └── ApplicationUser.cs
│   │   ├── Enums/
│   │   │   ├── DocumentCategory.cs               # Identity, Financial, Legal, etc.
│   │   │   ├── DocumentStatus.cs                 # Uploaded, UnderReview, Verified, Rejected, Expired
│   │   │   ├── HardCopyStatus.cs                 # WithMember → ... → ReturnedToMember
│   │   │   ├── DocumentRequestStatus.cs          # Requested, Acknowledged, InProgress, Fulfilled, Declined
│   │   │   ├── CaseStatus.cs                     # Draft, Active, DocsComplete, Submitted, Completed, Cancelled
│   │   │   ├── CaseAccessPermission.cs           # View, ViewDownload, ViewDownloadRequest
│   │   │   ├── UserRole.cs                       # OrgOwner, OrgMember, CaseManager, DocumentHandler, Member
│   │   │   ├── NotificationType.cs
│   │   │   ├── SubscriptionPlan.cs               # Starter, Professional, Enterprise
│   │   │   └── Urgency.cs                        # Low, Normal, High, Critical
│   │   └── Common/
│   │       ├── BaseEntity.cs                      # Id, CreatedAt, UpdatedAt
│   │       ├── AuditableEntity.cs                 # + CreatedBy, UpdatedBy
│   │       └── ISoftDelete.cs                     # IsDeleted, DeletedAt
│   │
│   └── TourDocs.Infrastructure/                   # External Services
│       ├── FileStorage/
│       │   ├── LocalFileStorageService.cs         # Store files in local folder
│       │   └── CloudFileStorageService.cs         # Future: S3/Azure Blob (same interface)
│       ├── Email/
│       │   └── SendGridEmailService.cs
│       ├── WhatsApp/
│       │   └── TwilioWhatsAppService.cs
│       ├── OCR/
│       │   └── OcrService.cs
│       ├── Identity/
│       │   ├── IdentityService.cs
│       │   ├── JwtTokenService.cs
│       │   └── RefreshTokenService.cs
│       ├── BackgroundJobs/
│       │   ├── DocumentExpiryCheckJob.cs          # Daily scan for expiring docs
│       │   ├── AccessExpiryCleanupJob.cs          # Revoke expired case access
│       │   └── NotificationDispatchJob.cs         # Batch send notifications
│       └── DependencyInjection.cs
│
├── tests/
│   ├── TourDocs.Core.Tests/                       # Unit tests for services
│   ├── TourDocs.Data.Tests/                       # Repository integration tests
│   └── TourDocs.API.Tests/                        # Controller + integration tests
│
├── TourDocs.sln
├── docker-compose.yml
└── .editorconfig
```

### 2.2 N-Tier Layer Responsibilities

| Layer | Project | Responsibilities | References |
|-------|---------|-----------------|------------|
| **API** | TourDocs.API | Controllers, middleware, filters, SignalR hubs, request/response handling | Core, Domain |
| **Business Logic** | TourDocs.Core | Services, DTOs, validation, business rules, mappings, orchestration | Data, Domain, Infrastructure interfaces |
| **Data Access** | TourDocs.Data | DbContext, repositories, unit of work, migrations, EF configurations, seeders | Domain |
| **Domain** | TourDocs.Domain | Entities, enums, base classes — **zero dependencies** | None |
| **Infrastructure** | TourDocs.Infrastructure | External services (file storage, email, WhatsApp, OCR, identity) | Domain, Core interfaces |

```
Dependency Flow:

  API → Core → Data → Domain
   │              ↑
   └→ Infrastructure (implements Core interfaces)
```

### 2.3 Local File Storage Strategy

Files are stored on the local file system using a structured folder hierarchy. The `IFileStorageService` abstraction makes it trivial to swap to cloud storage later.

```
/TourDocs.API/Storage/                             # Root storage folder
├── {organizationId}/
│   ├── {memberId}/
│   │   ├── identity/
│   │   │   ├── {documentId}_{version}_passport.pdf
│   │   │   └── {documentId}_{version}_nic.jpg
│   │   ├── financial/
│   │   │   └── {documentId}_{version}_bank_statement.pdf
│   │   ├── legal/
│   │   ├── professional/
│   │   ├── travel/
│   │   ├── medical/
│   │   └── photos/
│   └── ...
└── temp/                                          # Upload staging area
```

**IFileStorageService interface:**
```csharp
public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string folder);
    Task<Stream> DownloadAsync(string filePath);
    Task DeleteAsync(string filePath);
    Task<bool> ExistsAsync(string filePath);
    string GetFileUrl(string filePath);  // For local: relative path, for cloud: pre-signed URL
}
```

Switching to cloud later requires only:
1. Implement `CloudFileStorageService` (same interface)
2. Change one line in DI registration
3. Migrate existing files (one-time script)

---

## 3. Frontend - Angular Project Structure

```
tourdocs-web/
├── src/
│   ├── app/
│   │   ├── core/                                  # Singleton services, guards, interceptors
│   │   │   ├── auth/
│   │   │   │   ├── auth.service.ts
│   │   │   │   ├── auth.guard.ts
│   │   │   │   ├── role.guard.ts
│   │   │   │   ├── jwt.interceptor.ts
│   │   │   │   └── auth.models.ts
│   │   │   ├── services/
│   │   │   │   ├── api.service.ts                 # Base HTTP service
│   │   │   │   ├── notification.service.ts        # SignalR + toast
│   │   │   │   ├── file-upload.service.ts
│   │   │   │   └── signalr.service.ts
│   │   │   ├── interceptors/
│   │   │   │   ├── error.interceptor.ts
│   │   │   │   ├── loading.interceptor.ts
│   │   │   │   └── tenant.interceptor.ts
│   │   │   └── models/
│   │   │       ├── api-response.model.ts
│   │   │       └── pagination.model.ts
│   │   │
│   │   ├── shared/                                # Reusable components
│   │   │   ├── components/
│   │   │   │   ├── page-header/                   # Breadcrumb + title + actions
│   │   │   │   ├── confirm-dialog/                # Reusable confirmation modal
│   │   │   │   ├── file-upload/                   # Drag-drop + camera capture
│   │   │   │   ├── status-badge/                  # Color-coded status chip
│   │   │   │   ├── document-card/                 # Document preview card
│   │   │   │   ├── empty-state/                   # No-data illustration
│   │   │   │   ├── timeline/                      # Vertical timeline (hard copies)
│   │   │   │   ├── stat-card/                     # Dashboard KPI card
│   │   │   │   ├── data-table/                    # Enhanced mat-table wrapper
│   │   │   │   ├── search-input/                  # Debounced search field
│   │   │   │   └── avatar/                        # User/member avatar
│   │   │   ├── directives/
│   │   │   │   ├── has-role.directive.ts           # *hasRole="'OrgOwner'"
│   │   │   │   └── drag-drop.directive.ts
│   │   │   ├── pipes/
│   │   │   │   ├── time-ago.pipe.ts
│   │   │   │   ├── file-size.pipe.ts
│   │   │   │   └── document-status.pipe.ts
│   │   │   └── shared.module.ts
│   │   │
│   │   ├── store/                                 # NgRx Global Store
│   │   │   ├── auth/
│   │   │   │   ├── auth.actions.ts
│   │   │   │   ├── auth.reducer.ts
│   │   │   │   ├── auth.effects.ts
│   │   │   │   └── auth.selectors.ts
│   │   │   ├── notifications/
│   │   │   ├── ui/                                # Sidebar, theme, loading
│   │   │   └── app.state.ts
│   │   │
│   │   ├── layout/                                # App Shell (Primer-style)
│   │   │   ├── layout.component.ts
│   │   │   ├── layout.component.html
│   │   │   ├── layout.component.scss
│   │   │   ├── sidenav/
│   │   │   │   ├── sidenav.component.ts           # Collapsible dark sidebar
│   │   │   │   ├── sidenav.component.html
│   │   │   │   ├── sidenav.component.scss
│   │   │   │   ├── nav-item/
│   │   │   │   └── nav-group/                     # Expandable menu section
│   │   │   ├── toolbar/
│   │   │   │   ├── toolbar.component.ts           # Top app bar
│   │   │   │   ├── toolbar.component.html
│   │   │   │   ├── toolbar.component.scss
│   │   │   │   ├── user-menu/                     # Profile dropdown
│   │   │   │   ├── notification-bell/             # Badge + dropdown
│   │   │   │   └── search-bar/                    # Global search
│   │   │   └── footer/
│   │   │       └── footer.component.ts
│   │   │
│   │   ├── features/                              # Lazy-loaded Feature Modules
│   │   │   │
│   │   │   ├── auth/                              # PUBLIC (no layout shell)
│   │   │   │   ├── login/
│   │   │   │   ├── register/
│   │   │   │   ├── forgot-password/
│   │   │   │   ├── magic-link/                    # Member onboarding via link
│   │   │   │   └── auth.routes.ts
│   │   │   │
│   │   │   ├── dashboard/
│   │   │   │   ├── dashboard.component.ts
│   │   │   │   ├── dashboard.component.html
│   │   │   │   ├── dashboard.component.scss
│   │   │   │   ├── widgets/
│   │   │   │   │   ├── document-health-widget/    # Donut chart by status
│   │   │   │   │   ├── expiring-docs-widget/      # Table of soon-expiring
│   │   │   │   │   ├── case-readiness-widget/     # Bar chart per case
│   │   │   │   │   ├── recent-activity-widget/    # Audit feed
│   │   │   │   │   ├── member-stats-widget/       # Total members, active, etc.
│   │   │   │   │   └── hard-copy-tracker-widget/  # Active hard copy requests
│   │   │   │   └── dashboard.routes.ts
│   │   │   │
│   │   │   ├── members/                           # Member Management
│   │   │   │   ├── member-list/
│   │   │   │   │   ├── member-list.component.ts
│   │   │   │   │   ├── member-list.component.html
│   │   │   │   │   └── member-list.component.scss
│   │   │   │   ├── member-detail/
│   │   │   │   │   ├── member-detail.component.ts
│   │   │   │   │   ├── tabs/
│   │   │   │   │   │   ├── profile-tab/           # Personal + professional info
│   │   │   │   │   │   ├── documents-tab/         # All docs by category
│   │   │   │   │   │   ├── travel-history-tab/    # Past travel records
│   │   │   │   │   │   └── photos-tab/            # ID photos gallery
│   │   │   │   │   └── member-detail.component.html
│   │   │   │   ├── member-form/                   # Create/Edit form
│   │   │   │   ├── member-invite/                 # Send onboarding invite
│   │   │   │   ├── store/                         # Feature NgRx
│   │   │   │   ├── services/
│   │   │   │   │   └── members-api.service.ts
│   │   │   │   └── members.routes.ts
│   │   │   │
│   │   │   ├── documents/                         # Document Management
│   │   │   │   ├── document-vault/                # Filterable vault view
│   │   │   │   ├── document-upload/               # Upload dialog
│   │   │   │   ├── document-viewer/               # Preview/download
│   │   │   │   ├── document-verify/               # Review & verify flow
│   │   │   │   ├── document-requests/             # Incoming request list
│   │   │   │   ├── expiry-tracker/                # Expiring docs view
│   │   │   │   ├── store/
│   │   │   │   ├── services/
│   │   │   │   └── documents.routes.ts
│   │   │   │
│   │   │   ├── cases/                             # Case Management
│   │   │   │   ├── case-list/
│   │   │   │   ├── case-detail/
│   │   │   │   │   ├── case-overview-tab/         # Info, status, destination
│   │   │   │   │   ├── case-members-tab/          # Assigned members + readiness
│   │   │   │   │   ├── case-checklist-tab/        # Doc completion matrix
│   │   │   │   │   ├── case-access-tab/           # External access management
│   │   │   │   │   └── case-audit-tab/            # Case-specific audit trail
│   │   │   │   ├── case-form/                     # Create/edit case
│   │   │   │   ├── access-share-dialog/           # Grant access modal
│   │   │   │   ├── store/
│   │   │   │   ├── services/
│   │   │   │   └── cases.routes.ts
│   │   │   │
│   │   │   ├── hard-copies/                       # Hard Copy Tracking
│   │   │   │   ├── hard-copy-list/
│   │   │   │   ├── hard-copy-request/
│   │   │   │   ├── hard-copy-timeline/            # Visual status timeline
│   │   │   │   ├── handover-dialog/               # OTP/signature confirmation
│   │   │   │   ├── store/
│   │   │   │   └── hard-copies.routes.ts
│   │   │   │
│   │   │   ├── checklists/                        # Document Checklists
│   │   │   │   ├── checklist-list/                # Browse by country/type
│   │   │   │   ├── checklist-detail/
│   │   │   │   ├── checklist-editor/              # Create/edit checklist
│   │   │   │   └── checklists.routes.ts
│   │   │   │
│   │   │   ├── organization/                      # Org Settings
│   │   │   │   ├── org-settings/
│   │   │   │   ├── team-management/
│   │   │   │   ├── subscription/
│   │   │   │   └── organization.routes.ts
│   │   │   │
│   │   │   ├── audit/                             # Audit Log
│   │   │   │   ├── audit-log-list/
│   │   │   │   └── audit.routes.ts
│   │   │   │
│   │   │   ├── notifications/                     # Notification Center
│   │   │   │   ├── notification-list/
│   │   │   │   └── notification-preferences/
│   │   │   │
│   │   │   └── member-portal/                     # Simplified Member Self-Service
│   │   │       ├── my-documents/                  # View & upload own docs
│   │   │       ├── my-cases/                      # Upcoming cases & pending docs
│   │   │       ├── upload-wizard/                 # Guided step-by-step upload
│   │   │       └── member-portal.routes.ts
│   │   │
│   │   ├── app.component.ts
│   │   ├── app.config.ts
│   │   └── app.routes.ts
│   │
│   ├── assets/
│   │   ├── icons/
│   │   ├── images/
│   │   │   ├── logo.svg
│   │   │   ├── logo-compact.svg
│   │   │   └── empty-states/
│   │   └── i18n/
│   │       ├── en.json
│   │       └── si.json
│   │
│   ├── styles/
│   │   ├── _variables.scss
│   │   ├── _theme.scss                            # Angular Material custom theme
│   │   ├── _typography.scss
│   │   ├── _layout.scss                           # Primer-style layout helpers
│   │   ├── _cards.scss
│   │   ├── _tables.scss
│   │   ├── _forms.scss
│   │   ├── _dark-theme.scss
│   │   └── styles.scss
│   │
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   │
│   └── index.html
│
├── angular.json
├── package.json
└── tsconfig.json
```

---

## 4. Database Schema (MS SQL Server)

### 4.1 Entity Relationship Diagram

```
Organizations ─┬── OrganizationMembers ──── ApplicationUsers
               ├── Members ──┬── Documents ──── DocumentVersions
               │             ├── TravelHistory
               │             └── CaseMembers ──── Cases
               ├── Cases ──┬── CaseAccess
               │           └── CaseMembers
               └── Subscriptions

Documents ──── DocumentRequests
Documents ──── HardCopyRequests ──── HardCopyHandovers

Checklists ──── ChecklistItems

AuditLogs (polymorphic — tracks all entities)
Notifications (per-user)
```

### 4.2 Table Definitions

#### Core Tables

```sql
-- =============================================
-- ORGANIZATIONS
-- =============================================
CREATE TABLE [dbo].[Organizations] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Name]              NVARCHAR(200)       NOT NULL,
    [Slug]              NVARCHAR(100)       NOT NULL,
    [BusinessRegNo]     NVARCHAR(50)        NULL,
    [LogoUrl]           NVARCHAR(500)       NULL,
    [Address]           NVARCHAR(500)       NULL,
    [Phone]             NVARCHAR(20)        NULL,
    [Email]             NVARCHAR(255)       NULL,
    [Website]           NVARCHAR(255)       NULL,
    [Industry]          NVARCHAR(100)       NULL,       -- 'entertainment', 'sports', 'education', 'corporate', etc.
    [SubscriptionPlan]  NVARCHAR(20)        NOT NULL DEFAULT 'starter',
    [IsActive]          BIT                 NOT NULL DEFAULT 1,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]         UNIQUEIDENTIFIER    NULL,
    CONSTRAINT [PK_Organizations] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Organizations_Slug] UNIQUE ([Slug])
);

-- =============================================
-- APPLICATION USERS (extends ASP.NET Identity)
-- =============================================
CREATE TABLE [dbo].[ApplicationUsers] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL,
    [FullName]          NVARCHAR(200)       NOT NULL,
    [AvatarUrl]         NVARCHAR(500)       NULL,
    [IsActive]          BIT                 NOT NULL DEFAULT 1,
    [LastLoginAt]       DATETIME2           NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    -- ASP.NET Identity columns (Email, PasswordHash, PhoneNumber, etc.) inherited
    CONSTRAINT [PK_ApplicationUsers] PRIMARY KEY ([Id])
);

-- =============================================
-- ORGANIZATION MEMBERS (users within an org, with role)
-- =============================================
CREATE TABLE [dbo].[OrganizationMembers] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [OrganizationId]    UNIQUEIDENTIFIER    NOT NULL,
    [UserId]            UNIQUEIDENTIFIER    NOT NULL,
    [Role]              NVARCHAR(20)        NOT NULL,   -- 'owner', 'member'
    [InvitedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [JoinedAt]          DATETIME2           NULL,
    [IsActive]          BIT                 NOT NULL DEFAULT 1,
    CONSTRAINT [PK_OrganizationMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrgMembers_Org] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id]),
    CONSTRAINT [FK_OrgMembers_User] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUsers]([Id]),
    CONSTRAINT [UQ_OrgMembers_OrgUser] UNIQUE ([OrganizationId], [UserId])
);

-- =============================================
-- MEMBERS (the people whose documents are managed)
-- Generic: could be an artist, athlete, employee, student, etc.
-- =============================================
CREATE TABLE [dbo].[Members] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [OrganizationId]    UNIQUEIDENTIFIER    NOT NULL,
    [UserId]            UNIQUEIDENTIFIER    NULL,       -- NULL until member self-registers

    -- Personal Details
    [LegalFirstName]    NVARCHAR(100)       NOT NULL,
    [LegalLastName]     NVARCHAR(100)       NOT NULL,
    [DateOfBirth]       DATE                NULL,
    [Nationality]       NVARCHAR(60)        NULL,
    [NicNumber]         NVARCHAR(20)        NULL,
    [PassportNumber]    NVARCHAR(20)        NULL,
    [Phone]             NVARCHAR(20)        NULL,
    [Email]             NVARCHAR(255)       NULL,
    [Address]           NVARCHAR(500)       NULL,

    -- Professional / Contextual Details (flexible per org type)
    [Title]             NVARCHAR(100)       NULL,       -- Job title, stage name, etc.
    [Department]        NVARCHAR(100)       NULL,       -- Department, genre, sport, etc.
    [Specialization]    NVARCHAR(200)       NULL,       -- Skill, instrument, position, etc.
    [ExternalId]        NVARCHAR(50)        NULL,       -- Employee ID, student ID, etc.
    [CustomFields]      NVARCHAR(MAX)       NULL,       -- JSON: flexible additional fields

    -- Metadata
    [ProfilePhotoUrl]   NVARCHAR(500)       NULL,
    [Notes]             NVARCHAR(MAX)       NULL,
    [IsActive]          BIT                 NOT NULL DEFAULT 1,
    [IsDeleted]         BIT                 NOT NULL DEFAULT 0,
    [DeletedAt]         DATETIME2           NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]         UNIQUEIDENTIFIER    NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Members_Org] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id]),
    CONSTRAINT [FK_Members_User] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUsers]([Id])
);

CREATE INDEX [IX_Members_OrgId] ON [Members]([OrganizationId]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Members_Name] ON [Members]([LegalLastName], [LegalFirstName]);

-- =============================================
-- TRAVEL HISTORY
-- =============================================
CREATE TABLE [dbo].[TravelHistory] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [MemberId]          UNIQUEIDENTIFIER    NOT NULL,
    [Country]           NVARCHAR(60)        NOT NULL,
    [VisaType]          NVARCHAR(50)        NULL,
    [EntryDate]         DATE                NULL,
    [ExitDate]          DATE                NULL,
    [Purpose]           NVARCHAR(200)       NULL,
    [Notes]             NVARCHAR(MAX)       NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_TravelHistory] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TravelHistory_Member] FOREIGN KEY ([MemberId]) REFERENCES [Members]([Id])
);
```

#### Document Tables

```sql
-- =============================================
-- DOCUMENTS
-- =============================================
CREATE TABLE [dbo].[Documents] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [MemberId]          UNIQUEIDENTIFIER    NOT NULL,
    [OrganizationId]    UNIQUEIDENTIFIER    NOT NULL,

    [Category]          NVARCHAR(30)        NOT NULL,
    -- 'identity', 'financial', 'legal', 'professional', 'travel', 'medical', 'photos'

    [DocumentType]      NVARCHAR(50)        NOT NULL,
    -- e.g., 'passport', 'nic', 'bank_statement', 'police_clearance'

    [Title]             NVARCHAR(200)       NOT NULL,

    [Status]            NVARCHAR(20)        NOT NULL DEFAULT 'uploaded',
    -- 'uploaded', 'under_review', 'verified', 'rejected', 'expired', 'archived'

    [CurrentVersionId]  UNIQUEIDENTIFIER    NULL,
    [ExpiryDate]        DATE                NULL,
    [IsHardCopyNeeded]  BIT                 NOT NULL DEFAULT 0,

    [ExtractedData]     NVARCHAR(MAX)       NULL,       -- JSON: OCR/MRZ extracted data

    [VerificationNotes] NVARCHAR(MAX)       NULL,
    [VerifiedBy]        UNIQUEIDENTIFIER    NULL,
    [VerifiedAt]        DATETIME2           NULL,

    [IsDeleted]         BIT                 NOT NULL DEFAULT 0,
    [DeletedAt]         DATETIME2           NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]         UNIQUEIDENTIFIER    NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Documents_Member] FOREIGN KEY ([MemberId]) REFERENCES [Members]([Id]),
    CONSTRAINT [FK_Documents_Org] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id]),
    CONSTRAINT [FK_Documents_VerifiedBy] FOREIGN KEY ([VerifiedBy]) REFERENCES [ApplicationUsers]([Id])
);

CREATE INDEX [IX_Documents_MemberId] ON [Documents]([MemberId]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Documents_ExpiryDate] ON [Documents]([ExpiryDate]) WHERE [ExpiryDate] IS NOT NULL AND [IsDeleted] = 0;

-- =============================================
-- DOCUMENT VERSIONS
-- =============================================
CREATE TABLE [dbo].[DocumentVersions] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [DocumentId]        UNIQUEIDENTIFIER    NOT NULL,
    [VersionNumber]     INT                 NOT NULL,

    [FileName]          NVARCHAR(255)       NOT NULL,
    [FilePath]          NVARCHAR(1000)      NOT NULL,   -- Local path or cloud key
    [FileSize]          BIGINT              NOT NULL,
    [MimeType]          NVARCHAR(100)       NOT NULL,
    [Checksum]          NVARCHAR(64)        NULL,       -- SHA-256

    [UploadedBy]        UNIQUEIDENTIFIER    NOT NULL,
    [UploadedAt]        DATETIME2           NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_DocumentVersions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DocVersions_Document] FOREIGN KEY ([DocumentId]) REFERENCES [Documents]([Id]),
    CONSTRAINT [FK_DocVersions_UploadedBy] FOREIGN KEY ([UploadedBy]) REFERENCES [ApplicationUsers]([Id]),
    CONSTRAINT [UQ_DocVersions_DocVersion] UNIQUE ([DocumentId], [VersionNumber])
);

ALTER TABLE [dbo].[Documents]
    ADD CONSTRAINT [FK_Documents_CurrentVersion]
    FOREIGN KEY ([CurrentVersionId]) REFERENCES [DocumentVersions]([Id]);

-- =============================================
-- DOCUMENT REQUESTS
-- =============================================
CREATE TABLE [dbo].[DocumentRequests] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CaseId]            UNIQUEIDENTIFIER    NULL,
    [MemberId]          UNIQUEIDENTIFIER    NOT NULL,
    [RequestedBy]       UNIQUEIDENTIFIER    NOT NULL,

    [DocumentType]      NVARCHAR(50)        NOT NULL,
    [FormatRequirements] NVARCHAR(500)      NULL,
    [Urgency]           NVARCHAR(10)        NOT NULL DEFAULT 'normal',
    [Notes]             NVARCHAR(MAX)       NULL,

    [Status]            NVARCHAR(20)        NOT NULL DEFAULT 'requested',
    -- 'requested', 'acknowledged', 'in_progress', 'fulfilled', 'declined'

    [FulfilledDocumentId] UNIQUEIDENTIFIER  NULL,
    [DeclineReason]     NVARCHAR(500)       NULL,

    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_DocumentRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DocRequests_Case] FOREIGN KEY ([CaseId]) REFERENCES [Cases]([Id]),
    CONSTRAINT [FK_DocRequests_Member] FOREIGN KEY ([MemberId]) REFERENCES [Members]([Id]),
    CONSTRAINT [FK_DocRequests_RequestedBy] FOREIGN KEY ([RequestedBy]) REFERENCES [ApplicationUsers]([Id]),
    CONSTRAINT [FK_DocRequests_FulfilledDoc] FOREIGN KEY ([FulfilledDocumentId]) REFERENCES [Documents]([Id])
);
```

#### Case Tables

```sql
-- =============================================
-- CASES (generic: visa application, tour, work permit, etc.)
-- =============================================
CREATE TABLE [dbo].[Cases] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [OrganizationId]    UNIQUEIDENTIFIER    NOT NULL,

    [Name]              NVARCHAR(200)       NOT NULL,
    [CaseType]          NVARCHAR(50)        NOT NULL,   -- 'visa_application', 'tour', 'work_permit', 'scholarship', custom
    [ReferenceNumber]   NVARCHAR(50)        NULL,       -- External reference

    [DestinationCountry] NVARCHAR(60)       NULL,
    [DestinationCity]   NVARCHAR(100)       NULL,
    [Venue]             NVARCHAR(200)       NULL,
    [StartDate]         DATE                NULL,
    [EndDate]           DATE                NULL,

    [ContactName]       NVARCHAR(200)       NULL,       -- External contact (organizer, embassy, etc.)
    [ContactEmail]      NVARCHAR(255)       NULL,
    [ContactPhone]      NVARCHAR(20)        NULL,

    [ChecklistId]       UNIQUEIDENTIFIER    NULL,

    [Status]            NVARCHAR(20)        NOT NULL DEFAULT 'draft',
    -- 'draft', 'active', 'docs_complete', 'submitted', 'completed', 'cancelled'

    [Description]       NVARCHAR(MAX)       NULL,
    [Notes]             NVARCHAR(MAX)       NULL,

    [IsDeleted]         BIT                 NOT NULL DEFAULT 0,
    [DeletedAt]         DATETIME2           NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]         UNIQUEIDENTIFIER    NULL,
    CONSTRAINT [PK_Cases] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cases_Org] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id]),
    CONSTRAINT [FK_Cases_Checklist] FOREIGN KEY ([ChecklistId]) REFERENCES [Checklists]([Id])
);

CREATE INDEX [IX_Cases_OrgId] ON [Cases]([OrganizationId]) WHERE [IsDeleted] = 0;

-- =============================================
-- CASE MEMBERS (junction: members assigned to a case)
-- =============================================
CREATE TABLE [dbo].[CaseMembers] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CaseId]            UNIQUEIDENTIFIER    NOT NULL,
    [MemberId]          UNIQUEIDENTIFIER    NOT NULL,

    [Status]            NVARCHAR(20)        NOT NULL DEFAULT 'pending',
    -- 'pending', 'documents_ready', 'submitted', 'approved', 'rejected'

    [Notes]             NVARCHAR(MAX)       NULL,
    [AddedAt]           DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_CaseMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CaseMembers_Case] FOREIGN KEY ([CaseId]) REFERENCES [Cases]([Id]),
    CONSTRAINT [FK_CaseMembers_Member] FOREIGN KEY ([MemberId]) REFERENCES [Members]([Id]),
    CONSTRAINT [UQ_CaseMembers_CaseMember] UNIQUE ([CaseId], [MemberId])
);

-- =============================================
-- CASE ACCESS (external party access to a case)
-- =============================================
CREATE TABLE [dbo].[CaseAccess] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CaseId]            UNIQUEIDENTIFIER    NOT NULL,
    [UserId]            UNIQUEIDENTIFIER    NOT NULL,

    [Role]              NVARCHAR(20)        NOT NULL,    -- 'case_manager', 'document_handler'
    [Permission]        NVARCHAR(25)        NOT NULL,    -- 'view', 'view_download', 'view_download_request'

    [GrantedBy]         UNIQUEIDENTIFIER    NOT NULL,
    [GrantedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [ExpiresAt]         DATETIME2           NOT NULL,

    [IsActive]          BIT                 NOT NULL DEFAULT 1,
    [RevokedAt]         DATETIME2           NULL,
    [RevokedBy]         UNIQUEIDENTIFIER    NULL,

    CONSTRAINT [PK_CaseAccess] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CaseAccess_Case] FOREIGN KEY ([CaseId]) REFERENCES [Cases]([Id]),
    CONSTRAINT [FK_CaseAccess_User] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUsers]([Id]),
    CONSTRAINT [FK_CaseAccess_GrantedBy] FOREIGN KEY ([GrantedBy]) REFERENCES [ApplicationUsers]([Id]),
    CONSTRAINT [UQ_CaseAccess_CaseUser] UNIQUE ([CaseId], [UserId])
);
```

#### Hard Copy Tables

```sql
-- =============================================
-- HARD COPY REQUESTS
-- =============================================
CREATE TABLE [dbo].[HardCopyRequests] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [DocumentId]        UNIQUEIDENTIFIER    NOT NULL,
    [CaseId]            UNIQUEIDENTIFIER    NOT NULL,
    [RequestedBy]       UNIQUEIDENTIFIER    NOT NULL,

    [Status]            NVARCHAR(30)        NOT NULL DEFAULT 'requested',
    -- 'requested', 'acknowledged', 'collected_by_manager',
    -- 'handed_to_handler', 'at_embassy', 'returned_to_manager', 'returned_to_member'

    [Urgency]           NVARCHAR(10)        NOT NULL DEFAULT 'normal',
    [Notes]             NVARCHAR(MAX)       NULL,

    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_HardCopyRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HardCopy_Document] FOREIGN KEY ([DocumentId]) REFERENCES [Documents]([Id]),
    CONSTRAINT [FK_HardCopy_Case] FOREIGN KEY ([CaseId]) REFERENCES [Cases]([Id]),
    CONSTRAINT [FK_HardCopy_RequestedBy] FOREIGN KEY ([RequestedBy]) REFERENCES [ApplicationUsers]([Id])
);

-- =============================================
-- HARD COPY HANDOVERS (chain of custody)
-- =============================================
CREATE TABLE [dbo].[HardCopyHandovers] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [HardCopyRequestId] UNIQUEIDENTIFIER   NOT NULL,

    [FromUserId]        UNIQUEIDENTIFIER    NULL,
    [ToUserId]          UNIQUEIDENTIFIER    NULL,
    [FromRole]          NVARCHAR(20)        NULL,       -- 'member', 'manager', 'handler', 'embassy'
    [ToRole]            NVARCHAR(20)        NULL,

    [HandoverType]      NVARCHAR(20)        NOT NULL,   -- 'collection', 'handover', 'return'
    [ConfirmationMethod] NVARCHAR(20)       NULL,       -- 'otp', 'signature', 'photo'
    [ConfirmationData]  NVARCHAR(MAX)       NULL,

    [Notes]             NVARCHAR(MAX)       NULL,
    [RecordedAt]        DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [RecordedBy]        UNIQUEIDENTIFIER    NOT NULL,
    CONSTRAINT [PK_HardCopyHandovers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Handovers_Request] FOREIGN KEY ([HardCopyRequestId]) REFERENCES [HardCopyRequests]([Id]),
    CONSTRAINT [FK_Handovers_RecordedBy] FOREIGN KEY ([RecordedBy]) REFERENCES [ApplicationUsers]([Id])
);
```

#### Supporting Tables

```sql
-- =============================================
-- CHECKLISTS
-- =============================================
CREATE TABLE [dbo].[Checklists] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CountryCode]       NVARCHAR(3)         NULL,       -- NULL for non-country-specific checklists
    [CountryName]       NVARCHAR(60)        NULL,
    [Name]              NVARCHAR(200)       NOT NULL,   -- "Schengen Visa", "UK Work Permit", custom name
    [ChecklistType]     NVARCHAR(50)        NOT NULL,   -- 'visa', 'work_permit', 'tour', 'scholarship', 'custom'
    [Version]           INT                 NOT NULL DEFAULT 1,

    [IsSystem]          BIT                 NOT NULL DEFAULT 0,
    [OrganizationId]    UNIQUEIDENTIFIER    NULL,       -- NULL for system checklists

    [Notes]             NVARCHAR(MAX)       NULL,
    [IsActive]          BIT                 NOT NULL DEFAULT 1,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Checklists] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Checklists_Org] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id])
);

CREATE TABLE [dbo].[ChecklistItems] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ChecklistId]       UNIQUEIDENTIFIER    NOT NULL,

    [DocumentType]      NVARCHAR(50)        NOT NULL,
    [DocumentCategory]  NVARCHAR(30)        NOT NULL,
    [Description]       NVARCHAR(500)       NULL,
    [FormatNotes]       NVARCHAR(500)       NULL,       -- "Must be less than 3 months old"
    [IsRequired]        BIT                 NOT NULL DEFAULT 1,
    [RequiresOriginal]  BIT                 NOT NULL DEFAULT 0,
    [ValidityDays]      INT                 NULL,
    [SortOrder]         INT                 NOT NULL DEFAULT 0,

    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ChecklistItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChecklistItems_Checklist] FOREIGN KEY ([ChecklistId]) REFERENCES [Checklists]([Id])
);

-- =============================================
-- AUDIT LOGS
-- =============================================
CREATE TABLE [dbo].[AuditLogs] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [OrganizationId]    UNIQUEIDENTIFIER    NULL,
    [UserId]            UNIQUEIDENTIFIER    NULL,

    [Action]            NVARCHAR(50)        NOT NULL,
    -- e.g., 'document.uploaded', 'document.downloaded', 'document.verified',
    --       'case.created', 'access.granted', 'hardcopy.handover'

    [EntityType]        NVARCHAR(50)        NOT NULL,
    [EntityId]          UNIQUEIDENTIFIER    NOT NULL,

    [Details]           NVARCHAR(MAX)       NULL,       -- JSON payload
    [IpAddress]         NVARCHAR(45)        NULL,
    [UserAgent]         NVARCHAR(500)       NULL,

    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_AuditLogs_Org] ON [AuditLogs]([OrganizationId], [CreatedAt] DESC);
CREATE INDEX [IX_AuditLogs_Entity] ON [AuditLogs]([EntityType], [EntityId]);

-- =============================================
-- NOTIFICATIONS
-- =============================================
CREATE TABLE [dbo].[Notifications] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId]            UNIQUEIDENTIFIER    NOT NULL,
    [OrganizationId]    UNIQUEIDENTIFIER    NULL,

    [Type]              NVARCHAR(50)        NOT NULL,
    [Title]             NVARCHAR(200)       NOT NULL,
    [Message]           NVARCHAR(MAX)       NOT NULL,

    [EntityType]        NVARCHAR(50)        NULL,
    [EntityId]          UNIQUEIDENTIFIER    NULL,

    [Channel]           NVARCHAR(20)        NOT NULL,   -- 'in_app', 'email', 'whatsapp', 'sms'
    [IsRead]            BIT                 NOT NULL DEFAULT 0,
    [ReadAt]            DATETIME2           NULL,

    [SentAt]            DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_User] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUsers]([Id])
);

CREATE INDEX [IX_Notifications_User] ON [Notifications]([UserId], [IsRead], [SentAt] DESC);

-- =============================================
-- SUBSCRIPTIONS
-- =============================================
CREATE TABLE [dbo].[Subscriptions] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [OrganizationId]    UNIQUEIDENTIFIER    NOT NULL,

    [Plan]              NVARCHAR(20)        NOT NULL DEFAULT 'starter',
    [Status]            NVARCHAR(20)        NOT NULL DEFAULT 'active',

    [MaxMembers]        INT                 NOT NULL,
    [MaxCasesMonthly]   INT                 NOT NULL,
    [MaxExternalUsers]  INT                 NOT NULL,
    [MaxStorageBytes]   BIGINT              NOT NULL,

    [PaymentGatewayCustomerId]      NVARCHAR(100)   NULL,
    [PaymentGatewaySubscriptionId]  NVARCHAR(100)   NULL,

    [CurrentPeriodStart] DATETIME2          NULL,
    [CurrentPeriodEnd]  DATETIME2           NULL,

    [CreatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Subscriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Subscriptions_Org] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id]),
    CONSTRAINT [UQ_Subscriptions_Org] UNIQUE ([OrganizationId])
);
```

---

## 5. API Endpoints

### 5.1 Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/register` | Register organization owner |
| POST | `/api/v1/auth/login` | Login (returns JWT + refresh token) |
| POST | `/api/v1/auth/refresh` | Refresh access token |
| POST | `/api/v1/auth/forgot-password` | Send password reset email |
| POST | `/api/v1/auth/reset-password` | Reset password with token |
| POST | `/api/v1/auth/magic-link` | Generate magic link for member onboarding |
| POST | `/api/v1/auth/magic-link/verify` | Verify magic link token |
| POST | `/api/v1/auth/2fa/enable` | Enable two-factor auth |
| POST | `/api/v1/auth/2fa/verify` | Verify 2FA code |
| GET  | `/api/v1/auth/me` | Get current user profile |
| PUT  | `/api/v1/auth/me` | Update current user profile |

### 5.2 Organizations

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/organizations` | Create organization |
| GET  | `/api/v1/organizations/{id}` | Get organization details |
| PUT  | `/api/v1/organizations/{id}` | Update organization |
| GET  | `/api/v1/organizations/{id}/dashboard` | Dashboard stats |
| GET  | `/api/v1/organizations/{id}/members` | List team members |
| POST | `/api/v1/organizations/{id}/members/invite` | Invite team member |
| DELETE | `/api/v1/organizations/{id}/members/{memberId}` | Remove team member |
| PUT  | `/api/v1/organizations/{id}/members/{memberId}/role` | Change member role |

### 5.3 Members

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET  | `/api/v1/members` | List members (paginated, filterable) |
| POST | `/api/v1/members` | Create member |
| GET  | `/api/v1/members/{id}` | Get member details |
| PUT  | `/api/v1/members/{id}` | Update member |
| DELETE | `/api/v1/members/{id}` | Soft-delete member |
| GET  | `/api/v1/members/{id}/documents` | Get all member documents |
| GET  | `/api/v1/members/{id}/documents/summary` | Document completeness summary |
| GET  | `/api/v1/members/{id}/travel-history` | Get travel history |
| POST | `/api/v1/members/{id}/travel-history` | Add travel record |
| PUT  | `/api/v1/members/{id}/travel-history/{historyId}` | Update travel record |
| DELETE | `/api/v1/members/{id}/travel-history/{historyId}` | Delete travel record |
| POST | `/api/v1/members/{id}/invite` | Send onboarding invite |
| GET  | `/api/v1/members/search?q={query}` | Search members |

### 5.4 Documents

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/documents/upload` | Upload document (multipart) |
| GET  | `/api/v1/documents/{id}` | Get document metadata |
| GET  | `/api/v1/documents/{id}/download` | Download document file |
| GET  | `/api/v1/documents/{id}/versions` | Get version history |
| GET  | `/api/v1/documents/{id}/versions/{versionId}/download` | Download specific version |
| PUT  | `/api/v1/documents/{id}/verify` | Verify document |
| PUT  | `/api/v1/documents/{id}/reject` | Reject with reason |
| POST | `/api/v1/documents/{id}/reupload` | Upload new version |
| DELETE | `/api/v1/documents/{id}` | Archive document |
| GET  | `/api/v1/documents/expiring` | List expiring documents |
| POST | `/api/v1/documents/scan-passport` | OCR/MRZ passport scan |

### 5.5 Document Requests

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/document-requests` | Create document request |
| GET  | `/api/v1/document-requests` | List requests (filtered by role) |
| PUT  | `/api/v1/document-requests/{id}/acknowledge` | Acknowledge |
| PUT  | `/api/v1/document-requests/{id}/fulfill` | Fulfill with document |
| PUT  | `/api/v1/document-requests/{id}/decline` | Decline with reason |

### 5.6 Cases

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET  | `/api/v1/cases` | List cases (paginated, filterable) |
| POST | `/api/v1/cases` | Create case |
| GET  | `/api/v1/cases/{id}` | Get case details |
| PUT  | `/api/v1/cases/{id}` | Update case |
| DELETE | `/api/v1/cases/{id}` | Cancel/delete case |
| POST | `/api/v1/cases/{id}/members` | Assign members to case |
| DELETE | `/api/v1/cases/{id}/members/{memberId}` | Remove member from case |
| GET  | `/api/v1/cases/{id}/readiness` | Per-member document readiness |
| GET  | `/api/v1/cases/{id}/checklist` | Get case checklist |
| POST | `/api/v1/cases/{id}/access` | Grant external access |
| GET  | `/api/v1/cases/{id}/access` | List access grants |
| DELETE | `/api/v1/cases/{id}/access/{accessId}` | Revoke access |
| GET  | `/api/v1/cases/{id}/audit` | Case audit trail |
| GET  | `/api/v1/cases/{id}/download-package` | Download all docs as ZIP |

### 5.7 Hard Copies

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/hard-copies/request` | Request hard copy |
| GET  | `/api/v1/hard-copies` | List hard copy requests |
| GET  | `/api/v1/hard-copies/{id}` | Get request + timeline |
| PUT  | `/api/v1/hard-copies/{id}/acknowledge` | Acknowledge |
| PUT  | `/api/v1/hard-copies/{id}/collect` | Log collection from member |
| PUT  | `/api/v1/hard-copies/{id}/handover` | Record handover |
| PUT  | `/api/v1/hard-copies/{id}/at-embassy` | Mark as at embassy/authority |
| PUT  | `/api/v1/hard-copies/{id}/return-to-manager` | Return to manager |
| PUT  | `/api/v1/hard-copies/{id}/return-to-member` | Return to member (close) |

### 5.8 Checklists

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET  | `/api/v1/checklists` | List all checklists |
| GET  | `/api/v1/checklists/{id}` | Get checklist with items |
| GET  | `/api/v1/checklists/country/{countryCode}` | Get by country |
| GET  | `/api/v1/checklists/type/{type}` | Get by type (visa, work_permit, etc.) |
| POST | `/api/v1/checklists` | Create custom checklist |
| PUT  | `/api/v1/checklists/{id}` | Update checklist |
| POST | `/api/v1/checklists/{id}/clone` | Clone as custom |

### 5.9 Notifications & Audit

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET  | `/api/v1/notifications` | List user notifications |
| PUT  | `/api/v1/notifications/{id}/read` | Mark as read |
| PUT  | `/api/v1/notifications/read-all` | Mark all as read |
| GET  | `/api/v1/notifications/unread-count` | Unread count |
| GET  | `/api/v1/audit-logs` | List audit logs (org-scoped, filterable) |

### 5.10 Dashboard

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET  | `/api/v1/dashboard/stats` | KPI summary |
| GET  | `/api/v1/dashboard/document-health` | Documents by status |
| GET  | `/api/v1/dashboard/expiring-soon` | Top expiring documents |
| GET  | `/api/v1/dashboard/recent-activity` | Recent audit activity |
| GET  | `/api/v1/dashboard/case-readiness` | Upcoming cases + readiness % |

---

## 6. User Roles & Permissions

| Capability | Org Owner | Org Member | Case Manager | Document Handler | Member |
|---|---|---|---|---|---|
| Manage organization settings | Yes | No | No | No | No |
| Add/remove members | Yes | Yes | No | No | No |
| Upload documents | Yes | Yes | No | No | Own only |
| Verify documents | Yes | Yes | No | No | No |
| Create cases | Yes | Yes | Yes | No | No |
| Grant case access | Yes | Yes | No | No | No |
| View case documents | Yes | Yes | Yes | Yes | No |
| Download documents | Yes | Yes | If permitted | Yes | Own only |
| Request additional docs | No | No | Yes | Yes | No |
| Request hard copies | No | No | No | Yes | No |
| Manage hard copy handover | Yes | Yes | No | No | No |
| View audit log | Yes | Limited | No | No | No |

**Case Manager** = external party managing a case (e.g., event organizer, HR coordinator, university admin)
**Document Handler** = external party processing applications (e.g., visa handler, immigration agent, legal team)

---

## 7. UI/UX Design Specification (Primer-Inspired)

### 7.1 Layout Shell

```
┌──────────────────────────────────────────────────────────────┐
│  [=]  TourDocs           [Search...]      [Bell 3] [User v] │  ← mat-toolbar (white/primary)
├──────┬───────────────────────────────────────────────────────┤
│      │                                                       │
│  D   │   Page Header / Breadcrumb                            │
│  A   │   ─────────────────────────────────                   │
│  S   │                                                       │
│  H   │   ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐   │
│  B   │   │ Total   │ │ Docs    │ │Expiring │ │ Active  │   │
│  O   │   │ Members │ │Collected│ │  Soon   │ │  Cases  │   │
│  A   │   │   124   │ │  89%    │ │   7     │ │   3     │   │
│  R   │   └─────────┘ └─────────┘ └─────────┘ └─────────┘   │
│  D   │                                                       │
│      │   ┌───────────────────────┐ ┌─────────────────────┐   │
│  M   │   │  Document Health      │ │  Recent Activity    │   │
│  E   │   │  [Donut Chart]        │ │  ► Doc uploaded     │   │
│  M   │   │                       │ │  ► Case created     │   │
│  B   │   │  Verified: 67%        │ │  ► Access granted   │   │
│  E   │   └───────────────────────┘ └─────────────────────┘   │
│  R   │                                                       │
│  S   │   ┌─────────────────────────────────────────────────┐ │
│      │   │  Expiring Documents                  [View All] │ │
│  C   │   │  ───────────────────────────────────────         │ │
│  A   │   │  J. Perera  │ Passport    │ Apr 15  │ 21 days  │ │
│  S   │   │  K. Silva   │ Police Clr  │ Apr 02  │ 8 days   │ │
│  E   │   │  [< 1 2 3 >]                                   │ │
│  S   │   └─────────────────────────────────────────────────┘ │
│      │                                                       │
├──────┴───────────────────────────────────────────────────────┤
│  (c) 2026 TourDocs                                  v1.0.0  │
└──────────────────────────────────────────────────────────────┘
```

### 7.2 Color System

```scss
// Primary - Deep Blue (Trust, Security)
$td-primary-500:     #1565C0;
$td-primary-700:     #0D47A1;
$td-primary-100:     #BBDEFB;

// Accent - Teal (Action, Progress)
$td-accent-500:      #00897B;
$td-accent-700:      #00695C;
$td-accent-100:      #B2DFDB;

// Warn
$td-warn-500:        #F44336;

// Document Status Colors
$status-uploaded:    #42A5F5;    // Blue
$status-reviewing:   #FFA726;    // Orange
$status-verified:    #66BB6A;    // Green
$status-rejected:    #EF5350;    // Red
$status-expired:     #BDBDBD;    // Gray

// Hard Copy Status Colors
$hc-with-member:     #90CAF9;
$hc-collected:       #FFE082;
$hc-with-handler:    #CE93D8;
$hc-at-authority:    #EF5350;
$hc-returned:        #A5D6A7;

// Sidebar (dark, Primer-style)
$bg-sidebar:         #263238;
$bg-sidebar-hover:   #37474F;
$bg-sidebar-active:  #1565C0;
```

### 7.3 Sidenav Menu

```
┌──────────────────────┐
│  [TourDocs Logo]     │
│  Organization Name   │
├──────────────────────┤
│                      │
│  > Dashboard         │
│                      │
│  > Members           │
│     ├ All Members    │
│     └ Invite Member  │
│                      │
│  > Documents         │
│     ├ Document Vault │
│     ├ Pending Review │
│     ├ Requests       │
│     └ Expiry Tracker │
│                      │
│  > Cases             │
│     ├ All Cases      │
│     └ Create Case    │
│                      │
│  > Checklists        │
│                      │
│  > Hard Copies       │
│                      │
│  > Audit Log         │
│                      │
│  ─────────────────── │
│  > Organization      │
│     ├ Settings       │
│     ├ Team           │
│     └ Subscription   │
│                      │
│  > Notifications     │
│                      │
└──────────────────────┘
```

---

## 8. Implementation Phases

### Phase 1 — MVP (Weeks 1-12)

#### Sprint 1-2: Foundation (Weeks 1-4)
- [ ] Initialize .NET Core 8 solution with N-Tier structure (API, Core, Data, Domain, Infrastructure)
- [ ] Set up MSSQL database with EF Core migrations
- [ ] Implement generic Repository pattern + Unit of Work
- [ ] Configure ASP.NET Core Identity with JWT + refresh tokens
- [ ] Set up local file storage with IFileStorageService
- [ ] Create Angular 18 project with custom Material theme
- [ ] Build Primer-style layout shell (dark sidenav, toolbar, footer)
- [ ] Implement routing with lazy loading
- [ ] Set up NgRx (auth, UI state)
- [ ] Build auth pages (login, register, forgot password)
- [ ] JWT interceptor, auth guard, role guard
- [ ] Global error handling (API exception filter + Angular error interceptor)

#### Sprint 3-4: Members & Documents (Weeks 5-8)
- [ ] Organization CRUD service + API + settings page
- [ ] Team member invite/manage
- [ ] Member CRUD service + API with search/pagination
- [ ] Member list page (card grid + table toggle)
- [ ] Member detail page with tabbed layout (profile, documents, travel history, photos)
- [ ] Member create/edit form with reactive form validation
- [ ] Travel history CRUD
- [ ] Document upload service (local file storage)
- [ ] Document vault UI with category grouping and status badges
- [ ] Drag-and-drop upload dialog (multi-file support)
- [ ] Document status workflow (uploaded → under_review → verified/rejected)
- [ ] Document version history

#### Sprint 5-6: Cases & Dashboard (Weeks 9-12)
- [ ] Case CRUD service + API
- [ ] Case list + detail pages (tabbed: overview, members, checklist, access, audit)
- [ ] Assign members to cases
- [ ] Case readiness calculation (per-member document completeness vs. checklist)
- [ ] Basic access sharing (grant/revoke for external users with permissions)
- [ ] Dashboard API + frontend (KPI stat cards, document health chart, expiring docs table, recent activity)
- [ ] Basic email notifications (SendGrid)
- [ ] Document download

### Phase 2 — Core (Weeks 13-24)

#### Sprint 7-8: Hard Copies & Checklists (Weeks 13-16)
- [ ] Hard copy request service with full status state machine
- [ ] Hard copy timeline UI (customized mat-stepper)
- [ ] Handover recording with OTP/signature confirmation
- [ ] Checklist seed data (Schengen, UK, USA, Canada, Australia, Japan, South Korea, UAE)
- [ ] Checklist management UI (browse, create custom, clone)
- [ ] Auto-checklist assignment based on case destination

#### Sprint 9-10: Alerts & Audit (Weeks 17-20)
- [ ] Hangfire background job: daily expiry scan
- [ ] Configurable expiry alert thresholds (90/60/30/14 days)
- [ ] Expiry tracker page with urgency filters
- [ ] Comprehensive audit logging via API action filter
- [ ] Audit log viewer with search/filter/export
- [ ] SignalR real-time notifications
- [ ] Notification center (bell dropdown + full page)
- [ ] WhatsApp notification integration

#### Sprint 11-12: Requests & Polish (Weeks 21-24)
- [ ] Document request workflow (request → acknowledge → fulfill/decline)
- [ ] Document request UI for external users
- [ ] Request fulfillment flow for org managers
- [ ] Case document package download (ZIP)
- [ ] Dashboard advanced widgets (case readiness bar chart, hard copy tracker)
- [ ] Dark theme
- [ ] Performance optimization (virtual scroll, lazy loading audit)

### Phase 3 — Growth (Weeks 25-36)

#### Sprint 13-14: Member Portal (Weeks 25-28)
- [ ] Magic link generation + verification
- [ ] Member self-service portal (separate simplified layout)
- [ ] "My Documents" with guided upload wizard
- [ ] "My Cases" showing upcoming cases + pending documents
- [ ] WhatsApp invite flow

#### Sprint 15-16: OCR & Analytics (Weeks 29-32)
- [ ] MRZ/OCR passport scanning
- [ ] Auto-fill member profile from passport scan
- [ ] Analytics dashboard (turnaround times, success rates)
- [ ] Bulk document download
- [ ] Document format validation (file type, size, dimensions)

#### Sprint 17-18: Mobile & i18n (Weeks 33-36)
- [ ] Responsive optimization for tablet/mobile
- [ ] PWA support
- [ ] Multi-language support (Sinhala, Tamil)
- [ ] Subscription management + payment integration

### Phase 4 — Scale (Weeks 37-48)
- [ ] Cloud file storage migration (swap IFileStorageService implementation)
- [ ] Appointment/submission tracking
- [ ] Courier integration for hard copies
- [ ] Public API for third-party integrations
- [ ] Advanced reporting and CSV/PDF export
- [ ] Admin super-panel for system checklists
- [ ] Rate limiting and abuse protection

---

## 9. Key Technical Decisions

### 9.1 Why N-Tier over CQRS?
- Simpler to understand and onboard new developers
- Straightforward service → repository → database flow
- Less boilerplate than command/query handlers
- Sufficient for the current complexity level
- Can evolve to CQRS later if read/write scaling needs diverge

### 9.2 Why MSSQL?
- Excellent EF Core support with mature tooling
- Strong ecosystem on Windows development environments
- SSMS for database management and debugging
- JSON column support via NVARCHAR(MAX) for flexible fields
- Familiar to .NET development teams

### 9.3 Why Local File Storage First?
- Zero cloud cost during development and early users
- IFileStorageService abstraction makes cloud migration a single DI swap
- Faster development iteration (no cloud config/credentials needed)
- Suitable for MVP validation before committing to cloud spend

### 9.4 Generic Domain Design
- `Member` instead of `Artist` — works for any person whose documents need managing
- `Case` instead of `Event` — works for any scenario requiring a document set
- `CustomFields` (JSON) on Member — organizations can store domain-specific data
- `CaseType` and `ChecklistType` — configurable per organization's use case
- No hard-coded business logic tied to a specific industry

---

## 10. Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| **Performance** | API response < 200ms (p95), page load < 2s |
| **Availability** | 99.5% uptime SLA |
| **Security** | OWASP Top 10, AES-256 file encryption, 2FA, RBAC at API level |
| **Scalability** | Support 500 orgs, 10,000 members, 100,000 documents |
| **Backup** | Daily MSSQL backups, 30-day retention |
| **Monitoring** | Serilog structured logging, health checks |
| **Browser Support** | Chrome, Edge, Firefox, Safari (latest 2 versions) |
| **Accessibility** | WCAG 2.1 AA compliance |
| **File Storage** | Local first, cloud-ready via IFileStorageService abstraction |

---

## 11. Development Environment Setup

### Prerequisites
```
- .NET 8 SDK
- Node.js 20 LTS
- Angular CLI 18
- SQL Server 2022 (or SQL Server Express / LocalDB for dev)
- Redis 7 (optional for dev, required for prod)
- Docker Desktop (optional)
```

### Quick Start
```bash
# Backend
cd TourDocs/src/TourDocs.API
dotnet restore
dotnet ef database update --project ../TourDocs.Data
dotnet run

# Frontend
cd tourdocs-web
npm install
ng serve
```

### Connection String (Development)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TourDocsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "FileStorage": {
    "Provider": "Local",
    "LocalPath": "Storage"
  }
}
```

---

*This document is the single source of truth for the TourDocs project. All implementation decisions should reference this plan.*

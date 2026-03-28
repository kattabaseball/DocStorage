# TourDocs - Project Intelligence

## Project Overview

TourDocs is a SaaS platform for centralized document management with role-based access, version control, expiry tracking, and hard copy chain-of-custody workflows. It serves any organization managing people's documents — artist agencies, HR departments, universities, sports agencies, immigration consultancies.

The platform is designed as a multi-tenant system where each organization operates in complete isolation. Documents flow through a defined lifecycle from upload through verification, with automatic expiry detection. Physical documents are tracked through a chain-of-custody workflow that ensures accountability at every handoff.

## Architecture

- **Backend:** .NET Core 8 Web API with N-Tier architecture (API -> Core -> Data -> Domain + Infrastructure)
- **Frontend:** Angular 18 + Angular Material 18 + NgRx + SCSS
- **Database:** Microsoft SQL Server 2022
- **File Storage:** Local file system (IFileStorageService abstraction for future cloud migration)
- **Real-time:** SignalR for live notifications and document status updates
- **Background Jobs:** Hangfire for scheduled tasks (expiry checks, report generation, cleanup)
- **Caching:** Redis for session state and frequently accessed data
- **Logging:** Serilog with Seq sink for structured logging
- **API Documentation:** Swagger / Swashbuckle

## Project Structure

```
TourDocs/
├── backend/                         # .NET Core solution
│   └── src/
│       ├── TourDocs.API/            # Controllers, middleware, hubs, filters
│       │   ├── Controllers/         # API controllers (thin, delegation only)
│       │   ├── Middleware/           # TenantMiddleware, ExceptionMiddleware, RequestLoggingMiddleware
│       │   ├── Hubs/                # SignalR hubs (NotificationHub, DocumentHub)
│       │   ├── Filters/             # Action filters, authorization filters
│       │   └── Extensions/          # ServiceCollection extensions for DI registration
│       ├── TourDocs.Core/           # Business logic layer
│       │   ├── Services/            # Business logic services
│       │   ├── DTOs/                # Data transfer objects (request/response)
│       │   ├── Interfaces/          # Service and repository contracts
│       │   ├── Mapping/             # AutoMapper profiles
│       │   ├── Validators/          # FluentValidation validators
│       │   └── Exceptions/          # Custom exception types
│       ├── TourDocs.Data/           # Data access layer
│       │   ├── Context/             # ApplicationDbContext
│       │   ├── Configurations/      # EF Core entity configurations (Fluent API)
│       │   ├── Repositories/        # Repository implementations
│       │   ├── Migrations/          # EF Core migrations
│       │   └── UnitOfWork/          # UnitOfWork implementation
│       ├── TourDocs.Domain/         # Domain entities (ZERO external dependencies)
│       │   ├── Entities/            # Entity classes
│       │   ├── Enums/               # Domain enumerations
│       │   ├── Common/              # BaseEntity, AuditableEntity, ISoftDelete
│       │   └── Constants/           # Domain constants
│       └── TourDocs.Infrastructure/ # External concerns
│           ├── FileStorage/         # IFileStorageService implementations
│           ├── Email/               # IEmailService implementations
│           ├── Identity/            # JWT generation, token management
│           └── BackgroundJobs/      # Hangfire job definitions
├── frontend/
│   └── tourdocs-web/                # Angular 18 SPA
│       └── src/
│           ├── app/
│           │   ├── core/            # Singleton services, guards, interceptors
│           │   ├── shared/          # Shared components, directives, pipes
│           │   ├── features/        # Feature modules (lazy-loaded)
│           │   │   ├── auth/
│           │   │   ├── dashboard/
│           │   │   ├── members/
│           │   │   ├── documents/
│           │   │   ├── cases/
│           │   │   ├── checklists/
│           │   │   ├── hard-copies/
│           │   │   ├── reports/
│           │   │   ├── settings/
│           │   │   └── admin/
│           │   ├── layout/          # Shell, sidebar, toolbar, footer
│           │   └── store/           # Root NgRx store (auth, UI, notifications)
│           ├── assets/
│           ├── environments/
│           └── styles/              # Global SCSS (_variables, _mixins, _theme)
└── docs/                            # Documentation
    ├── API.md
    ├── SETUP.md
    ├── ARCHITECTURE.md
    ├── DEPLOYMENT.md
    └── CONTRIBUTING.md
```

## Key Domain Concepts

### Organization
Top-level tenant entity. All data belongs to exactly one organization. Multi-tenancy is enforced via `organization_id` filtering at the data layer. Each organization has its own settings, branding, and user base.

### Member
Any person whose documents are managed. This is deliberately generic — NOT "artist" or "employee" or "student." A member belongs to one organization and has a profile with contact details, categories, tags, and associated documents.

### Case
Any scenario requiring a collection of documents. This is deliberately generic — NOT "event" or "application" or "project." A case has a type, status, deadline, assigned members, and a checklist of required documents.

### Document
A versioned file with metadata, status workflow, and optional expiry date. Documents belong to a member and optionally to a case. Each upload creates a new version while preserving history.

### Checklist
A template defining which document types are required for a specific case type. Checklists are reusable and can be assigned to cases. Progress is tracked per member per case.

### Hard Copy
A physical document with chain-of-custody tracking. Hard copies move through a defined workflow of handoffs between member, manager, handler, and authority, with each transition logged.

### Audit Log
Every significant action (document view, download, status change, login) is recorded with timestamp, user, IP address, and details. Audit logs are immutable and never soft-deleted.

## Coding Standards

### Backend (.NET)

#### Architecture Rules
- N-Tier pattern: Controller -> Service -> Repository -> DbContext
- All business logic lives in `TourDocs.Core` services, NEVER in controllers
- Controllers should be thin: validate input, call service, return result
- Use `IUnitOfWork` for transactions spanning multiple repositories
- Domain project (`TourDocs.Domain`) has ZERO external NuGet dependencies
- Infrastructure project handles all external integrations (files, email, identity)

#### Entity Rules
- All entities inherit from `BaseEntity` (Id, CreatedAt, CreatedBy) or `AuditableEntity` (adds UpdatedAt, UpdatedBy)
- Soft delete via `ISoftDelete` interface (IsDeleted, DeletedAt, DeletedBy properties)
- Global query filter in DbContext automatically excludes soft-deleted records
- Use `IgnoreQueryFilters()` only when explicitly querying deleted records (e.g., admin restore)

#### Data Transfer
- Use AutoMapper for entity-to-DTO mapping; never expose entities directly to API consumers
- DTOs are split: `CreateXxxRequest`, `UpdateXxxRequest`, `XxxResponse`, `XxxListResponse`
- Use FluentValidation for all request DTOs; validators are auto-registered via assembly scanning
- Pagination uses `PagedRequest` (PageNumber, PageSize, SortBy, SortDirection, SearchTerm) and returns `PagedResponse<T>`

#### Error Handling
- Custom exceptions: `NotFoundException`, `ForbiddenException`, `ValidationException`, `BusinessRuleException`, `ConflictException`
- `ExceptionMiddleware` catches all exceptions and returns consistent `ApiResponse<T>` with appropriate HTTP status codes
- Never throw generic `Exception` — always use a typed custom exception
- Log exceptions with Serilog structured logging including correlation ID

#### API Conventions
- All responses wrapped in `ApiResponse<T>` with `Success`, `Message`, `Data`, `Errors` properties
- All endpoints versioned under `/api/v1/`
- RESTful naming: plural nouns, no verbs (e.g., `/api/v1/members`, not `/api/v1/getMember`)
- Use `[ProducesResponseType]` attributes on all controller actions for Swagger documentation
- Use async/await throughout; never use `.Result` or `.Wait()`
- Nullable reference types enabled project-wide (`<Nullable>enable</Nullable>`)

#### Naming Conventions
- Interfaces prefixed with `I` (e.g., `IMemberService`, `IMemberRepository`)
- Async methods suffixed with `Async` (e.g., `GetByIdAsync`, `CreateAsync`)
- Private fields prefixed with `_` (e.g., `_memberService`, `_logger`)
- Constants in `PascalCase` (e.g., `MaxFileSize`, `DefaultPageSize`)

### Frontend (Angular)

#### Component Architecture
- Standalone components exclusively (no NgModules except SharedModule for legacy compatibility)
- New control flow syntax: `@if`, `@for`, `@switch` — NEVER `*ngIf`, `*ngFor`, `*ngSwitch`
- `inject()` function preferred over constructor injection for services
- Smart (container) components handle data fetching; dumb (presentational) components receive data via `@Input` / `@Output`
- Signals preferred for local component state where appropriate

#### State Management
- NgRx for global state: auth, UI preferences, notifications
- NgRx for feature state: members, documents, cases (each feature has its own store slice)
- Pattern: Actions -> Effects -> Reducer -> Selectors
- Effects handle all side effects (API calls, navigation, toast messages)
- Selectors for all store reads — never access store state directly

#### Routing
- Feature modules lazy-loaded via `loadChildren` in `app.routes.ts`
- Guards: `authGuard` (functional guard, checks JWT), `roleGuard` (checks user role), `unsavedChangesGuard`
- Route data includes `title`, `breadcrumb`, `requiredRole`

#### Forms
- Reactive forms with `FormBuilder` for ALL forms — never template-driven forms
- Validation messages displayed via shared `ValidationMessageComponent`
- Custom validators in `shared/validators/` (e.g., `fileSize`, `fileType`, `dateRange`)
- Forms emit via `(ngSubmit)` — never bind submit to a button click

#### Styling
- SCSS with BEM methodology (block__element--modifier)
- Global variables in `_variables.scss` (colors, spacing, breakpoints, shadows)
- Primer-style layout: dark sidebar (`#263238`), white toolbar, card-based content area
- Angular Material theme customized in `_theme.scss`
- Responsive breakpoints: mobile (< 768px), tablet (768px-1024px), desktop (> 1024px)

#### API Integration
- Feature-specific API services extend `BaseApiService` (provides get, post, put, delete helpers)
- `JwtInterceptor` attaches bearer token to all outgoing requests
- `ErrorInterceptor` handles 401 (trigger refresh), 403 (redirect), 500 (toast error)
- API URLs configured via `environment.ts` — never hardcoded

## Important Patterns

### Multi-Tenancy
- Organization ID is stored in JWT claims as `org_id`
- `TenantMiddleware` runs early in the pipeline, extracts organization context, and sets `ITenantContext`
- All repository queries automatically filter by `organization_id` via a base repository method
- Global EF Core query filter handles soft deletes independently
- Admin/superadmin users can access cross-tenant data via explicit scope override
- Tenant isolation is tested: integration tests verify one tenant cannot access another's data

### File Storage
- `IFileStorageService` abstraction allows swapping `LocalFileStorageService` for `AzureBlobStorageService` or `S3StorageService`
- Files stored at: `Storage/{orgId}/{memberId}/{category}/{docId}_{version}_{filename}`
- Original filenames preserved in database metadata; physical files use sanitized names
- File paths NEVER exposed in API responses — access via download endpoint with authorization check
- Virus scanning hook point: `IFileScanner` interface (not yet implemented, logs warning)
- Maximum file size: 50MB (configurable via `appsettings.json`)
- Allowed extensions configurable per organization

### Authentication Flow
1. User submits credentials to `/api/v1/auth/login`
2. Backend validates, generates JWT access token (15 min) + refresh token (7 days, stored in DB)
3. Frontend stores tokens in `localStorage`
4. `JwtInterceptor` attaches `Authorization: Bearer {token}` to all API calls
5. On 401 response, interceptor calls `/api/v1/auth/refresh` with refresh token
6. If refresh succeeds, original request is retried; if refresh fails, user is logged out
7. Role-based authorization: backend uses `[Authorize(Roles = "Admin,Manager")]`, frontend uses `roleGuard`

### Roles
- **SuperAdmin** — Platform-level administration (cross-tenant)
- **Admin** — Organization administrator (full org access)
- **Manager** — Manages members and their documents
- **Handler** — Processes cases and document verification
- **Viewer** — Read-only access to assigned members/cases

### Document Status Workflow
```
Uploaded -> UnderReview -> Verified -> [Expired] (automatic via Hangfire job)
                       -> Rejected -> (member re-uploads -> Uploaded)
```
- Status transitions enforced in `DocumentService` — only valid transitions allowed
- Each transition logged in `DocumentStatusHistory` table
- Expiry check runs daily via Hangfire recurring job
- Expired documents trigger notifications to assigned manager

### Hard Copy Status Flow
```
WithMember -> Requested -> Acknowledged -> CollectedByManager
          -> HandedToHandler -> AtAuthority -> ReturnedToManager -> ReturnedToMember
```
- Each transition requires the acting user to be in the correct role
- Chain-of-custody creates immutable `HardCopyTransfer` records
- Transfers include timestamp, from_user, to_user, notes, optional signature reference

### Real-Time Notifications
- SignalR `NotificationHub` pushes events to connected clients
- Events: document status changed, new document uploaded, hard copy transfer, case deadline approaching
- Users join SignalR groups by organization ID and user ID
- Notification history persisted in database for offline users

## Testing

### Backend
- **Framework:** xUnit + Moq + FluentAssertions
- **Unit tests:** Service methods, validators, utility functions
- **Integration tests:** Repository queries against in-memory SQL (or test container)
- **Test naming:** `MethodName_Scenario_ExpectedBehavior` (e.g., `GetByIdAsync_NonExistentId_ThrowsNotFoundException`)
- **Test data:** Use builder pattern (`MemberBuilder`, `DocumentBuilder`) for test entity construction
- **Coverage target:** 80% for Core project, 60% for API project

### Frontend
- **Unit tests:** Jasmine + Karma — test components, services, pipes, guards
- **E2E tests:** Cypress — test critical user flows (login, upload document, verify, case workflow)
- **Test naming:** `should [expected behavior] when [scenario]`
- **Component tests:** Use `TestBed` with mocked services; test template rendering and user interactions
- **Store tests:** Test reducers as pure functions; test effects with mocked actions and services

## Build and Run

### Prerequisites
- .NET 8 SDK
- Node.js 20 LTS + npm 10
- SQL Server 2022 (or Docker)
- Redis (optional, for caching)

### Backend
```bash
cd backend/src/TourDocs.API
dotnet restore
dotnet ef database update --project ../TourDocs.Data
dotnet run
# API available at https://localhost:7001
# Swagger at https://localhost:7001/swagger
```

### Frontend
```bash
cd frontend/tourdocs-web
npm ci
ng serve
# App available at http://localhost:4200
```

### Docker (Full Stack)
```bash
docker-compose up -d
# API: http://localhost:7001
# Web: http://localhost:4200
# Seq: http://localhost:5341
# Hangfire Dashboard: http://localhost:7001/hangfire
```

## Environment Variables

### Backend (Critical)
- `ConnectionStrings__DefaultConnection` — SQL Server connection string
- `Jwt__Secret` — JWT signing key (minimum 256 bits)
- `Jwt__Issuer` — JWT issuer URL
- `Jwt__Audience` — JWT audience URL
- `FileStorage__BasePath` — Root path for file storage
- `Redis__ConnectionString` — Redis connection string
- `Email__SendGridApiKey` — SendGrid API key for transactional emails
- `Seq__ServerUrl` — Seq server URL for structured logging

### Frontend
- `API_URL` — Backend API base URL
- `SIGNALR_URL` — SignalR hub URL

## Common Mistakes to Avoid

1. **Don't put business logic in controllers.** Controllers validate input, call a service method, and return the result. All business rules, authorization checks beyond role attributes, and data orchestration belong in services.

2. **Don't access DbContext directly from controllers.** Always go through repositories via `IUnitOfWork`. The repository layer provides the query abstraction and ensures consistent filtering.

3. **Don't use `*ngIf` or `*ngFor` in templates.** Use the new control flow syntax: `@if (condition) { }`, `@for (item of items; track item.id) { }`.

4. **Don't create new NgModules for features.** Use standalone components with lazy-loaded routes. The only module is `SharedModule` for backward compatibility.

5. **Don't hardcode "artist" or "event" terminology.** The domain is generic: use "member" and "case." The UI may display organization-specific labels, but code always uses generic terms.

6. **Don't store files without the `IFileStorageService` abstraction.** Never use `System.IO.File` directly in services. Always inject and use the storage service interface.

7. **Don't skip audit logging.** All document access (view, download, print) must be logged. Use `IAuditService.LogAsync()` in service methods.

8. **Don't forget multi-tenancy filtering.** Every query must be scoped to the current organization. The base repository handles this, but custom queries must include the filter.

9. **Don't commit `appsettings.Development.json` or `.env` files.** These contain secrets. Use `.env.example` as a template.

10. **Don't use `.Result` or `.Wait()` on async methods.** This causes deadlocks. Use `await` throughout the call chain.

11. **Don't return entities from API endpoints.** Always map to DTOs using AutoMapper. Entities may contain navigation properties or internal fields that should not be exposed.

12. **Don't skip validation.** Every request DTO must have a corresponding FluentValidation validator. Validators are auto-discovered and run before the controller action.

## Git Workflow

- **main** — Production-ready code, protected branch
- **develop** — Integration branch for next release
- **feature/TOUR-{id}-{description}** — Feature branches off develop
- **bugfix/TOUR-{id}-{description}** — Bug fix branches off develop
- **hotfix/TOUR-{id}-{description}** — Hotfix branches off main
- **release/v{major}.{minor}.{patch}** — Release preparation branches

Commit messages follow Conventional Commits:
```
feat(members): add bulk import from CSV
fix(documents): resolve version conflict on concurrent upload
chore(deps): update Angular to 18.2
docs(api): add pagination examples to Swagger
```

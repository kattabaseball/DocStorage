# TourDocs - Architecture Decision Records

## Overview

This document records the key architectural decisions made for the TourDocs platform, including the context, decision, and consequences of each choice.

---

## ADR-001: N-Tier Architecture over CQRS

### Context

We needed to choose an application architecture that balances complexity, maintainability, and team familiarity. The primary candidates were:

1. **N-Tier (Layered):** Controller -> Service -> Repository -> Database
2. **CQRS + MediatR:** Separate command and query models with a mediator pattern
3. **Clean Architecture:** Onion/hexagonal with domain at the center

### Decision

We chose **N-Tier architecture** with clearly defined layers: API, Core (Business Logic), Data (Persistence), Domain (Entities), and Infrastructure (External Services).

### Rationale

- **Team familiarity:** N-Tier is the most widely understood pattern in .NET teams, reducing onboarding time.
- **Appropriate complexity:** TourDocs is a document management system, not a complex domain with intricate business rules requiring DDD tactical patterns.
- **Predictable flow:** Request follows a clear, linear path through layers, making debugging straightforward.
- **CQRS overhead:** Separate read/write models add significant complexity with minimal benefit for our read/write ratio and consistency requirements.
- **Future migration path:** If specific features require CQRS (e.g., complex reporting), it can be introduced for those features without rewriting the entire system.

### Consequences

- Business logic must be strictly contained in the Core/Service layer
- Controllers must remain thin (no business logic)
- Data access must go through repositories (no direct DbContext in services)
- Cross-cutting concerns (logging, caching, authorization) are handled via middleware and attributes

---

## ADR-002: Microsoft SQL Server over PostgreSQL

### Context

We needed a relational database that supports multi-tenancy, full-text search, and integrates well with the .NET ecosystem.

### Decision

We chose **Microsoft SQL Server 2022 Developer Edition** for development and **Standard/Enterprise** for production.

### Rationale

- **EF Core integration:** SQL Server is the most mature and best-tested EF Core provider with the fewest edge cases.
- **Tooling:** SQL Server Management Studio, Azure Data Studio, and Visual Studio profiler provide excellent development tools.
- **Full-text search:** Built-in full-text search for document metadata queries without additional infrastructure.
- **JSON support:** `FOR JSON` and `OPENJSON` for flexible schema scenarios (e.g., document custom fields).
- **Temporal tables:** Built-in temporal tables can complement our audit logging for entity history.
- **Cloud migration path:** Direct upgrade path to Azure SQL Database when cloud migration occurs.
- **Enterprise familiarity:** Most enterprise clients already have SQL Server infrastructure and DBA expertise.

### Consequences

- Higher licensing cost in production compared to PostgreSQL (mitigated by Developer Edition for dev)
- SQL Server-specific features (temporal tables, CROSS APPLY) may create vendor lock-in
- Docker image is larger than PostgreSQL (1.5GB vs 200MB)

---

## ADR-003: Local File Storage with Abstraction Layer

### Context

Documents are the core asset of the platform. We needed a file storage strategy that works immediately while allowing future cloud migration.

### Decision

We implemented an **`IFileStorageService` interface** with a **`LocalFileStorageService`** implementation that stores files on the local file system.

### Rationale

- **Simplicity:** Local file storage requires no additional infrastructure or cloud accounts for development.
- **Cost:** Zero storage cost during development and early production.
- **Abstraction:** The `IFileStorageService` interface means swapping to Azure Blob, AWS S3, or MinIO requires only a new implementation class and a DI registration change.
- **Control:** Local storage gives full control over file structure, naming, and access patterns.
- **Performance:** No network latency for file operations during development.

### Storage Path Convention

```
Storage/{organizationId}/{memberId}/{category}/{documentId}_{version}_{sanitizedFilename}
```

### Migration Path to Cloud

1. Create `AzureBlobStorageService` or `S3StorageService` implementing `IFileStorageService`
2. Change DI registration from `LocalFileStorageService` to the cloud implementation
3. Run a one-time migration script to upload existing files to cloud storage
4. No changes needed in services, controllers, or any other code

### Consequences

- Local storage does not provide built-in redundancy (mitigated by backup strategy)
- File serving bypasses CDN benefits (acceptable for authenticated document access)
- Must ensure file system permissions are correctly configured in production
- Storage path must never be exposed in API responses (security requirement)

---

## ADR-004: Generic Domain Model

### Context

TourDocs was initially conceived for artist agencies but needed to serve a broader market: HR departments, universities, sports agencies, immigration consultancies, and any organization managing people's documents.

### Decision

We use **generic terminology** throughout the domain model:

| Generic Term | Instead of          | Represents                                  |
|-------------|---------------------|---------------------------------------------|
| Member      | Artist, Employee    | Any person whose documents are managed      |
| Case        | Event, Application  | Any scenario requiring documents            |
| Organization| Agency, Company     | Any entity that uses the platform           |
| Handler     | Agent, Officer      | Any user who processes cases                |

### Rationale

- **Market flexibility:** A single codebase serves multiple industries without fork or customization.
- **Reduced cognitive load:** Developers reason about generic concepts, not industry-specific jargon.
- **UI customization:** Organization settings can define display labels ("Artist" instead of "Member") without code changes.
- **Simpler onboarding:** New team members do not need industry-specific knowledge to contribute.

### Consequences

- UI labels must be configurable per organization (stored in org settings)
- Documentation must consistently use generic terms
- Feature requests from specific industries must be evaluated for generic applicability
- Some industry-specific features may require compromise in abstraction

---

## ADR-005: Multi-Tenancy via Row-Level Filtering

### Context

TourDocs is a multi-tenant SaaS platform. We needed an isolation strategy that balances security, cost, and operational complexity.

### Approaches Considered

1. **Database per tenant:** Complete isolation, highest cost, most operational overhead.
2. **Schema per tenant:** Good isolation, moderate cost, complex migrations.
3. **Row-level filtering:** Shared database and schema, lowest cost, simplest operations.

### Decision

We chose **row-level filtering** where all tenants share the same database and tables, with an `organization_id` column on every tenant-scoped table.

### Implementation

- `TenantMiddleware` extracts `org_id` from the JWT token and sets `ITenantContext.OrganizationId`
- Base repository methods automatically include `WHERE organization_id = @orgId` in all queries
- EF Core global query filters handle soft-delete filtering (separate concern)
- Service methods verify entity ownership before allowing operations
- Direct SQL queries (if any) must include the tenant filter

### Rationale

- **Cost efficiency:** Single database instance serves all tenants, minimizing infrastructure cost.
- **Operational simplicity:** One schema to migrate, one backup to manage, one connection pool.
- **Development speed:** No database provisioning workflow needed for new tenants.
- **Sufficient isolation:** For document management (not financial or healthcare data), row-level isolation with proper authorization is adequate.

### Consequences

- Every tenant-scoped query MUST filter by organization_id (enforced via base repository)
- Performance may degrade with very large tenants (mitigated by proper indexing)
- A bug in filtering could expose data across tenants (mitigated by testing and code review)
- No tenant can have custom schema modifications
- Noisy neighbor problem possible (mitigated by rate limiting and resource quotas)

---

## ADR-006: JWT Authentication with Refresh Tokens

### Context

We needed a stateless authentication mechanism suitable for a SPA frontend communicating with a REST API.

### Decision

We use **JWT access tokens** (short-lived, 15 minutes) combined with **refresh tokens** (long-lived, 7 days, stored in database).

### Flow

1. User authenticates with email/password
2. Server returns JWT access token + refresh token
3. Frontend stores both in localStorage
4. Access token attached to every API request via `Authorization: Bearer` header
5. On 401 response, frontend uses refresh token to obtain new access token
6. Refresh tokens are single-use (rotated on each refresh)
7. Logout revokes the refresh token in the database

### Rationale

- **Stateless requests:** JWT allows the API to validate requests without database lookups for every request.
- **Short access token lifetime:** Limits damage window if a token is compromised.
- **Refresh token rotation:** Single-use refresh tokens detect token theft (reuse triggers full logout).
- **Industry standard:** JWT is the de facto standard for SPA authentication.

### Security Measures

- JWT signed with HMAC-SHA256 (symmetric key, suitable for single-API architecture)
- Refresh tokens stored as hashed values in the database
- Refresh token reuse detection (if a used token is presented, all user tokens are revoked)
- Rate limiting on authentication endpoints (10 requests/minute)
- Password hashing with bcrypt (work factor 12)

### Consequences

- Tokens in localStorage are vulnerable to XSS (mitigated by Content Security Policy and input sanitization)
- Token revocation is not instant for access tokens (must wait for expiry; refresh token revocation is immediate)
- JWT payload size increases with claims (keep claims minimal)

---

## ADR-007: NgRx for State Management

### Context

The Angular frontend needs consistent state management for auth state, feature data, and UI state across multiple lazy-loaded feature modules.

### Decision

We use **NgRx** (Redux pattern) for all global and feature-level state management.

### Rationale

- **Predictability:** Unidirectional data flow makes state changes traceable and debuggable.
- **DevTools:** NgRx DevTools enable time-travel debugging and action logging.
- **Separation of concerns:** Effects handle side effects cleanly, keeping components focused on presentation.
- **Consistency:** A single pattern for all state management reduces cognitive overhead.
- **Testing:** Reducers are pure functions (trivially testable), effects are isolated side effects.

### When to Use NgRx vs. Local State

- **NgRx:** Auth state, feature entity lists, pagination state, notification state, UI preferences
- **Local component state (signals/properties):** Form state, UI toggles, temporary selections, dialog open/close

### Consequences

- Higher initial boilerplate compared to simple services with BehaviorSubject
- Learning curve for developers unfamiliar with Redux patterns
- Feature store registration adds steps to new feature creation
- Over-engineering risk: not every piece of state needs NgRx

---

## ADR-008: Hangfire for Background Jobs

### Context

We needed background job processing for: document expiry checks, notification delivery, report generation, and cleanup tasks.

### Decision

We use **Hangfire** with SQL Server storage for background job processing.

### Rationale

- **SQL Server storage:** Reuses the existing database, no additional infrastructure (Redis, RabbitMQ) required.
- **Dashboard:** Built-in web dashboard for monitoring jobs, retries, and failures.
- **Retry policy:** Automatic retry with exponential backoff for transient failures.
- **Scheduling:** Support for recurring (cron), delayed, and fire-and-forget jobs.
- **Simplicity:** Jobs are regular C# methods, no special message serialization needed.

### Consequences

- Hangfire jobs run in the same process as the API (can be moved to a worker service later)
- SQL Server polling adds slight database load (configurable interval)
- Dashboard requires authentication (secured via `HangfireAuthorizationFilter`)
- Job serialization uses JSON, so job parameters must be serializable

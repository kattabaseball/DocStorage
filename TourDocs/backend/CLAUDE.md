# TourDocs Backend - Project Intelligence

## Solution Structure

```
backend/
├── src/
│   ├── TourDocs.API/                    # ASP.NET Core Web API (entry point)
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs        # Login, register, refresh, logout
│   │   │   ├── MembersController.cs     # CRUD + search + bulk operations
│   │   │   ├── DocumentsController.cs   # Upload, download, status transitions
│   │   │   ├── CasesController.cs       # Case lifecycle management
│   │   │   ├── ChecklistsController.cs  # Checklist templates and instances
│   │   │   ├── HardCopiesController.cs  # Hard copy chain-of-custody
│   │   │   ├── OrganizationsController.cs # Org settings (admin only)
│   │   │   ├── UsersController.cs       # User management (admin only)
│   │   │   ├── ReportsController.cs     # Report generation endpoints
│   │   │   └── AuditController.cs       # Audit log queries (admin only)
│   │   ├── Middleware/
│   │   │   ├── ExceptionMiddleware.cs   # Global exception handler -> ApiResponse
│   │   │   ├── TenantMiddleware.cs      # Extract org_id from JWT, set ITenantContext
│   │   │   ├── RequestLoggingMiddleware.cs # Log request/response with correlation ID
│   │   │   └── PerformanceMiddleware.cs # Log slow requests (> 500ms)
│   │   ├── Hubs/
│   │   │   ├── NotificationHub.cs       # Real-time notifications
│   │   │   └── DocumentHub.cs           # Live document status updates
│   │   ├── Filters/
│   │   │   ├── ValidationFilter.cs      # Auto-validate request DTOs
│   │   │   └── AuditActionFilter.cs     # Auto-log controller actions
│   │   ├── Extensions/
│   │   │   ├── ServiceCollectionExtensions.cs  # DI registration
│   │   │   ├── MiddlewareExtensions.cs         # Middleware pipeline
│   │   │   └── SwaggerExtensions.cs            # Swagger configuration
│   │   ├── Program.cs                   # Application entry point and configuration
│   │   ├── appsettings.json             # Base configuration (non-secret)
│   │   └── appsettings.Development.json # Dev overrides (gitignored)
│   │
│   ├── TourDocs.Core/                   # Business logic layer
│   │   ├── Services/
│   │   │   ├── AuthService.cs           # Authentication and token management
│   │   │   ├── MemberService.cs         # Member business logic
│   │   │   ├── DocumentService.cs       # Document lifecycle management
│   │   │   ├── CaseService.cs           # Case workflow logic
│   │   │   ├── ChecklistService.cs      # Checklist template management
│   │   │   ├── HardCopyService.cs       # Chain-of-custody logic
│   │   │   ├── NotificationService.cs   # Notification creation and dispatch
│   │   │   ├── AuditService.cs          # Audit log recording
│   │   │   ├── ReportService.cs         # Report generation
│   │   │   └── UserService.cs           # User account management
│   │   ├── DTOs/
│   │   │   ├── Auth/                    # LoginRequest, TokenResponse, etc.
│   │   │   ├── Members/                 # CreateMemberRequest, MemberResponse, etc.
│   │   │   ├── Documents/               # UploadDocumentRequest, DocumentResponse, etc.
│   │   │   ├── Cases/                   # CreateCaseRequest, CaseResponse, etc.
│   │   │   ├── Checklists/              # ChecklistTemplateDto, ChecklistItemDto, etc.
│   │   │   ├── HardCopies/              # HardCopyResponse, TransferRequest, etc.
│   │   │   └── Common/                  # PagedRequest, PagedResponse, ApiResponse
│   │   ├── Interfaces/
│   │   │   ├── Services/                # IAuthService, IMemberService, etc.
│   │   │   ├── Repositories/            # IMemberRepository, IDocumentRepository, etc.
│   │   │   └── Infrastructure/          # IFileStorageService, IEmailService, etc.
│   │   ├── Mapping/
│   │   │   └── MappingProfile.cs        # AutoMapper profiles (all in one file or split per feature)
│   │   ├── Validators/
│   │   │   ├── Auth/                    # LoginRequestValidator, RegisterRequestValidator
│   │   │   ├── Members/                 # CreateMemberRequestValidator, etc.
│   │   │   ├── Documents/               # UploadDocumentRequestValidator, etc.
│   │   │   └── Cases/                   # CreateCaseRequestValidator, etc.
│   │   └── Exceptions/
│   │       ├── NotFoundException.cs
│   │       ├── ForbiddenException.cs
│   │       ├── ValidationException.cs
│   │       ├── BusinessRuleException.cs
│   │       └── ConflictException.cs
│   │
│   ├── TourDocs.Data/                   # Data access layer
│   │   ├── Context/
│   │   │   └── ApplicationDbContext.cs  # DbContext with entity configs and query filters
│   │   ├── Configurations/
│   │   │   ├── MemberConfiguration.cs   # Fluent API config for Member entity
│   │   │   ├── DocumentConfiguration.cs # Fluent API config for Document entity
│   │   │   ├── CaseConfiguration.cs
│   │   │   ├── OrganizationConfiguration.cs
│   │   │   └── ...                      # One config file per entity
│   │   ├── Repositories/
│   │   │   ├── BaseRepository.cs        # Generic CRUD + tenant filtering
│   │   │   ├── MemberRepository.cs      # Member-specific queries
│   │   │   ├── DocumentRepository.cs    # Document-specific queries
│   │   │   ├── CaseRepository.cs
│   │   │   └── ...
│   │   ├── UnitOfWork/
│   │   │   └── UnitOfWork.cs            # IUnitOfWork implementation
│   │   ├── Migrations/                  # EF Core auto-generated migrations
│   │   └── Seeders/
│   │       └── DataSeeder.cs            # Initial data (roles, default org, admin user)
│   │
│   ├── TourDocs.Domain/                 # Domain entities (ZERO NuGet dependencies)
│   │   ├── Entities/
│   │   │   ├── Organization.cs
│   │   │   ├── User.cs
│   │   │   ├── Member.cs
│   │   │   ├── Document.cs
│   │   │   ├── DocumentVersion.cs
│   │   │   ├── Case.cs
│   │   │   ├── CaseMember.cs
│   │   │   ├── ChecklistTemplate.cs
│   │   │   ├── ChecklistItem.cs
│   │   │   ├── ChecklistInstance.cs
│   │   │   ├── HardCopy.cs
│   │   │   ├── HardCopyTransfer.cs
│   │   │   ├── Notification.cs
│   │   │   ├── AuditLog.cs
│   │   │   └── RefreshToken.cs
│   │   ├── Enums/
│   │   │   ├── DocumentStatus.cs        # Uploaded, UnderReview, Verified, Rejected, Expired
│   │   │   ├── HardCopyStatus.cs        # WithMember, Requested, ..., ReturnedToMember
│   │   │   ├── CaseStatus.cs            # Draft, Active, Completed, Cancelled
│   │   │   ├── UserRole.cs              # SuperAdmin, Admin, Manager, Handler, Viewer
│   │   │   ├── NotificationType.cs
│   │   │   └── AuditAction.cs           # View, Download, Upload, StatusChange, Login, etc.
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs            # Id (Guid), CreatedAt, CreatedBy
│   │   │   ├── AuditableEntity.cs       # Extends BaseEntity + UpdatedAt, UpdatedBy
│   │   │   └── ISoftDelete.cs           # IsDeleted, DeletedAt, DeletedBy
│   │   └── Constants/
│   │       ├── Roles.cs                 # Role name constants
│   │       └── Policies.cs              # Authorization policy name constants
│   │
│   └── TourDocs.Infrastructure/         # External integrations
│       ├── FileStorage/
│       │   ├── LocalFileStorageService.cs
│       │   └── FileStorageSettings.cs
│       ├── Email/
│       │   ├── SendGridEmailService.cs
│       │   └── EmailTemplates.cs
│       ├── Identity/
│       │   ├── JwtTokenService.cs       # JWT generation and validation
│       │   ├── JwtSettings.cs
│       │   └── PasswordHasher.cs
│       └── BackgroundJobs/
│           ├── DocumentExpiryJob.cs     # Daily check for expired documents
│           ├── NotificationCleanupJob.cs # Clean old read notifications
│           └── ReportGenerationJob.cs   # Scheduled report generation
│
├── tests/
│   ├── TourDocs.UnitTests/              # xUnit unit tests
│   │   ├── Services/
│   │   ├── Validators/
│   │   └── Helpers/                     # Test builders, fixtures
│   └── TourDocs.IntegrationTests/       # Integration tests with test DB
│       ├── Controllers/
│       ├── Repositories/
│       └── Fixtures/                    # WebApplicationFactory, test DB setup
│
├── TourDocs.sln                         # Solution file
├── .env.example                         # Environment variable template
├── Dockerfile                           # Multi-stage Docker build
├── CLAUDE.md                            # This file
└── Directory.Build.props                # Shared MSBuild properties
```

## How to Add a New Entity

Follow these steps in order. Each step builds on the previous one.

### Step 1: Create the Entity (TourDocs.Domain)

```csharp
// Domain/Entities/Widget.cs
namespace TourDocs.Domain.Entities;

public class Widget : AuditableEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
```

### Step 2: Create the EF Configuration (TourDocs.Data)

```csharp
// Data/Configurations/WidgetConfiguration.cs
namespace TourDocs.Data.Configurations;

public class WidgetConfiguration : IEntityTypeConfiguration<Widget>
{
    public void Configure(EntityTypeBuilder<Widget> builder)
    {
        builder.ToTable("Widgets");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .HasMaxLength(2000);

        builder.HasOne(w => w.Organization)
            .WithMany()
            .HasForeignKey(w => w.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete filter
        builder.HasQueryFilter(w => !w.IsDeleted);

        // Indexes
        builder.HasIndex(w => w.OrganizationId);
        builder.HasIndex(w => new { w.OrganizationId, w.Name }).IsUnique();
    }
}
```

### Step 3: Add DbSet to ApplicationDbContext

```csharp
// In Data/Context/ApplicationDbContext.cs
public DbSet<Widget> Widgets => Set<Widget>();
```

### Step 4: Create and Run Migration

```bash
cd backend/src/TourDocs.API
dotnet ef migrations add AddWidget --project ../TourDocs.Data --context ApplicationDbContext
dotnet ef database update --project ../TourDocs.Data
```

### Step 5: Create Repository Interface (TourDocs.Core)

```csharp
// Core/Interfaces/Repositories/IWidgetRepository.cs
namespace TourDocs.Core.Interfaces.Repositories;

public interface IWidgetRepository : IBaseRepository<Widget>
{
    Task<Widget?> GetByNameAsync(Guid orgId, string name);
    Task<IEnumerable<Widget>> GetByOrganizationAsync(Guid orgId);
}
```

### Step 6: Implement Repository (TourDocs.Data)

```csharp
// Data/Repositories/WidgetRepository.cs
namespace TourDocs.Data.Repositories;

public class WidgetRepository : BaseRepository<Widget>, IWidgetRepository
{
    public WidgetRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Widget?> GetByNameAsync(Guid orgId, string name)
    {
        return await _dbSet
            .Where(w => w.OrganizationId == orgId && w.Name == name)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Widget>> GetByOrganizationAsync(Guid orgId)
    {
        return await _dbSet
            .Where(w => w.OrganizationId == orgId)
            .OrderBy(w => w.Name)
            .ToListAsync();
    }
}
```

### Step 7: Register Repository in UnitOfWork

```csharp
// In Data/UnitOfWork/UnitOfWork.cs
public IWidgetRepository Widgets => _widgets ??= new WidgetRepository(_context);
private IWidgetRepository? _widgets;
```

Also add to `IUnitOfWork` interface:
```csharp
IWidgetRepository Widgets { get; }
```

### Step 8: Create DTOs (TourDocs.Core)

```csharp
// Core/DTOs/Widgets/CreateWidgetRequest.cs
public record CreateWidgetRequest(string Name, string? Description);

// Core/DTOs/Widgets/UpdateWidgetRequest.cs
public record UpdateWidgetRequest(string Name, string? Description);

// Core/DTOs/Widgets/WidgetResponse.cs
public record WidgetResponse(Guid Id, string Name, string? Description, DateTime CreatedAt);
```

### Step 9: Create Validator (TourDocs.Core)

```csharp
// Core/Validators/Widgets/CreateWidgetRequestValidator.cs
public class CreateWidgetRequestValidator : AbstractValidator<CreateWidgetRequest>
{
    public CreateWidgetRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");
    }
}
```

### Step 10: Create AutoMapper Profile

```csharp
// In Core/Mapping/MappingProfile.cs (add to existing profile)
CreateMap<Widget, WidgetResponse>();
CreateMap<CreateWidgetRequest, Widget>();
```

### Step 11: Create Service Interface and Implementation

```csharp
// Core/Interfaces/Services/IWidgetService.cs
public interface IWidgetService
{
    Task<WidgetResponse> GetByIdAsync(Guid id);
    Task<PagedResponse<WidgetResponse>> GetAllAsync(PagedRequest request);
    Task<WidgetResponse> CreateAsync(CreateWidgetRequest request);
    Task<WidgetResponse> UpdateAsync(Guid id, UpdateWidgetRequest request);
    Task DeleteAsync(Guid id);
}

// Core/Services/WidgetService.cs
public class WidgetService : IWidgetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<WidgetService> _logger;

    public WidgetService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITenantContext tenantContext,
        ILogger<WidgetService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<WidgetResponse> GetByIdAsync(Guid id)
    {
        var widget = await _unitOfWork.Widgets.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Widget), id);

        return _mapper.Map<WidgetResponse>(widget);
    }

    public async Task<WidgetResponse> CreateAsync(CreateWidgetRequest request)
    {
        var existing = await _unitOfWork.Widgets
            .GetByNameAsync(_tenantContext.OrganizationId, request.Name);

        if (existing is not null)
            throw new ConflictException($"Widget with name '{request.Name}' already exists.");

        var widget = _mapper.Map<Widget>(request);
        widget.OrganizationId = _tenantContext.OrganizationId;

        await _unitOfWork.Widgets.AddAsync(widget);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Widget {WidgetId} created for org {OrgId}", widget.Id, widget.OrganizationId);
        return _mapper.Map<WidgetResponse>(widget);
    }

    // ... UpdateAsync, DeleteAsync follow same pattern
}
```

### Step 12: Create Controller (TourDocs.API)

```csharp
// API/Controllers/WidgetsController.cs
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class WidgetsController : ControllerBase
{
    private readonly IWidgetService _widgetService;

    public WidgetsController(IWidgetService widgetService)
    {
        _widgetService = widgetService;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<WidgetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _widgetService.GetByIdAsync(id);
        return Ok(ApiResponse<WidgetResponse>.Success(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WidgetResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateWidgetRequest request)
    {
        var result = await _widgetService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<WidgetResponse>.Success(result));
    }

    // ... GET all, PUT, DELETE follow same pattern
}
```

### Step 13: Register Service in DI

```csharp
// In API/Extensions/ServiceCollectionExtensions.cs
services.AddScoped<IWidgetService, WidgetService>();
```

## How to Add a New API Endpoint

For adding an endpoint to an existing controller:

1. Add the method signature to the service interface in `Core/Interfaces/Services/`
2. Implement the method in the service class in `Core/Services/`
3. Create any new DTOs needed in `Core/DTOs/`
4. Create validators for new request DTOs in `Core/Validators/`
5. Add AutoMapper mappings if needed in `Core/Mapping/MappingProfile.cs`
6. Add the controller action in `API/Controllers/` with proper attributes
7. Write unit tests for the service method
8. Test via Swagger at `https://localhost:7001/swagger`

## EF Core Migration Workflow

### Creating a Migration

Always run migration commands from the API project directory, pointing to the Data project:

```bash
cd backend/src/TourDocs.API

# Create a new migration
dotnet ef migrations add <MigrationName> --project ../TourDocs.Data --context ApplicationDbContext

# Apply migrations to database
dotnet ef database update --project ../TourDocs.Data

# Remove last migration (if not yet applied)
dotnet ef migrations remove --project ../TourDocs.Data

# Generate SQL script (for production review)
dotnet ef migrations script --project ../TourDocs.Data --output ../migrations.sql

# Generate idempotent script (safe to run multiple times)
dotnet ef migrations script --project ../TourDocs.Data --idempotent --output ../migrations.sql
```

### Migration Naming Conventions

Use descriptive names that indicate what changed:
- `AddWidget` — New entity/table
- `AddDescriptionToWidget` — New column
- `AddWidgetOrganizationIndex` — New index
- `RenameWidgetTitleToName` — Column rename
- `RemoveWidgetLegacyField` — Column removal

### Migration Rules

- NEVER modify an existing migration that has been applied to any environment
- ALWAYS review generated migration code before applying
- ALWAYS test migrations against a copy of production data before deploying
- Include both `Up()` and `Down()` methods — ensure rollback works
- For data migrations, use raw SQL in the migration rather than EF entities (entities may change)

## Dependency Injection Registration

All DI registration is centralized in `API/Extensions/ServiceCollectionExtensions.cs`:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<IChecklistService, ChecklistService>();
        services.AddScoped<IHardCopyService, HardCopyService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Tenant Context
        services.AddScoped<ITenantContext, TenantContext>();

        // File Storage
        services.Configure<FileStorageSettings>(config.GetSection("FileStorage"));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // Email
        services.AddScoped<IEmailService, SendGridEmailService>();

        // JWT
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                }));

        return services;
    }
}
```

**Registration in Program.cs:**
```csharp
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
```

### Lifetime Rules

- **Scoped** (default): Services, repositories, UnitOfWork, TenantContext — one instance per HTTP request
- **Singleton**: Configuration objects, `ILogger<T>` (framework handles this)
- **Transient**: Validators, lightweight stateless utilities

## Repository Pattern

### Base Repository

The `BaseRepository<T>` provides generic CRUD operations with automatic tenant filtering:

```csharp
public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual void Update(T entity) => _dbSet.Update(entity);

    public virtual void Delete(T entity)
    {
        if (entity is ISoftDelete softDelete)
        {
            softDelete.IsDeleted = true;
            softDelete.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
        else
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task<PagedResponse<T>> GetPagedAsync(PagedRequest request)
    {
        var query = _dbSet.AsQueryable();
        var total = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<T>(items, total, request.PageNumber, request.PageSize);
    }
}
```

### Feature-Specific Repositories

Feature repositories extend `BaseRepository<T>` and add domain-specific queries:

```csharp
public class DocumentRepository : BaseRepository<Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Document>> GetByMemberAsync(Guid memberId)
    {
        return await _dbSet
            .Include(d => d.Versions)
            .Where(d => d.MemberId == memberId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetExpiringAsync(DateTime beforeDate)
    {
        return await _dbSet
            .Where(d => d.ExpiryDate != null && d.ExpiryDate <= beforeDate && d.Status == DocumentStatus.Verified)
            .Include(d => d.Member)
            .ToListAsync();
    }
}
```

### Unit of Work

The `IUnitOfWork` aggregates all repositories and manages transactions:

```csharp
public interface IUnitOfWork : IDisposable
{
    IMemberRepository Members { get; }
    IDocumentRepository Documents { get; }
    ICaseRepository Cases { get; }
    IChecklistRepository Checklists { get; }
    IHardCopyRepository HardCopies { get; }
    IAuditLogRepository AuditLogs { get; }
    INotificationRepository Notifications { get; }
    IUserRepository Users { get; }
    IOrganizationRepository Organizations { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

Use transactions for operations that span multiple aggregates:

```csharp
await _unitOfWork.BeginTransactionAsync();
try
{
    await _unitOfWork.Documents.AddAsync(document);
    await _unitOfWork.AuditLogs.AddAsync(auditEntry);
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitTransactionAsync();
}
catch
{
    await _unitOfWork.RollbackTransactionAsync();
    throw;
}
```

## Exception Handling Flow

### Custom Exceptions

```
BusinessRuleException     -> 422 Unprocessable Entity
ConflictException         -> 409 Conflict
ForbiddenException        -> 403 Forbidden
NotFoundException         -> 404 Not Found
ValidationException       -> 400 Bad Request
UnauthorizedAccessException -> 401 Unauthorized (framework)
Exception (unhandled)     -> 500 Internal Server Error
```

### ExceptionMiddleware

The middleware catches all exceptions and returns consistent `ApiResponse<T>`:

```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, e.Message),
            ForbiddenException e => (StatusCodes.Status403Forbidden, e.Message),
            ValidationException e => (StatusCodes.Status400BadRequest, e.Message),
            BusinessRuleException e => (StatusCodes.Status422UnprocessableEntity, e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, e.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Failure(message);
        // For ValidationException, include field-level errors
        if (exception is ValidationException validationEx)
            response.Errors = validationEx.Errors;

        await context.Response.WriteAsJsonAsync(response);
    }
}
```

### Throwing Exceptions in Services

```csharp
// Entity not found
var member = await _unitOfWork.Members.GetByIdAsync(id)
    ?? throw new NotFoundException(nameof(Member), id);

// Business rule violation
if (document.Status == DocumentStatus.Expired)
    throw new BusinessRuleException("Cannot verify an expired document.");

// Authorization check beyond role
if (member.OrganizationId != _tenantContext.OrganizationId)
    throw new ForbiddenException("You do not have access to this member.");

// Duplicate detection
var existing = await _unitOfWork.Members.GetByEmailAsync(request.Email);
if (existing is not null)
    throw new ConflictException($"A member with email '{request.Email}' already exists.");
```

## Authentication and Authorization

### JWT Configuration

In `appsettings.json`:
```json
{
  "Jwt": {
    "Secret": "SET_VIA_ENVIRONMENT_VARIABLE",
    "Issuer": "TourDocs",
    "Audience": "TourDocs",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Token Generation (JwtTokenService)

```csharp
public string GenerateAccessToken(User user, Organization org)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, user.Role.ToString()),
        new("org_id", org.Id.ToString()),
        new("org_name", org.Name),
        new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

### Authorization Patterns

**Role-based (attribute):**
```csharp
[Authorize(Roles = "Admin,Manager")]
[HttpPost]
public async Task<IActionResult> Create(...) { }
```

**Policy-based (for complex rules):**
```csharp
// In Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageMembers", policy =>
        policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("CanVerifyDocuments", policy =>
        policy.RequireRole("Admin", "Manager", "Handler"));
    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireRole("SuperAdmin"));
});

// In controller
[Authorize(Policy = "CanVerifyDocuments")]
[HttpPut("{id}/verify")]
public async Task<IActionResult> Verify(Guid id) { }
```

**Service-level authorization (for tenant-scoped checks):**
```csharp
// In service - verify entity belongs to current tenant
var member = await _unitOfWork.Members.GetByIdAsync(id)
    ?? throw new NotFoundException(nameof(Member), id);

if (member.OrganizationId != _tenantContext.OrganizationId)
    throw new ForbiddenException("Access denied.");
```

## Background Jobs with Hangfire

### Setup in Program.cs

```csharp
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// After app.Build()
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Register recurring jobs
RecurringJob.AddOrUpdate<DocumentExpiryJob>(
    "document-expiry-check",
    job => job.ExecuteAsync(),
    Cron.Daily(2, 0)); // Run at 2:00 AM UTC

RecurringJob.AddOrUpdate<NotificationCleanupJob>(
    "notification-cleanup",
    job => job.ExecuteAsync(),
    Cron.Weekly(DayOfWeek.Sunday, 3, 0)); // Sunday 3:00 AM UTC
```

### Job Implementation

```csharp
public class DocumentExpiryJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DocumentExpiryJob> _logger;

    public DocumentExpiryJob(IServiceScopeFactory scopeFactory, ILogger<DocumentExpiryJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var expiringDocs = await unitOfWork.Documents.GetExpiringAsync(DateTime.UtcNow);

        foreach (var doc in expiringDocs)
        {
            doc.Status = DocumentStatus.Expired;
            unitOfWork.Documents.Update(doc);

            await notificationService.SendDocumentExpiredAsync(doc);
            _logger.LogInformation("Document {DocId} marked as expired", doc.Id);
        }

        await unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Document expiry job completed. {Count} documents expired.", expiringDocs.Count());
    }
}
```

### Enqueuing One-Off Jobs

```csharp
// Fire-and-forget
BackgroundJob.Enqueue<IEmailService>(service =>
    service.SendWelcomeEmailAsync(user.Email, user.FirstName));

// Delayed
BackgroundJob.Schedule<IReportService>(service =>
    service.GenerateMonthlyReportAsync(orgId),
    TimeSpan.FromHours(1));
```

## Logging with Serilog

### Configuration in Program.cs

```csharp
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithCorrelationId()
        .Enrich.WithMachineName()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}")
        .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341")
        .WriteTo.File("logs/tourdocs-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30));
```

### Logging Conventions

```csharp
// Structured logging with named parameters (NOT string interpolation)
_logger.LogInformation("Member {MemberId} created by {UserId} in org {OrgId}", member.Id, userId, orgId);

// Correct log levels:
// Trace   - Detailed diagnostic info (query parameters, full request bodies)
// Debug   - Development-time diagnostics (cache hits/misses, mapping details)
// Information - Normal application flow (entity created, job completed, user logged in)
// Warning - Recoverable issues (retry attempt, deprecated feature used, nearing rate limit)
// Error   - Failures requiring attention (unhandled exception, external service failure)
// Fatal   - Application cannot continue (database unreachable, critical config missing)

// Always include correlation context
using (LogContext.PushProperty("MemberId", memberId))
{
    _logger.LogInformation("Processing document upload");
    // ... all log entries within this block include MemberId
}
```

## Database Connection String Management

### Development

Use User Secrets (never commit connection strings):

```bash
cd backend/src/TourDocs.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=TourDocs;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Secret" "your-256-bit-secret-key-for-development-only"
```

### appsettings.json (committed)

Contains non-secret configuration only:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SET_VIA_USER_SECRETS_OR_ENV"
  }
}
```

### Production

Use environment variables (Docker, Azure, etc.):
```bash
ConnectionStrings__DefaultConnection="Server=prod-server;Database=TourDocs;..."
```

### Precedence Order

1. Environment variables (highest priority)
2. User Secrets (development only)
3. `appsettings.{Environment}.json`
4. `appsettings.json` (lowest priority)

## API Response Format

All endpoints return a consistent response wrapper:

```json
{
  "success": true,
  "message": "Member retrieved successfully.",
  "data": { ... },
  "errors": null
}
```

Error response:
```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": {
    "Email": ["Email is required.", "Email must be a valid email address."],
    "Name": ["Name must not exceed 200 characters."]
  }
}
```

Paged response:
```json
{
  "success": true,
  "message": null,
  "data": {
    "items": [ ... ],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 8,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "errors": null
}
```

using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TourDocs.API.Filters;
using TourDocs.API.Hubs;
using TourDocs.API.Middleware;
using TourDocs.Core;
using TourDocs.Data;
using TourDocs.Data.Context;
using TourDocs.Domain.Entities;
using TourDocs.Infrastructure;
using TourDocs.Infrastructure.BackgroundJobs;
using TourDocs.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

// Data layer (DbContext, repositories, UoW)
builder.Services.AddDataServices(builder.Configuration);

// ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };

    // SignalR token support
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OrgOwnerOnly", policy => policy.RequireRole("OrgOwner"));
    options.AddPolicy("CanManageMembers", policy => policy.RequireRole("OrgOwner", "OrgMember", "CaseManager"));
    options.AddPolicy("CanManageCases", policy => policy.RequireRole("OrgOwner", "OrgMember", "CaseManager"));
    options.AddPolicy("CanVerifyDocuments", policy => policy.RequireRole("OrgOwner", "OrgMember", "CaseManager", "DocumentHandler"));
    options.AddPolicy("CanViewAudit", policy => policy.RequireRole("OrgOwner"));
});

// Core services (business logic, AutoMapper, validators)
builder.Services.AddCoreServices();

// Infrastructure services (file storage, JWT, identity)
builder.Services.AddInfrastructureServices(builder.Configuration);

// HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Controllers with filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
    options.Filters.Add<AuditActionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetValue<string>("Cors:AllowedOrigins") ?? "http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// SignalR
builder.Services.AddSignalR();

// Hangfire
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"),
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
                SchemaName = "HangFire"
            }));
builder.Services.AddHangfireServer();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TourDocs API",
        Version = "v1",
        Description = "Enterprise Document Management Platform API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Exception handling middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TenantMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TourDocs API v1");
    });
}

app.UseHttpsRedirection();

// Static files for local file storage
var fileStoragePath = builder.Configuration.GetValue<string>("FileStorage:BasePath") ?? "Storage";
if (!Directory.Exists(fileStoragePath))
{
    Directory.CreateDirectory(fileStoragePath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), fileStoragePath)),
    RequestPath = "/files"
});

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Hangfire Dashboard (dev only for now)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}

// Register recurring jobs
RecurringJob.AddOrUpdate<DocumentExpiryJob>(
    "document-expiry-check",
    job => job.ExecuteAsync(),
    Cron.Daily(2, 0)); // 2:00 AM UTC daily

RecurringJob.AddOrUpdate<NotificationCleanupJob>(
    "notification-cleanup",
    job => job.ExecuteAsync(),
    Cron.Weekly(DayOfWeek.Sunday, 3, 0)); // Sunday 3:00 AM UTC

app.UseSerilogRequestLogging();

// Seed database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TourDocs.Data.Context.ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TourDocs.Domain.Entities.ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await TourDocs.Data.Seeders.DatabaseSeeder.SeedAsync(context, userManager, roleManager);
}

app.Run();

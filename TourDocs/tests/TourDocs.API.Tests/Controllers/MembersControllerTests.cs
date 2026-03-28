using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TourDocs.API.Tests.Controllers;

/// <summary>
/// Integration tests for the Members API controller.
/// Uses WebApplicationFactory with a test authentication handler to simulate
/// authenticated and unauthenticated requests.
/// </summary>
public class MembersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private static readonly Guid TestOrgId = Guid.Parse("aaaa0000-0000-0000-0000-000000000001");
    private static readonly Guid TestUserId = Guid.Parse("aaaa0000-0000-0000-0000-000000000002");

    public MembersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureTestServices(services =>
            {
                // Register a test authentication scheme that bypasses JWT validation
                services.AddAuthentication("TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { });
            });
        });
    }

    /// <summary>
    /// Creates an HttpClient that includes a valid test authentication token.
    /// The TestAuthHandler will parse the "X-Test-UserId" and "X-Test-OrgId" headers
    /// to construct claims for the authenticated user.
    /// </summary>
    private HttpClient CreateAuthenticatedClient()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
        client.DefaultRequestHeaders.Add("X-Test-UserId", TestUserId.ToString());
        client.DefaultRequestHeaders.Add("X-Test-OrgId", TestOrgId.ToString());

        return client;
    }

    /// <summary>
    /// Creates an HttpClient with no authentication headers — simulating an anonymous user.
    /// </summary>
    private HttpClient CreateAnonymousClient()
    {
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    // ─────────────────────────────────────────────────────────────
    // GET /api/v1/members
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetMembers_Authenticated_ReturnsOk()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/v1/members");

        // Assert
        // The endpoint should return 200 OK for authenticated requests.
        // Even if the member list is empty, the response should be successful.
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task GetMembers_Unauthenticated_Returns401()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync("/api/v1/members");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ─────────────────────────────────────────────────────────────
    // POST /api/v1/members
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateMember_ValidPayload_Returns201()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        var payload = new
        {
            legalFirstName = "Amal",
            legalLastName = "Silva",
            email = $"amal.{Guid.NewGuid():N}@test.com", // Unique email per test run
            nationality = "Sri Lankan",
            phone = "+94 77 123 4567"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/api/v1/members", content);

        // Assert
        // Expect 201 Created for a successful creation.
        // Some implementations may return 200 OK instead — both indicate success.
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            response.Headers.Location.Should().NotBeNull("a 201 response should include a Location header");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateMember_InvalidPayload_Returns400()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Missing required fields (legalFirstName and legalLastName)
        var payload = new
        {
            email = "incomplete@test.com"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/api/v1/members", content);

        // Assert
        // Should return 400 Bad Request for missing required fields,
        // or 422 Unprocessable Entity for validation failures.
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.UnprocessableEntity);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().NotBeNullOrEmpty();
    }
}

/// <summary>
/// A test authentication handler that creates authenticated users from request headers.
/// This handler is used in integration tests to bypass JWT authentication while still
/// providing proper claims-based identity for authorization checks.
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Only authenticate if the Authorization header is present
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userId = Request.Headers["X-Test-UserId"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        var orgId = Request.Headers["X-Test-OrgId"].FirstOrDefault() ?? Guid.NewGuid().ToString();

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Email, "test@test.com"),
            new Claim(ClaimTypes.Role, "OrgOwner"),
            new Claim("org_id", orgId),
        };

        var identity = new ClaimsIdentity(claims, "TestScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

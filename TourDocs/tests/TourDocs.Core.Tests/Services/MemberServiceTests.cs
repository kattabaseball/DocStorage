using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Members;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Core.Services;
using TourDocs.Domain.Entities;

namespace TourDocs.Core.Tests.Services;

public class MemberServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Member>> _memberRepoMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ILogger<MemberService>> _loggerMock;
    private readonly MemberService _sut;

    private static readonly Guid OrgId = Guid.Parse("aaaa0000-0000-0000-0000-000000000001");
    private static readonly Guid UserId = Guid.Parse("aaaa0000-0000-0000-0000-000000000002");

    public MemberServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _memberRepoMock = new Mock<IRepository<Member>>();
        _tenantContextMock = new Mock<ITenantContext>();
        _auditServiceMock = new Mock<IAuditService>();
        _loggerMock = new Mock<ILogger<MemberService>>();

        _unitOfWorkMock.Setup(u => u.Members).Returns(_memberRepoMock.Object);

        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);
        _tenantContextMock.Setup(t => t.UserId).Returns(UserId);

        _sut = new MemberService(
            _unitOfWorkMock.Object,
            _tenantContextMock.Object,
            _auditServiceMock.Object,
            _loggerMock.Object);
    }

    // ─────────────────────────────────────────────────────────────
    // CreateAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateMember_ValidInput_ReturnsCreatedMember()
    {
        // Arrange
        var request = new CreateMemberRequest
        {
            LegalFirstName = "Kamal",
            LegalLastName = "Perera",
            Email = "kamal@test.com",
            Nationality = "Sri Lankan",
            Phone = "+94 77 123 4567"
        };

        _memberRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
            .ReturnsAsync((Member?)null);

        _memberRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Member>()))
            .ReturnsAsync((Member m) => m);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.LegalFirstName.Should().Be("Kamal");
        result.LegalLastName.Should().Be("Perera");
        result.Email.Should().Be("kamal@test.com");
        result.Nationality.Should().Be("Sri Lankan");
        result.OrganizationId.Should().Be(OrgId);
        result.IsActive.Should().BeTrue();
        result.Id.Should().NotBeEmpty();

        _memberRepoMock.Verify(r => r.AddAsync(It.IsAny<Member>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _auditServiceMock.Verify(a => a.LogAsync(
            OrgId, UserId, "Member.Created", "Member", It.IsAny<Guid>(), It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task CreateMember_DuplicateEmail_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateMemberRequest
        {
            LegalFirstName = "Nimal",
            LegalLastName = "Silva",
            Email = "existing@test.com"
        };

        var existingMember = new Member
        {
            Id = Guid.NewGuid(),
            OrganizationId = OrgId,
            LegalFirstName = "Existing",
            LegalLastName = "Member",
            Email = "existing@test.com"
        };

        _memberRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
            .ReturnsAsync(existingMember);

        // Act
        var act = () => _sut.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*email*already exists*");

        _memberRepoMock.Verify(r => r.AddAsync(It.IsAny<Member>()), Times.Never);
    }

    // ─────────────────────────────────────────────────────────────
    // GetByIdAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetMemberById_ExistingId_ReturnsMember()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var member = new Member
        {
            Id = memberId,
            OrganizationId = OrgId,
            LegalFirstName = "Amal",
            LegalLastName = "Silva",
            Email = "amal@email.com",
            IsActive = true
        };

        _memberRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
            .ReturnsAsync(member);

        // Act
        var result = await _sut.GetByIdAsync(memberId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(memberId);
        result.LegalFirstName.Should().Be("Amal");
        result.LegalLastName.Should().Be("Silva");
    }

    [Fact]
    public async Task GetMemberById_NonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        _memberRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
            .ReturnsAsync((Member?)null);

        // Act
        var act = () => _sut.GetByIdAsync(memberId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Member*{memberId}*");
    }

    // ─────────────────────────────────────────────────────────────
    // UpdateAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateMember_ValidInput_UpdatesMember()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var existingMember = new Member
        {
            Id = memberId,
            OrganizationId = OrgId,
            LegalFirstName = "Old",
            LegalLastName = "Name",
            Email = "old@test.com",
            IsActive = true
        };

        var request = new UpdateMemberRequest
        {
            LegalFirstName = "Updated",
            LegalLastName = "Name",
            Email = "updated@test.com",
            Phone = "+94 71 999 8888"
        };

        // FirstOrDefaultAsync is called twice: once to find the member, once to check duplicate email
        var callCount = 0;
        _memberRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount == 1 ? existingMember : null; // First: found, Second: no duplicate
            });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _sut.UpdateAsync(memberId, request);

        // Assert
        result.Should().NotBeNull();
        result.LegalFirstName.Should().Be("Updated");
        result.Email.Should().Be("updated@test.com");
        result.Phone.Should().Be("+94 71 999 8888");

        _memberRepoMock.Verify(r => r.Update(It.IsAny<Member>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    // ─────────────────────────────────────────────────────────────
    // DeleteAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteMember_ExistingId_SoftDeletes()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var member = new Member
        {
            Id = memberId,
            OrganizationId = OrgId,
            LegalFirstName = "ToDelete",
            LegalLastName = "Member",
            IsActive = true,
            IsDeleted = false
        };

        _memberRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
            .ReturnsAsync(member);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _sut.DeleteAsync(memberId);

        // Assert
        member.IsDeleted.Should().BeTrue();
        member.IsActive.Should().BeFalse();
        member.DeletedAt.Should().NotBeNull();
        member.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _memberRepoMock.Verify(r => r.Update(member), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _auditServiceMock.Verify(a => a.LogAsync(
            OrgId, UserId, "Member.Deleted", "Member", memberId, It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    // ─────────────────────────────────────────────────────────────
    // GetByOrganizationAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetMembersByOrganization_ReturnsFilteredMembers()
    {
        // Arrange
        var members = new List<Member>
        {
            new() { Id = Guid.NewGuid(), OrganizationId = OrgId, LegalFirstName = "Amal", LegalLastName = "Silva", IsDeleted = false },
            new() { Id = Guid.NewGuid(), OrganizationId = OrgId, LegalFirstName = "Chaminda", LegalLastName = "Bandara", IsDeleted = false },
            new() { Id = Guid.NewGuid(), OrganizationId = OrgId, LegalFirstName = "Deleted", LegalLastName = "Member", IsDeleted = true },
            new() { Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), LegalFirstName = "Other", LegalLastName = "Org", IsDeleted = false },
        };

        _memberRepoMock
            .Setup(r => r.Query())
            .Returns(members.AsQueryable());

        var pagedRequest = new PagedRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _sut.GetByOrganizationAsync(OrgId, pagedRequest);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items.Should().Contain(m => m.LegalFirstName == "Amal");
        result.Items.Should().Contain(m => m.LegalFirstName == "Chaminda");
        result.Items.Should().NotContain(m => m.LegalFirstName == "Deleted");
        result.Items.Should().NotContain(m => m.LegalFirstName == "Other");
    }

    // ─────────────────────────────────────────────────────────────
    // SearchAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchMembers_MatchingQuery_ReturnsResults()
    {
        // Arrange
        var members = new List<Member>
        {
            new() { Id = Guid.NewGuid(), OrganizationId = OrgId, LegalFirstName = "Amal", LegalLastName = "Silva", Email = "amal@email.com", IsDeleted = false },
            new() { Id = Guid.NewGuid(), OrganizationId = OrgId, LegalFirstName = "Chaminda", LegalLastName = "Bandara", Email = "chaminda@email.com", IsDeleted = false },
            new() { Id = Guid.NewGuid(), OrganizationId = OrgId, LegalFirstName = "Dilini", LegalLastName = "Jayawardena", Email = "dilini@email.com", IsDeleted = false },
        };

        _memberRepoMock
            .Setup(r => r.Query())
            .Returns(members.AsQueryable());

        var pagedRequest = new PagedRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _sut.SearchAsync("amal", pagedRequest);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().LegalFirstName.Should().Be("Amal");
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task SearchMembers_NoMatch_ReturnsEmpty()
    {
        // Arrange
        var members = new List<Member>
        {
            new() { Id = Guid.NewGuid(), OrganizationId = OrgId, LegalFirstName = "Amal", LegalLastName = "Silva", Email = "amal@email.com", IsDeleted = false },
        };

        _memberRepoMock
            .Setup(r => r.Query())
            .Returns(members.AsQueryable());

        var pagedRequest = new PagedRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _sut.SearchAsync("nonexistent", pagedRequest);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}

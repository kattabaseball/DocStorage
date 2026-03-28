using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourDocs.Core.DTOs.Cases;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Core.Services;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.Tests.Services;

public class CaseServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Case>> _caseRepoMock;
    private readonly Mock<IRepository<CaseMember>> _caseMemberRepoMock;
    private readonly Mock<IRepository<CaseAccess>> _caseAccessRepoMock;
    private readonly Mock<IRepository<Member>> _memberRepoMock;
    private readonly Mock<IRepository<ChecklistItem>> _checklistItemRepoMock;
    private readonly Mock<IRepository<Document>> _documentRepoMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ILogger<CaseService>> _loggerMock;
    private readonly CaseService _sut;

    private static readonly Guid OrgId = Guid.Parse("aaaa0000-0000-0000-0000-000000000001");
    private static readonly Guid UserId = Guid.Parse("aaaa0000-0000-0000-0000-000000000002");

    public CaseServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _caseRepoMock = new Mock<IRepository<Case>>();
        _caseMemberRepoMock = new Mock<IRepository<CaseMember>>();
        _caseAccessRepoMock = new Mock<IRepository<CaseAccess>>();
        _memberRepoMock = new Mock<IRepository<Member>>();
        _checklistItemRepoMock = new Mock<IRepository<ChecklistItem>>();
        _documentRepoMock = new Mock<IRepository<Document>>();
        _tenantContextMock = new Mock<ITenantContext>();
        _auditServiceMock = new Mock<IAuditService>();
        _loggerMock = new Mock<ILogger<CaseService>>();

        _unitOfWorkMock.Setup(u => u.Cases).Returns(_caseRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CaseMembers).Returns(_caseMemberRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CaseAccesses).Returns(_caseAccessRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Members).Returns(_memberRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ChecklistItems).Returns(_checklistItemRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Documents).Returns(_documentRepoMock.Object);

        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);
        _tenantContextMock.Setup(t => t.UserId).Returns(UserId);

        _sut = new CaseService(
            _unitOfWorkMock.Object,
            _tenantContextMock.Object,
            _auditServiceMock.Object,
            _loggerMock.Object);
    }

    // ─────────────────────────────────────────────────────────────
    // CreateAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateCase_ValidInput_ReturnsCreatedCase()
    {
        // Arrange
        var request = new CreateCaseRequest
        {
            Name = "European Summer Tour 2026",
            CaseType = "tour",
            DestinationCountry = "Germany",
            DestinationCity = "Berlin",
            StartDate = new DateTime(2026, 6, 15),
            EndDate = new DateTime(2026, 7, 5),
            Description = "Three-week European tour."
        };

        _caseRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Case>()))
            .ReturnsAsync((Case c) => c);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("European Summer Tour 2026");
        result.CaseType.Should().Be("tour");
        result.DestinationCountry.Should().Be("Germany");
        result.Status.Should().Be(CaseStatus.Draft);
        result.ReferenceNumber.Should().StartWith("CASE-");
        result.OrganizationId.Should().Be(OrgId);
        result.Id.Should().NotBeEmpty();

        _caseRepoMock.Verify(r => r.AddAsync(It.IsAny<Case>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _auditServiceMock.Verify(a => a.LogAsync(
            OrgId, UserId, "Case.Created", "Case", It.IsAny<Guid>(), It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    // ─────────────────────────────────────────────────────────────
    // AssignMembersAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AssignMembers_ValidIds_CreatesJunctions()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var member1Id = Guid.NewGuid();
        var member2Id = Guid.NewGuid();

        var caseEntity = new Case
        {
            Id = caseId,
            OrganizationId = OrgId,
            Name = "Test Case"
        };

        var member1 = new Member { Id = member1Id, OrganizationId = OrgId, LegalFirstName = "M1", LegalLastName = "L1" };
        var member2 = new Member { Id = member2Id, OrganizationId = OrgId, LegalFirstName = "M2", LegalLastName = "L2" };

        _caseRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Case, bool>>>()))
            .ReturnsAsync(caseEntity);

        _caseMemberRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<CaseMember, bool>>>()))
            .ReturnsAsync(new List<CaseMember>()); // No existing assignments

        _memberRepoMock
            .Setup(r => r.GetByIdAsync(member1Id))
            .ReturnsAsync(member1);

        _memberRepoMock
            .Setup(r => r.GetByIdAsync(member2Id))
            .ReturnsAsync(member2);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _sut.AssignMembersAsync(caseId, new[] { member1Id, member2Id });

        // Assert
        _caseMemberRepoMock.Verify(
            r => r.AddRangeAsync(It.Is<IEnumerable<CaseMember>>(items => items.Count() == 2)),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _auditServiceMock.Verify(a => a.LogAsync(
            OrgId, UserId, "Case.MembersAssigned", "Case", caseId, It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task AssignMembers_AlreadyAssigned_SkipsDuplicates()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var member1Id = Guid.NewGuid();
        var member2Id = Guid.NewGuid();

        var caseEntity = new Case { Id = caseId, OrganizationId = OrgId, Name = "Test Case" };

        // member1 is already assigned
        var existingAssignment = new CaseMember { CaseId = caseId, MemberId = member1Id };

        _caseRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Case, bool>>>()))
            .ReturnsAsync(caseEntity);

        _caseMemberRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<CaseMember, bool>>>()))
            .ReturnsAsync(new List<CaseMember> { existingAssignment });

        _memberRepoMock
            .Setup(r => r.GetByIdAsync(member2Id))
            .ReturnsAsync(new Member { Id = member2Id, OrganizationId = OrgId, LegalFirstName = "M2", LegalLastName = "L2" });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _sut.AssignMembersAsync(caseId, new[] { member1Id, member2Id });

        // Assert — only 1 new assignment (member2), member1 skipped
        _caseMemberRepoMock.Verify(
            r => r.AddRangeAsync(It.Is<IEnumerable<CaseMember>>(items => items.Count() == 1)),
            Times.Once);
    }

    // ─────────────────────────────────────────────────────────────
    // GetReadinessPercentageAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetCaseReadiness_CalculatesCorrectPercentage()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var checklistId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var caseEntity = new Case
        {
            Id = caseId,
            OrganizationId = OrgId,
            ChecklistId = checklistId,
            Name = "Test Case"
        };

        // 2 required checklist items
        var checklistItems = new List<ChecklistItem>
        {
            new() { ChecklistId = checklistId, DocumentType = "Passport", DocumentCategory = DocumentCategory.Identity, IsRequired = true },
            new() { ChecklistId = checklistId, DocumentType = "Bank Statement", DocumentCategory = DocumentCategory.Financial, IsRequired = true },
        };

        // 1 member assigned
        var caseMembers = new List<CaseMember>
        {
            new() { CaseId = caseId, MemberId = memberId }
        };

        // Member has 1 of 2 required documents verified
        var memberDocs = new List<Document>
        {
            new()
            {
                MemberId = memberId,
                OrganizationId = OrgId,
                DocumentType = "Passport",
                Category = DocumentCategory.Identity,
                Status = DocumentStatus.Verified,
                IsDeleted = false
            }
        };

        _caseRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Case, bool>>>()))
            .ReturnsAsync(caseEntity);

        _checklistItemRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<ChecklistItem, bool>>>()))
            .ReturnsAsync(checklistItems);

        _caseMemberRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<CaseMember, bool>>>()))
            .ReturnsAsync(caseMembers);

        _documentRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Document, bool>>>()))
            .ReturnsAsync(memberDocs);

        // Act
        var result = await _sut.GetReadinessPercentageAsync(caseId);

        // Assert — 1 fulfilled out of 2 required = 50%
        result.Should().Be(50.0);
    }

    [Fact]
    public async Task GetCaseReadiness_NoChecklist_ReturnsZero()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var caseEntity = new Case
        {
            Id = caseId,
            OrganizationId = OrgId,
            ChecklistId = null,
            Name = "No Checklist Case"
        };

        _caseRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Case, bool>>>()))
            .ReturnsAsync(caseEntity);

        // Act
        var result = await _sut.GetReadinessPercentageAsync(caseId);

        // Assert
        result.Should().Be(0);
    }

    // ─────────────────────────────────────────────────────────────
    // GrantAccessAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GrantAccess_ValidInput_CreatesAccessRecord()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var caseEntity = new Case { Id = caseId, OrganizationId = OrgId, Name = "Test Case" };

        _caseRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Case, bool>>>()))
            .ReturnsAsync(caseEntity);

        _caseAccessRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CaseAccess, bool>>>()))
            .ReturnsAsync((CaseAccess?)null); // No existing access

        _caseAccessRepoMock
            .Setup(r => r.AddAsync(It.IsAny<CaseAccess>()))
            .ReturnsAsync((CaseAccess ca) => ca);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new GrantAccessRequest
        {
            UserId = targetUserId,
            Role = UserRole.DocumentHandler,
            Permission = CaseAccessPermission.ViewDownload,
            ExpiresAt = new DateTime(2026, 12, 31)
        };

        // Act
        await _sut.GrantAccessAsync(caseId, request);

        // Assert
        _caseAccessRepoMock.Verify(r => r.AddAsync(It.Is<CaseAccess>(ca =>
            ca.CaseId == caseId &&
            ca.UserId == targetUserId &&
            ca.Permission == CaseAccessPermission.ViewDownload &&
            ca.Role == UserRole.DocumentHandler &&
            ca.GrantedBy == UserId &&
            ca.IsActive == true)), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GrantAccess_DuplicateAccess_ThrowsBusinessRuleException()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var caseEntity = new Case { Id = caseId, OrganizationId = OrgId, Name = "Test Case" };

        var existingAccess = new CaseAccess
        {
            CaseId = caseId,
            UserId = targetUserId,
            IsActive = true
        };

        _caseRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Case, bool>>>()))
            .ReturnsAsync(caseEntity);

        _caseAccessRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CaseAccess, bool>>>()))
            .ReturnsAsync(existingAccess);

        var request = new GrantAccessRequest
        {
            UserId = targetUserId,
            Role = UserRole.DocumentHandler,
            Permission = CaseAccessPermission.ViewDownload
        };

        // Act
        var act = () => _sut.GrantAccessAsync(caseId, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already has active access*");
    }

    // ─────────────────────────────────────────────────────────────
    // RevokeAccessAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task RevokeAccess_ValidId_DeactivatesAccess()
    {
        // Arrange
        var accessId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var access = new CaseAccess
        {
            Id = accessId,
            CaseId = caseId,
            UserId = targetUserId,
            IsActive = true,
            Permission = CaseAccessPermission.ViewDownload
        };

        var caseEntity = new Case
        {
            Id = caseId,
            OrganizationId = OrgId,
            Name = "Test Case"
        };

        _caseAccessRepoMock
            .Setup(r => r.GetByIdAsync(accessId))
            .ReturnsAsync(access);

        _caseRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Case, bool>>>()))
            .ReturnsAsync(caseEntity);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _sut.RevokeAccessAsync(accessId);

        // Assert
        access.IsActive.Should().BeFalse();
        access.RevokedAt.Should().NotBeNull();
        access.RevokedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        access.RevokedBy.Should().Be(UserId);

        _caseAccessRepoMock.Verify(r => r.Update(access), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RevokeAccess_NonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var accessId = Guid.NewGuid();

        _caseAccessRepoMock
            .Setup(r => r.GetByIdAsync(accessId))
            .ReturnsAsync((CaseAccess?)null);

        // Act
        var act = () => _sut.RevokeAccessAsync(accessId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*CaseAccess*{accessId}*");
    }
}

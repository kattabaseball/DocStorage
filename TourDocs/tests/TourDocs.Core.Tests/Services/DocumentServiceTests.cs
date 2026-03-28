using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourDocs.Core.DTOs.Documents;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Core.Services;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.Tests.Services;

public class DocumentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Document>> _documentRepoMock;
    private readonly Mock<IRepository<DocumentVersion>> _versionRepoMock;
    private readonly Mock<IRepository<Member>> _memberRepoMock;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ILogger<DocumentService>> _loggerMock;
    private readonly DocumentService _sut;

    private static readonly Guid OrgId = Guid.Parse("aaaa0000-0000-0000-0000-000000000001");
    private static readonly Guid UserId = Guid.Parse("aaaa0000-0000-0000-0000-000000000002");
    private static readonly Guid MemberId = Guid.Parse("aaaa0000-0000-0000-0000-000000000003");

    public DocumentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _documentRepoMock = new Mock<IRepository<Document>>();
        _versionRepoMock = new Mock<IRepository<DocumentVersion>>();
        _memberRepoMock = new Mock<IRepository<Member>>();
        _fileStorageMock = new Mock<IFileStorageService>();
        _tenantContextMock = new Mock<ITenantContext>();
        _auditServiceMock = new Mock<IAuditService>();
        _loggerMock = new Mock<ILogger<DocumentService>>();

        _unitOfWorkMock.Setup(u => u.Documents).Returns(_documentRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.DocumentVersions).Returns(_versionRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Members).Returns(_memberRepoMock.Object);

        _tenantContextMock.Setup(t => t.OrganizationId).Returns(OrgId);
        _tenantContextMock.Setup(t => t.UserId).Returns(UserId);

        _sut = new DocumentService(
            _unitOfWorkMock.Object,
            _fileStorageMock.Object,
            _tenantContextMock.Object,
            _auditServiceMock.Object,
            _loggerMock.Object);
    }

    // ─────────────────────────────────────────────────────────────
    // UploadAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task UploadDocument_ValidFile_CreatesDocumentAndVersion()
    {
        // Arrange
        var member = new Member
        {
            Id = MemberId,
            OrganizationId = OrgId,
            LegalFirstName = "Amal",
            LegalLastName = "Silva"
        };

        _memberRepoMock
            .Setup(r => r.GetByIdAsync(MemberId))
            .ReturnsAsync(member);

        var fileStream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var storedPath = $"storage/{OrgId}/{MemberId}/Identity/test_passport.pdf";

        _fileStorageMock
            .Setup(f => f.SaveFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), "passport.pdf"))
            .ReturnsAsync(storedPath);

        _documentRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Document>()))
            .ReturnsAsync((Document d) => d);

        _versionRepoMock
            .Setup(r => r.AddAsync(It.IsAny<DocumentVersion>()))
            .ReturnsAsync((DocumentVersion v) => v);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new UploadDocumentRequest
        {
            MemberId = MemberId,
            Category = DocumentCategory.Identity,
            DocumentType = "Passport",
            Title = "Amal Silva — Passport",
            ExpiryDate = new DateTime(2028, 6, 15),
            IsHardCopyNeeded = true,
            FileName = "passport.pdf",
            FileSize = 1_500_000,
            MimeType = "application/pdf",
            FileStream = fileStream
        };

        // Act
        var result = await _sut.UploadAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.MemberId.Should().Be(MemberId);
        result.Category.Should().Be(DocumentCategory.Identity);
        result.DocumentType.Should().Be("Passport");
        result.Title.Should().Be("Amal Silva — Passport");
        result.Status.Should().Be(DocumentStatus.Uploaded);
        result.ExpiryDate.Should().Be(new DateTime(2028, 6, 15));
        result.IsHardCopyNeeded.Should().BeTrue();
        result.CurrentVersionNumber.Should().Be(1);
        result.VersionCount.Should().Be(1);

        _documentRepoMock.Verify(r => r.AddAsync(It.IsAny<Document>()), Times.Once);
        _versionRepoMock.Verify(r => r.AddAsync(It.IsAny<DocumentVersion>()), Times.Once);
        _fileStorageMock.Verify(f => f.SaveFileAsync(fileStream, It.IsAny<string>(), "passport.pdf"), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UploadDocument_NonExistentMember_ThrowsNotFoundException()
    {
        // Arrange
        _memberRepoMock
            .Setup(r => r.GetByIdAsync(MemberId))
            .ReturnsAsync((Member?)null);

        var request = new UploadDocumentRequest
        {
            MemberId = MemberId,
            Category = DocumentCategory.Identity,
            DocumentType = "Passport",
            Title = "Test",
            FileName = "test.pdf",
            FileStream = new MemoryStream()
        };

        // Act
        var act = () => _sut.UploadAsync(request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Member*{MemberId}*");
    }

    // ─────────────────────────────────────────────────────────────
    // VerifyAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task VerifyDocument_ValidId_UpdatesStatusToVerified()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var document = new Document
        {
            Id = docId,
            MemberId = MemberId,
            OrganizationId = OrgId,
            Status = DocumentStatus.Uploaded,
            Title = "Amal Silva — Passport",
            DocumentType = "Passport",
            Category = DocumentCategory.Identity
        };

        var member = new Member
        {
            Id = MemberId,
            OrganizationId = OrgId,
            LegalFirstName = "Amal",
            LegalLastName = "Silva"
        };

        _documentRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Document, bool>>>()))
            .ReturnsAsync(document);

        _memberRepoMock
            .Setup(r => r.GetByIdAsync(MemberId))
            .ReturnsAsync(member);

        _versionRepoMock
            .Setup(r => r.CountAsync(It.IsAny<Expression<Func<DocumentVersion, bool>>>()))
            .ReturnsAsync(1);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _sut.VerifyAsync(docId, "Document verified against original.");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(DocumentStatus.Verified);
        result.VerificationNotes.Should().Be("Document verified against original.");
        result.VerifiedAt.Should().NotBeNull();
        result.VerifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        document.VerifiedBy.Should().Be(UserId);
        _documentRepoMock.Verify(r => r.Update(document), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task VerifyDocument_AlreadyVerified_ThrowsBusinessRuleException()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var document = new Document
        {
            Id = docId,
            OrganizationId = OrgId,
            Status = DocumentStatus.Verified,
            Title = "Already Verified"
        };

        _documentRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Document, bool>>>()))
            .ReturnsAsync(document);

        // Act
        var act = () => _sut.VerifyAsync(docId);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Cannot verify*");
    }

    // ─────────────────────────────────────────────────────────────
    // RejectAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task RejectDocument_ValidId_UpdatesStatusToRejected()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var document = new Document
        {
            Id = docId,
            MemberId = MemberId,
            OrganizationId = OrgId,
            Status = DocumentStatus.UnderReview,
            Title = "Dilini — Police Clearance",
            DocumentType = "Police Clearance",
            Category = DocumentCategory.Legal
        };

        var member = new Member
        {
            Id = MemberId,
            OrganizationId = OrgId,
            LegalFirstName = "Dilini",
            LegalLastName = "Jayawardena"
        };

        _documentRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Document, bool>>>()))
            .ReturnsAsync(document);

        _memberRepoMock
            .Setup(r => r.GetByIdAsync(MemberId))
            .ReturnsAsync(member);

        _versionRepoMock
            .Setup(r => r.CountAsync(It.IsAny<Expression<Func<DocumentVersion, bool>>>()))
            .ReturnsAsync(1);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _sut.RejectAsync(docId, "Certificate not apostilled.");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(DocumentStatus.Rejected);
        result.VerificationNotes.Should().Be("Certificate not apostilled.");

        _documentRepoMock.Verify(r => r.Update(document), Times.Once);
        _auditServiceMock.Verify(a => a.LogAsync(
            OrgId, UserId, "Document.Rejected", "Document", docId, It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    // ─────────────────────────────────────────────────────────────
    // GetExpiringDocumentsAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetExpiringDocuments_ReturnsDocumentsExpiringSoon()
    {
        // Arrange
        var expiringDoc = new Document
        {
            Id = Guid.NewGuid(),
            MemberId = MemberId,
            OrganizationId = OrgId,
            Status = DocumentStatus.Verified,
            Title = "Expiring Passport",
            DocumentType = "Passport",
            Category = DocumentCategory.Identity,
            ExpiryDate = DateTime.UtcNow.AddDays(15),
            IsDeleted = false
        };

        var member = new Member
        {
            Id = MemberId,
            OrganizationId = OrgId,
            LegalFirstName = "Test",
            LegalLastName = "Member"
        };

        _documentRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Document, bool>>>()))
            .ReturnsAsync(new List<Document> { expiringDoc });

        _memberRepoMock
            .Setup(r => r.GetByIdAsync(MemberId))
            .ReturnsAsync(member);

        _versionRepoMock
            .Setup(r => r.CountAsync(It.IsAny<Expression<Func<DocumentVersion, bool>>>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.GetExpiringDocumentsAsync(30);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Expiring Passport");
        result.First().ExpiryDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(15), TimeSpan.FromMinutes(5));
    }

    // ─────────────────────────────────────────────────────────────
    // DownloadAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task DownloadDocument_ValidId_ReturnsFileStream()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var versionId = Guid.NewGuid();
        var document = new Document
        {
            Id = docId,
            MemberId = MemberId,
            OrganizationId = OrgId,
            CurrentVersionId = versionId,
            Title = "Amal — Passport"
        };

        var version = new DocumentVersion
        {
            Id = versionId,
            DocumentId = docId,
            VersionNumber = 1,
            FilePath = "storage/test/passport.pdf",
            FileName = "passport.pdf"
        };

        var expectedStream = new MemoryStream(new byte[] { 1, 2, 3 });

        _documentRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Document, bool>>>()))
            .ReturnsAsync(document);

        _versionRepoMock
            .Setup(r => r.GetByIdAsync(versionId))
            .ReturnsAsync(version);

        _fileStorageMock
            .Setup(f => f.GetFileAsync("storage/test/passport.pdf"))
            .ReturnsAsync(expectedStream);

        // Act
        var result = await _sut.DownloadAsync(docId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(expectedStream);

        _auditServiceMock.Verify(a => a.LogAsync(
            OrgId, UserId, "Document.Downloaded", "Document", docId, It.IsAny<string>(),
            It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    // ─────────────────────────────────────────────────────────────
    // ReuploadAsync
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task ReuploadDocument_CreatesNewVersion()
    {
        // Arrange
        var docId = Guid.NewGuid();
        var document = new Document
        {
            Id = docId,
            MemberId = MemberId,
            OrganizationId = OrgId,
            Category = DocumentCategory.Identity,
            DocumentType = "Passport",
            Title = "Amal — Passport",
            Status = DocumentStatus.Rejected
        };

        var existingVersions = new List<DocumentVersion>
        {
            new()
            {
                Id = Guid.NewGuid(),
                DocumentId = docId,
                VersionNumber = 1,
                FilePath = "old/path.pdf"
            }
        };

        var member = new Member
        {
            Id = MemberId,
            OrganizationId = OrgId,
            LegalFirstName = "Amal",
            LegalLastName = "Silva"
        };

        var fileStream = new MemoryStream(new byte[] { 5, 6, 7, 8 });
        var newFilePath = $"storage/{OrgId}/{MemberId}/Identity/new_passport.pdf";

        _documentRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Document, bool>>>()))
            .ReturnsAsync(document);

        _versionRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<DocumentVersion, bool>>>()))
            .ReturnsAsync(existingVersions);

        _fileStorageMock
            .Setup(f => f.SaveFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), "new_passport.pdf"))
            .ReturnsAsync(newFilePath);

        _versionRepoMock
            .Setup(r => r.AddAsync(It.IsAny<DocumentVersion>()))
            .ReturnsAsync((DocumentVersion v) => v);

        _memberRepoMock
            .Setup(r => r.GetByIdAsync(MemberId))
            .ReturnsAsync(member);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new UploadDocumentRequest
        {
            MemberId = MemberId,
            FileName = "new_passport.pdf",
            FileSize = 2_000_000,
            MimeType = "application/pdf",
            FileStream = fileStream,
            ExpiryDate = new DateTime(2030, 1, 1)
        };

        // Act
        var result = await _sut.ReuploadAsync(docId, request);

        // Assert
        result.Should().NotBeNull();
        result.CurrentVersionNumber.Should().Be(2);
        result.VersionCount.Should().Be(2);
        result.Status.Should().Be(DocumentStatus.Uploaded); // Reset to Uploaded after reupload
        result.ExpiryDate.Should().Be(new DateTime(2030, 1, 1));

        _versionRepoMock.Verify(r => r.AddAsync(It.Is<DocumentVersion>(v => v.VersionNumber == 2)), Times.Once);
        _documentRepoMock.Verify(r => r.Update(document), Times.Once);
    }
}

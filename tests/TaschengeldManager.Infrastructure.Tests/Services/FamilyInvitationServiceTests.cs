using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Core.Interfaces.Services;
using TaschengeldManager.Infrastructure.Services;

namespace TaschengeldManager.Infrastructure.Tests.Services;

/// <summary>
/// Tests for FamilyInvitationService.
/// </summary>
public class FamilyInvitationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IFamilyRepository> _familyRepoMock;
    private readonly Mock<IFamilyInvitationRepository> _invitationRepoMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<FamilyInvitationService>> _loggerMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly FamilyInvitationService _sut;

    public FamilyInvitationServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _familyRepoMock = new Mock<IFamilyRepository>();
        _invitationRepoMock = new Mock<IFamilyInvitationRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _emailServiceMock = new Mock<IEmailService>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<FamilyInvitationService>>();
        _cacheServiceMock = new Mock<ICacheService>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Families).Returns(_familyRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.FamilyInvitations).Returns(_invitationRepoMock.Object);

        _sut = new FamilyInvitationService(
            _unitOfWorkMock.Object,
            _tokenServiceMock.Object,
            _emailServiceMock.Object,
            _configurationMock.Object,
            _loggerMock.Object,
            _cacheServiceMock.Object);
    }

    #region InviteToFamilyAsync Tests

    [Fact]
    public async Task InviteToFamilyAsync_WhenParentInvitesRelative_SendsEmail()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var parent = new User { Id = parentId, Nickname = "Vater", Role = UserRole.Parent };
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = parentId } },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };
        var request = new InviteRequest
        {
            Email = "oma@example.com",
            Role = UserRole.Relative,
            RelationshipDescription = "Oma"
        };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);
        _userRepoMock.Setup(r => r.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parent);
        _invitationRepoMock.Setup(r => r.GetPendingByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FamilyInvitation>());
        _tokenServiceMock.Setup(t => t.GenerateSecureToken(It.IsAny<int>()))
            .Returns("secure_token");
        _tokenServiceMock.Setup(t => t.HashToken(It.IsAny<string>()))
            .Returns("hashed_token");
        _configurationMock.Setup(c => c["App:BaseUrl"])
            .Returns("https://localhost:5001");

        // Act
        var result = await _sut.InviteToFamilyAsync(parentId, familyId, request);

        // Assert
        result.Should().NotBeNull();
        result.InvitedEmail.Should().Be("oma@example.com");
        result.InvitedRole.Should().Be(UserRole.Relative);
        result.Status.Should().Be(InvitationStatus.Pending);

        _invitationRepoMock.Verify(r => r.AddAsync(It.IsAny<FamilyInvitation>(), It.IsAny<CancellationToken>()), Times.Once);
        _emailServiceMock.Verify(e => e.SendInvitationEmailAsync(
            "oma@example.com",
            "Vater",
            "Familie Müller",
            It.Is<string>(s => s.Contains("secure_token")),
            "Relative",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InviteToFamilyAsync_WhenInvitingChild_ThrowsException()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = parentId } },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };
        var request = new InviteRequest { Email = "child@example.com", Role = UserRole.Child };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var act = () => _sut.InviteToFamilyAsync(parentId, familyId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot invite children. Use AddChild instead.");
    }

    [Fact]
    public async Task InviteToFamilyAsync_WhenPendingInvitationExists_ThrowsException()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = parentId } },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };
        var existingInvitation = new FamilyInvitation { Id = new FamilyInvitationId(Guid.NewGuid()), FamilyId = familyId, InvitedEmail = "oma@example.com" };
        var request = new InviteRequest { Email = "oma@example.com", Role = UserRole.Relative };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);
        _invitationRepoMock.Setup(r => r.GetPendingByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FamilyInvitation> { existingInvitation });

        // Act
        var act = () => _sut.InviteToFamilyAsync(parentId, familyId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("An invitation is already pending for this email");
    }

    #endregion
}

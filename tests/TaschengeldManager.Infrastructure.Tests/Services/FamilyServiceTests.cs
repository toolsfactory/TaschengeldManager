using FluentAssertions;
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
/// Tests for FamilyService (core CRUD operations only).
/// See also: FamilyInvitationServiceTests, ChildManagementServiceTests, FamilyMemberServiceTests
/// </summary>
public class FamilyServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IFamilyRepository> _familyRepoMock;
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<ILogger<FamilyService>> _loggerMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly FamilyService _sut;

    public FamilyServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _familyRepoMock = new Mock<IFamilyRepository>();
        _accountRepoMock = new Mock<IAccountRepository>();
        _loggerMock = new Mock<ILogger<FamilyService>>();
        _cacheServiceMock = new Mock<ICacheService>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Families).Returns(_familyRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Accounts).Returns(_accountRepoMock.Object);

        // Default setup for batch loading accounts (N+1 fix)
        _accountRepoMock.Setup(r => r.GetByUserIdsAsync(It.IsAny<IEnumerable<UserId>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account>());

        _sut = new FamilyService(
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _cacheServiceMock.Object);
    }

    #region CreateFamilyAsync Tests

    [Fact]
    public async Task CreateFamilyAsync_WhenParentCreates_ReturnsFamily()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User { Id = userId, Nickname = "Vater", Role = UserRole.Parent };
        var request = new CreateFamilyRequest { Name = "Familie Müller" };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.CreateFamilyAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Familie Müller");
        result.FamilyCode.Should().HaveLength(6);
        result.Parents.Should().HaveCount(1);
        result.Parents.First().IsPrimary.Should().BeTrue();

        _familyRepoMock.Verify(r => r.AddAsync(It.IsAny<Family>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateFamilyAsync_WhenUserIsChild_ThrowsException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User { Id = userId, Nickname = "Max", Role = UserRole.Child };
        var request = new CreateFamilyRequest { Name = "Familie Müller" };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var act = () => _sut.CreateFamilyAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Only parents can create families");
    }

    [Fact]
    public async Task CreateFamilyAsync_WhenUserNotFound_ThrowsException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var request = new CreateFamilyRequest { Name = "Familie Müller" };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _sut.CreateFamilyAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Only parents can create families");
    }

    #endregion

    #region GetFamilyAsync Tests

    [Fact]
    public async Task GetFamilyAsync_WhenParentHasAccess_ReturnsFamily()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = userId, IsPrimary = true } },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var result = await _sut.GetFamilyAsync(userId, familyId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(familyId.Value);
        result.Name.Should().Be("Familie Müller");
    }

    [Fact]
    public async Task GetFamilyAsync_WhenUserHasNoAccess_ReturnsNull()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var otherUserId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = otherUserId } },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var result = await _sut.GetFamilyAsync(userId, familyId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetFamilyAsync_WhenFamilyNotFound_ReturnsNull()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Family?)null);

        // Act
        var result = await _sut.GetFamilyAsync(userId, familyId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetFamiliesForUserAsync Tests

    [Fact]
    public async Task GetFamiliesForUserAsync_WhenParent_ReturnsFamilies()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User { Id = userId, Nickname = "Vater", Role = UserRole.Parent };
        var family = new Family
        {
            Id = new FamilyId(Guid.NewGuid()),
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = userId, IsPrimary = true } },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _familyRepoMock.Setup(r => r.GetFamiliesForParentAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Family> { family });

        // Act
        var result = await _sut.GetFamiliesForUserAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Familie Müller");
    }

    [Fact]
    public async Task GetFamiliesForUserAsync_WhenChild_ReturnsFamily()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var user = new User { Id = userId, Nickname = "Max", Role = UserRole.Child, FamilyId = familyId };
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent>(),
            Children = new List<User> { user },
            Relatives = new List<FamilyRelative>()
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var result = await _sut.GetFamiliesForUserAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Familie Müller");
    }

    [Fact]
    public async Task GetFamiliesForUserAsync_WhenUserNotFound_ReturnsEmpty()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.GetFamiliesForUserAsync(userId);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}

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
/// Tests for ChildManagementService.
/// </summary>
public class ChildManagementServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IFamilyRepository> _familyRepoMock;
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ILogger<ChildManagementService>> _loggerMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly ChildManagementService _sut;

    public ChildManagementServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _familyRepoMock = new Mock<IFamilyRepository>();
        _accountRepoMock = new Mock<IAccountRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _loggerMock = new Mock<ILogger<ChildManagementService>>();
        _cacheServiceMock = new Mock<ICacheService>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Families).Returns(_familyRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Accounts).Returns(_accountRepoMock.Object);

        // Default setup for batch loading accounts (N+1 fix)
        _accountRepoMock.Setup(r => r.GetByUserIdsAsync(It.IsAny<IEnumerable<UserId>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account>());

        _sut = new ChildManagementService(
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _loggerMock.Object,
            _cacheServiceMock.Object);
    }

    #region AddChildAsync Tests

    [Fact]
    public async Task AddChildAsync_WhenParentAddsChild_ReturnsChildDto()
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
        var request = new AddChildRequest { Nickname = "Max", Pin = "1234", InitialBalance = 10.00m };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);
        _passwordHasherMock.Setup(p => p.HashPin(It.IsAny<string>()))
            .Returns("hashed_pin");

        // Act
        var result = await _sut.AddChildAsync(parentId, familyId, request);

        // Assert
        result.Should().NotBeNull();
        result.Nickname.Should().Be("Max");
        result.Balance.Should().Be(10.00m);
        result.IsLocked.Should().BeFalse();

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _accountRepoMock.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddChildAsync_WhenNotParent_ThrowsUnauthorized()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var otherParentId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = otherParentId } },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };
        var request = new AddChildRequest { Nickname = "Max", Pin = "1234" };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var act = () => _sut.AddChildAsync(userId, familyId, request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Only parents can add children");
    }

    [Fact]
    public async Task AddChildAsync_WhenDuplicateNickname_ThrowsException()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var existingChild = new User { Id = new UserId(Guid.NewGuid()), Nickname = "Max", Role = UserRole.Child };
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = parentId } },
            Children = new List<User> { existingChild },
            Relatives = new List<FamilyRelative>()
        };
        var request = new AddChildRequest { Nickname = "Max", Pin = "1234" };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var act = () => _sut.AddChildAsync(parentId, familyId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A child with this nickname already exists in the family");
    }

    [Fact]
    public async Task AddChildAsync_WhenFamilyNotFound_ThrowsException()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var request = new AddChildRequest { Nickname = "Max", Pin = "1234" };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Family?)null);

        // Act
        var act = () => _sut.AddChildAsync(parentId, familyId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Family not found");
    }

    #endregion

    #region RemoveChildAsync Tests

    [Fact]
    public async Task RemoveChildAsync_WhenParentRemovesChild_LocksAccount()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var childId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var child = new User { Id = childId, Nickname = "Max", Role = UserRole.Child, FamilyId = familyId };
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = parentId } },
            Children = new List<User> { child },
            Relatives = new List<FamilyRelative>()
        };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        await _sut.RemoveChildAsync(parentId, familyId, childId);

        // Assert
        child.FamilyId.Should().BeNull();
        child.IsLocked.Should().BeTrue();
        child.LockReason.Should().Be("Removed from family");
        child.LockedByUserId.Should().Be(parentId);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveChildAsync_WhenNotParent_ThrowsUnauthorized()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var otherParentId = new UserId(Guid.NewGuid());
        var childId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = otherParentId } },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var act = () => _sut.RemoveChildAsync(userId, familyId, childId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Only parents can remove children");
    }

    [Fact]
    public async Task RemoveChildAsync_WhenChildNotInFamily_ThrowsException()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var childId = new UserId(Guid.NewGuid());
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

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var act = () => _sut.RemoveChildAsync(parentId, familyId, childId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Child not found in family");
    }

    #endregion
}

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Core.Interfaces.Services;
using TaschengeldManager.Infrastructure.Services;

namespace TaschengeldManager.Infrastructure.Tests.Services;

/// <summary>
/// Tests for FamilyMemberService.
/// </summary>
public class FamilyMemberServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IFamilyRepository> _familyRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ILogger<FamilyMemberService>> _loggerMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly FamilyMemberService _sut;

    public FamilyMemberServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _familyRepoMock = new Mock<IFamilyRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILogger<FamilyMemberService>>();
        _cacheServiceMock = new Mock<ICacheService>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Families).Returns(_familyRepoMock.Object);

        _sut = new FamilyMemberService(
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _emailServiceMock.Object,
            _loggerMock.Object,
            _cacheServiceMock.Object);
    }

    #region RemoveParentAsync Tests

    [Fact]
    public async Task RemoveParentAsync_WhenRemovingNonPrimaryParent_Succeeds()
    {
        // Arrange
        var primaryParentId = new UserId(Guid.NewGuid());
        var secondaryParentId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var secondaryParent = new FamilyParent { UserId = secondaryParentId, IsPrimary = false };
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent>
            {
                new() { UserId = primaryParentId, IsPrimary = true },
                secondaryParent
            },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        await _sut.RemoveParentAsync(primaryParentId, familyId, secondaryParentId);

        // Assert
        family.Parents.Should().HaveCount(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveParentAsync_WhenRemovingPrimaryParent_ThrowsException()
    {
        // Arrange
        var primaryParentId = new UserId(Guid.NewGuid());
        var secondaryParentId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent>
            {
                new() { UserId = primaryParentId, IsPrimary = true },
                new() { UserId = secondaryParentId, IsPrimary = false }
            },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative>()
        };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var act = () => _sut.RemoveParentAsync(secondaryParentId, familyId, primaryParentId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot remove the primary parent");
    }

    #endregion

    #region RemoveRelativeAsync Tests

    [Fact]
    public async Task RemoveRelativeAsync_WhenParentRemovesRelative_Succeeds()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var relativeId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var relative = new FamilyRelative { UserId = relativeId, RelationshipDescription = "Oma" };
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = parentId } },
            Children = new List<User>(),
            Relatives = new List<FamilyRelative> { relative }
        };

        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        await _sut.RemoveRelativeAsync(parentId, familyId, relativeId);

        // Assert
        family.Relatives.Should().BeEmpty();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveRelativeAsync_WhenRelativeNotFound_ThrowsException()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var relativeId = new UserId(Guid.NewGuid());
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
        var act = () => _sut.RemoveRelativeAsync(parentId, familyId, relativeId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Relative not found in family");
    }

    #endregion
}

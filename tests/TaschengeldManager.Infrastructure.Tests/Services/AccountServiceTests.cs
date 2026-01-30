using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Services;

namespace TaschengeldManager.Infrastructure.Tests.Services;

public class AccountServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IFamilyRepository> _familyRepoMock;
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly Mock<ILogger<AccountService>> _loggerMock;
    private readonly AccountService _sut;

    public AccountServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _accountRepoMock = new Mock<IAccountRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _familyRepoMock = new Mock<IFamilyRepository>();
        _transactionRepoMock = new Mock<ITransactionRepository>();
        _loggerMock = new Mock<ILogger<AccountService>>();

        _unitOfWorkMock.Setup(u => u.Accounts).Returns(_accountRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Families).Returns(_familyRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Transactions).Returns(_transactionRepoMock.Object);

        _sut = new AccountService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    #region GetMyAccountAsync Tests

    [Fact]
    public async Task GetMyAccountAsync_WhenChildHasAccount_ReturnsAccountDto()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var accountId = new AccountId(Guid.NewGuid());
        var user = new User
        {
            Id = userId,
            Nickname = "Max",
            Role = UserRole.Child
        };
        var account = new Account
        {
            Id = accountId,
            UserId = userId,
            Balance = 50.00m,
            InterestEnabled = true,
            InterestRate = 2.5m
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _accountRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _sut.GetMyAccountAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(accountId.Value);
        result.OwnerName.Should().Be("Max");
        result.Balance.Should().Be(50.00m);
        result.InterestEnabled.Should().BeTrue();
        result.InterestRate.Should().Be(2.5m);
    }

    [Fact]
    public async Task GetMyAccountAsync_WhenUserIsNotChild_ReturnsNull()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User
        {
            Id = userId,
            Nickname = "Parent",
            Role = UserRole.Parent
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.GetMyAccountAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetMyAccountAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.GetMyAccountAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region WithdrawAsync Tests

    [Fact]
    public async Task WithdrawAsync_WhenSufficientBalance_CreatesTransaction()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var accountId = new AccountId(Guid.NewGuid());
        var user = new User { Id = userId, Nickname = "Max", Role = UserRole.Child };
        var account = new Account { Id = accountId, UserId = userId, Balance = 100.00m };
        var request = new WithdrawRequest { Amount = 25.00m, Description = "Süßigkeiten", Category = "Snacks" };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _accountRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _sut.WithdrawAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(-25.00m);
        result.Type.Should().Be(TransactionType.Withdrawal);
        result.BalanceAfter.Should().Be(75.00m);
        result.Description.Should().Be("Süßigkeiten");
        result.Category.Should().Be("Snacks");

        _transactionRepoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_WhenInsufficientBalance_ThrowsException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var accountId = new AccountId(Guid.NewGuid());
        var user = new User { Id = userId, Nickname = "Max", Role = UserRole.Child };
        var account = new Account { Id = accountId, UserId = userId, Balance = 10.00m };
        var request = new WithdrawRequest { Amount = 25.00m };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _accountRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var act = () => _sut.WithdrawAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Insufficient balance");
    }

    [Fact]
    public async Task WithdrawAsync_WhenUserIsNotChild_ThrowsException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User { Id = userId, Nickname = "Parent", Role = UserRole.Parent };
        var request = new WithdrawRequest { Amount = 25.00m };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var act = () => _sut.WithdrawAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Only children can withdraw from their own accounts");
    }

    #endregion

    #region DepositAsync Tests

    [Fact]
    public async Task DepositAsync_WhenParentHasAccess_CreatesTransaction()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var childId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var accountId = new AccountId(Guid.NewGuid());

        var parent = new User { Id = parentId, Nickname = "Vater", Role = UserRole.Parent };
        var child = new User { Id = childId, Nickname = "Max", Role = UserRole.Child, FamilyId = familyId };
        var account = new Account { Id = accountId, UserId = childId, Balance = 50.00m };
        var family = new Family
        {
            Id = familyId,
            Name = "Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = parentId } }
        };
        var request = new DepositRequest { Amount = 20.00m, Description = "Taschengeld" };

        _accountRepoMock.Setup(r => r.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _userRepoMock.Setup(r => r.GetByIdAsync(childId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(child);
        _userRepoMock.Setup(r => r.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parent);
        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var result = await _sut.DepositAsync(parentId, accountId, request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(20.00m);
        result.Type.Should().Be(TransactionType.Deposit);
        result.BalanceAfter.Should().Be(70.00m);
        result.Description.Should().Be("Taschengeld");

        _transactionRepoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DepositAsync_WhenParentNotInFamily_ThrowsUnauthorized()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var otherParentId = new UserId(Guid.NewGuid());
        var childId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var accountId = new AccountId(Guid.NewGuid());

        var child = new User { Id = childId, Nickname = "Max", Role = UserRole.Child, FamilyId = familyId };
        var account = new Account { Id = accountId, UserId = childId, Balance = 50.00m };
        var family = new Family
        {
            Id = familyId,
            Name = "Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = otherParentId } }
        };
        var request = new DepositRequest { Amount = 20.00m };

        _accountRepoMock.Setup(r => r.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _userRepoMock.Setup(r => r.GetByIdAsync(childId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(child);
        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var act = () => _sut.DepositAsync(parentId, accountId, request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task DepositAsync_WhenAccountNotFound_ThrowsException()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var accountId = new AccountId(Guid.NewGuid());
        var request = new DepositRequest { Amount = 20.00m };

        _accountRepoMock.Setup(r => r.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        // Act
        var act = () => _sut.DepositAsync(parentId, accountId, request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Account not found");
    }

    #endregion

    #region GiftAsync Tests

    [Fact]
    public async Task GiftAsync_WhenRelativeHasAccess_CreatesTransaction()
    {
        // Arrange
        var relativeId = new UserId(Guid.NewGuid());
        var childId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var accountId = new AccountId(Guid.NewGuid());

        var relative = new User { Id = relativeId, Nickname = "Oma", Role = UserRole.Relative };
        var child = new User { Id = childId, Nickname = "Max", Role = UserRole.Child, FamilyId = familyId };
        var account = new Account { Id = accountId, UserId = childId, Balance = 50.00m };
        var family = new Family
        {
            Id = familyId,
            Name = "Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent>(),
            Relatives = new List<FamilyRelative> { new() { UserId = relativeId } }
        };
        var request = new GiftRequest { AccountId = accountId.Value, Amount = 50.00m, Description = "Zum Geburtstag" };

        _userRepoMock.Setup(r => r.GetByIdAsync(relativeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(relative);
        _accountRepoMock.Setup(r => r.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _userRepoMock.Setup(r => r.GetByIdAsync(childId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(child);
        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var result = await _sut.GiftAsync(relativeId, request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(50.00m);
        result.Type.Should().Be(TransactionType.Gift);
        result.BalanceAfter.Should().Be(100.00m);
        result.Description.Should().Be("Zum Geburtstag");
        result.CreatedByName.Should().Be("Oma");

        _transactionRepoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GiftAsync_WhenUserIsChild_ThrowsUnauthorized()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User { Id = userId, Nickname = "Child", Role = UserRole.Child };
        var request = new GiftRequest { AccountId = Guid.NewGuid(), Amount = 50.00m };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var act = () => _sut.GiftAsync(userId, request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Only parents and relatives can give gifts");
    }

    #endregion

    #region SetInterestAsync Tests

    [Fact]
    public async Task SetInterestAsync_WhenParentHasAccess_UpdatesInterest()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var childId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var accountId = new AccountId(Guid.NewGuid());

        var child = new User { Id = childId, Nickname = "Max", Role = UserRole.Child, FamilyId = familyId };
        var account = new Account { Id = accountId, UserId = childId, Balance = 100.00m, InterestEnabled = false };
        var family = new Family
        {
            Id = familyId,
            Name = "Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = parentId } }
        };
        var request = new SetInterestRequest { Enabled = true, InterestRate = 3.0m };

        _accountRepoMock.Setup(r => r.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _userRepoMock.Setup(r => r.GetByIdAsync(childId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(child);
        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var result = await _sut.SetInterestAsync(parentId, accountId, request);

        // Assert
        result.Should().NotBeNull();
        result.InterestEnabled.Should().BeTrue();
        result.InterestRate.Should().Be(3.0m);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetInterestAsync_WhenDisablingInterest_ClearsRate()
    {
        // Arrange
        var parentId = new UserId(Guid.NewGuid());
        var childId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var accountId = new AccountId(Guid.NewGuid());

        var child = new User { Id = childId, Nickname = "Max", Role = UserRole.Child, FamilyId = familyId };
        var account = new Account { Id = accountId, UserId = childId, Balance = 100.00m, InterestEnabled = true, InterestRate = 3.0m };
        var family = new Family
        {
            Id = familyId,
            Name = "Müller",
            FamilyCode = "ABC123",
            Parents = new List<FamilyParent> { new() { UserId = parentId } }
        };
        var request = new SetInterestRequest { Enabled = false };

        _accountRepoMock.Setup(r => r.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _userRepoMock.Setup(r => r.GetByIdAsync(childId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(child);
        _familyRepoMock.Setup(r => r.GetWithMembersAsync(familyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);

        // Act
        var result = await _sut.SetInterestAsync(parentId, accountId, request);

        // Assert
        result.Should().NotBeNull();
        result.InterestEnabled.Should().BeFalse();
        result.InterestRate.Should().BeNull();
    }

    #endregion
}

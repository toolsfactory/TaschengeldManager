using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Api.Tests;

/// <summary>
/// Integration tests for AccountController endpoints.
/// </summary>
public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AccountControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region GetAccount Tests

    [Fact]
    public async Task GetAccount_AsParent_ReturnsAccount()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Müller");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);
        var (child, account) = await builder.CreateChildWithAccountAsync(family, "Emma", initialBalance: 50m);

        _client.AuthenticateAs(parent);

        // Act
        var response = await _client.GetAsync($"/api/account/{account.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AccountDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(account.Id.Value);
        result.Balance.Should().Be(50m);
    }

    [Fact]
    public async Task GetAccount_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        _client.ClearAuthentication();

        // Act
        var response = await _client.GetAsync($"/api/account/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAccount_NonExistent_ReturnsNotFound()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync();
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);

        _client.AuthenticateAs(parent);

        // Act
        var response = await _client.GetAsync($"/api/account/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GetMyAccount Tests

    [Fact]
    public async Task GetMyAccount_AsChild_ReturnsOwnAccount()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Schmidt");
        var (child, account) = await builder.CreateChildWithAccountAsync(family, "Max", initialBalance: 25m);

        _client.AuthenticateAs(child);

        // Act
        var response = await _client.GetAsync("/api/account/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AccountDto>();
        result.Should().NotBeNull();
        result!.Balance.Should().Be(25m);
    }

    #endregion

    #region GetFamilyAccounts Tests

    [Fact]
    public async Task GetFamilyAccounts_AsParent_ReturnsAllAccounts()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Weber");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);
        await builder.CreateChildWithAccountAsync(family, "Anna", initialBalance: 10m);
        await builder.CreateChildWithAccountAsync(family, "Ben", initialBalance: 20m);

        _client.AuthenticateAs(parent);

        // Act
        var response = await _client.GetAsync($"/api/account/family/{family.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AccountDto>>();
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    #endregion

    #region Deposit Tests

    [Fact]
    public async Task Deposit_AsParent_IncreasesBalance()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Fischer");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);
        var (child, account) = await builder.CreateChildWithAccountAsync(family, "Lisa", initialBalance: 100m);

        _client.AuthenticateAs(parent);

        var request = new DepositRequest
        {
            Amount = 50m,
            Description = "Taschengeld"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/account/{account.Id}/deposit", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<TransactionDto>();
        result.Should().NotBeNull();
        result!.Amount.Should().Be(50m);
        result.Type.Should().Be(TransactionType.Deposit);
    }

    [Fact]
    public async Task Deposit_NegativeAmount_ReturnsBadRequest()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync();
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);
        var (_, account) = await builder.CreateChildWithAccountAsync(family, "Tom");

        _client.AuthenticateAs(parent);

        var request = new DepositRequest
        {
            Amount = -50m,
            Description = "Invalid"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/account/{account.Id}/deposit", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Withdraw Tests

    [Fact]
    public async Task Withdraw_AsChild_DecreasesBalance()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Schulz");
        var (child, account) = await builder.CreateChildWithAccountAsync(family, "Felix", initialBalance: 100m);

        _client.AuthenticateAs(child);

        var request = new WithdrawRequest
        {
            Amount = 30m,
            Description = "Süßigkeiten"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/withdraw", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<TransactionDto>();
        result.Should().NotBeNull();
        Math.Abs(result!.Amount).Should().Be(30m); // Withdrawals are stored as negative amounts
        result.Type.Should().Be(TransactionType.Withdrawal);
    }

    [Fact]
    public async Task Withdraw_InsufficientBalance_ReturnsBadRequest()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync();
        var (child, _) = await builder.CreateChildWithAccountAsync(family, "Lena", initialBalance: 10m);

        _client.AuthenticateAs(child);

        var request = new WithdrawRequest
        {
            Amount = 100m,
            Description = "Zu viel"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/withdraw", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Gift Tests

    [Fact]
    public async Task Gift_AsRelative_IncreasesBalance()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Becker");
        var relative = await builder.CreateRelativeAsync(
            family,
            email: $"oma_{Guid.NewGuid():N}@test.de",
            firstName: "Oma",
            lastName: "Becker");
        var (_, account) = await builder.CreateChildWithAccountAsync(family, "Sophie", initialBalance: 50m);

        _client.AuthenticateAs(relative);

        var request = new GiftRequest
        {
            AccountId = account.Id.Value,
            Amount = 25m,
            Description = "Geburtstagsgeschenk"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/gift", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<TransactionDto>();
        result.Should().NotBeNull();
        result!.Amount.Should().Be(25m);
        result.Type.Should().Be(TransactionType.Gift);
    }

    #endregion

    #region Transactions Tests

    [Fact]
    public async Task GetTransactions_ReturnsTransactionHistory()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Wagner");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);
        var (child, account) = await builder.CreateChildWithAccountAsync(family, "Tim", initialBalance: 0m);

        await builder.CreateTransactionAsync(account, 50m, TransactionType.Deposit, parent, "Taschengeld");
        await builder.CreateTransactionAsync(account, 10m, TransactionType.Withdrawal, child, "Eis");
        await builder.CreateTransactionAsync(account, 25m, TransactionType.Gift, parent, "Von Opa");

        _client.AuthenticateAs(parent);

        // Act
        var response = await _client.GetAsync($"/api/account/{account.Id}/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<TransactionDto>>();
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetTransactions_WithPagination_ReturnsLimitedResults()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync();
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);
        var (_, account) = await builder.CreateChildWithAccountAsync(family, "Nina", initialBalance: 0m);

        for (int i = 0; i < 10; i++)
        {
            await builder.CreateTransactionAsync(account, 5m, TransactionType.Deposit, parent, $"Zahlung {i + 1}");
        }

        _client.AuthenticateAs(parent);

        // Act
        var response = await _client.GetAsync($"/api/account/{account.Id}/transactions?limit=5&offset=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<TransactionDto>>();
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    #endregion

    #region Interest Tests

    [Fact]
    public async Task SetInterest_AsParent_UpdatesAccountSettings()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Hoffmann");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);
        var (_, account) = await builder.CreateChildWithAccountAsync(family, "Paul", initialBalance: 100m);

        _client.AuthenticateAs(parent);

        var request = new SetInterestRequest
        {
            Enabled = true,
            InterestRate = 2.5m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/account/{account.Id}/interest", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AccountDto>();
        result.Should().NotBeNull();
        result!.InterestRate.Should().Be(2.5m);
        result.InterestEnabled.Should().BeTrue();
    }

    #endregion
}

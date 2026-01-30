using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Api.Tests;

/// <summary>
/// Integration tests for FamilyController endpoints.
/// </summary>
public class FamilyControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public FamilyControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region CreateFamily Tests

    [Fact]
    public async Task CreateFamily_AsParentWithoutFamily_CreatesFamily()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: null);

        _client.AuthenticateAs(parent);

        var request = new CreateFamilyRequest
        {
            Name = "Neue Familie"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/family", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<FamilyDto>();
        result.Should().NotBeNull();
        result!.Name.Should().Be("Neue Familie");
        result.FamilyCode.Should().HaveLength(6);
    }

    [Fact]
    public async Task CreateFamily_AsParentWithExistingFamily_CreatesNewFamily()
    {
        // Arrange - API allows parents to create new families (they can belong to multiple)
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var existingFamily = await builder.CreateFamilyAsync("Bestehende Familie");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: existingFamily);

        _client.AuthenticateAs(parent);

        var request = new CreateFamilyRequest
        {
            Name = "Zweite Familie"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/family", request);

        // Assert - API allows creating new families
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateFamily_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        _client.ClearAuthentication();

        var request = new CreateFamilyRequest
        {
            Name = "Test Familie"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/family", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region GetFamily Tests

    [Fact]
    public async Task GetFamily_AsFamilyMember_ReturnsFamily()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Meier Familie");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);

        _client.AuthenticateAs(parent);

        // Act
        var response = await _client.GetAsync($"/api/family/{family.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<FamilyDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(family.Id.Value);
        result.Name.Should().Be("Meier Familie");
    }

    [Fact]
    public async Task GetFamily_NonMember_ReturnsNotFoundOrForbidden()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family1 = await builder.CreateFamilyAsync("Familie 1");
        var family2 = await builder.CreateFamilyAsync("Familie 2");
        var parentInFamily1 = await builder.CreateParentAsync(
            email: $"parent1_{Guid.NewGuid():N}@test.de",
            family: family1);

        _client.AuthenticateAs(parentInFamily1);

        // Act - Try to access family2
        var response = await _client.GetAsync($"/api/family/{family2.Id}");

        // Assert - Should return NotFound (user doesn't have access to see it exists)
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.Forbidden);
    }

    #endregion

    #region GetFamilies Tests

    [Fact]
    public async Task GetFamilies_ReturnsUsersFamilies()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Meine Familie");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);

        _client.AuthenticateAs(parent);

        // Act
        var response = await _client.GetAsync("/api/family");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<FamilyDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterThanOrEqualTo(1);
    }

    #endregion

    #region AddChild Tests

    [Fact]
    public async Task AddChild_AsParent_CreatesChildWithAccount()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Klein Familie");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);

        _client.AuthenticateAs(parent);

        var request = new AddChildRequest
        {
            Nickname = "NeuesKind",
            Pin = "1234"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/family/{family.Id}/children", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ChildDto>();
        result.Should().NotBeNull();
        result!.Nickname.Should().Be("NeuesKind");
    }

    [Fact]
    public async Task AddChild_WithShortPin_ReturnsBadRequest()
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

        var request = new AddChildRequest
        {
            Nickname = "Kind",
            Pin = "12" // Too short
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/family/{family.Id}/children", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Invite Tests

    [Fact]
    public async Task InviteToFamily_AsParent_CreatesInvitation()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Einladungs Familie");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);

        _client.AuthenticateAs(parent);

        var request = new InviteRequest
        {
            Email = $"oma_{Guid.NewGuid():N}@test.de",
            Role = UserRole.Relative
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/family/{family.Id}/invitations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<InvitationDto>();
        result.Should().NotBeNull();
        result!.InvitedRole.Should().Be(UserRole.Relative);
    }

    #endregion

    #region AcceptInvitation Tests

    [Fact]
    public async Task AcceptInvitation_WithInvalidToken_ReturnsBadRequest()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var user = await builder.CreateParentAsync(
            email: $"user_{Guid.NewGuid():N}@test.de",
            family: null);

        _client.AuthenticateAs(user);

        var request = new AcceptInvitationRequest
        {
            Token = "INVALID123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/family/invitations/accept", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region RemoveChild Tests

    [Fact]
    public async Task RemoveChild_AsParent_RemovesChild()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Entfernungs Familie");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);
        var (child, _) = await builder.CreateChildWithAccountAsync(family, "ZuEntfernen");

        _client.AuthenticateAs(parent);

        // Act
        var response = await _client.DeleteAsync($"/api/family/{family.Id}/children/{child.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    #endregion

    #region RemoveRelative Tests

    [Fact]
    public async Task RemoveRelative_AsParent_RemovesRelative()
    {
        // Arrange
        using var scope = _factory.CreateScope();
        var context = _factory.GetDbContext(scope);
        var builder = new TestDataBuilder(context);

        var family = await builder.CreateFamilyAsync("Familie mit Verwandten");
        var parent = await builder.CreateParentAsync(
            email: $"parent_{Guid.NewGuid():N}@test.de",
            family: family);
        var relative = await builder.CreateRelativeAsync(
            family,
            email: $"relative_{Guid.NewGuid():N}@test.de");

        _client.AuthenticateAs(parent);

        // Act
        var response = await _client.DeleteAsync($"/api/family/{family.Id}/relatives/{relative.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    #endregion
}

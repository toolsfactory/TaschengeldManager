using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TaschengeldManager.Core.Configuration;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Core.Interfaces.Services;
using TaschengeldManager.Infrastructure.Services;

namespace TaschengeldManager.Infrastructure.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IFamilyRepository> _familyRepoMock;
    private readonly Mock<ISessionRepository> _sessionRepoMock;
    private readonly Mock<ILoginAttemptRepository> _loginAttemptRepoMock;
    private readonly Mock<ITotpBackupCodeRepository> _backupCodeRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly Mock<IOptions<AuthSettings>> _authSettingsMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _familyRepoMock = new Mock<IFamilyRepository>();
        _sessionRepoMock = new Mock<ISessionRepository>();
        _loginAttemptRepoMock = new Mock<ILoginAttemptRepository>();
        _backupCodeRepoMock = new Mock<ITotpBackupCodeRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _authSettingsMock = new Mock<IOptions<AuthSettings>>();

        var authSettings = new AuthSettings
        {
            MaxFailedLoginAttempts = 5,
            LockoutDurationsMinutes = [5, 15, 60, 1440]
        };
        _authSettingsMock.Setup(x => x.Value).Returns(authSettings);

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Families).Returns(_familyRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Sessions).Returns(_sessionRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.LoginAttempts).Returns(_loginAttemptRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.TotpBackupCodes).Returns(_backupCodeRepoMock.Object);

        _sut = new AuthService(
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object,
            _authSettingsMock.Object);
    }

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_WhenEmailNotExists_CreatesUser()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "SecurePassword123!",
            Nickname = "TestUser"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password");
        _tokenServiceMock.Setup(t => t.GenerateMfaToken(It.IsAny<UserId>()))
            .Returns("mfa_setup_token");

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.MfaSetupRequired.Should().BeTrue();
        result.MfaSetupToken.Should().Be("mfa_setup_token");

        _userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Email == "test@example.com" &&
            u.NormalizedEmail == "TEST@EXAMPLE.COM" &&
            u.Role == UserRole.Parent), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ThrowsException()
    {
        // Arrange
        var existingUser = new User { Id = new UserId(Guid.NewGuid()), Email = "test@example.com" };
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "SecurePassword123!",
            Nickname = "TestUser"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var act = () => _sut.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already registered");
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WhenCredentialsValidAndNoMfa_ReturnsLoginResponse()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            Nickname = "TestUser",
            Role = UserRole.Parent,
            MfaEnabled = false,
            Passkeys = new List<Passkey>(),
            BiometricTokens = new List<BiometricToken>()
        };
        var request = new LoginRequest { Email = "test@example.com", Password = "password" };

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access_token");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
            .Returns("refresh_token");
        _tokenServiceMock.Setup(t => t.HashToken(It.IsAny<string>()))
            .Returns("hashed_refresh_token");

        // Act
        var result = await _sut.LoginAsync(request, "127.0.0.1", "TestBrowser");

        // Assert
        result.RequiresMfa.Should().BeFalse();
        result.LoginResponse.Should().NotBeNull();
        result.LoginResponse!.AccessToken.Should().Be("access_token");
        result.LoginResponse.RefreshToken.Should().Be("refresh_token");
        result.LoginResponse.User.Id.Should().Be(userId.Value);
    }

    [Fact]
    public async Task LoginAsync_WhenMfaEnabled_ReturnsMfaRequiredResponse()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            Nickname = "TestUser",
            Role = UserRole.Parent,
            MfaEnabled = true,
            TotpSecret = "totp_secret",
            Passkeys = new List<Passkey>(),
            BiometricTokens = new List<BiometricToken>()
        };
        var request = new LoginRequest { Email = "test@example.com", Password = "password" };

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateMfaToken(It.IsAny<UserId>()))
            .Returns("mfa_token");

        // Act
        var result = await _sut.LoginAsync(request, "127.0.0.1", "TestBrowser");

        // Assert
        result.RequiresMfa.Should().BeTrue();
        result.MfaResponse.Should().NotBeNull();
        result.MfaResponse!.MfaToken.Should().Be("mfa_token");
        result.MfaResponse.UserId.Should().Be(userId.Value);
        result.MfaResponse.AvailableMethods.Should().Contain(MfaMethod.Totp);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ThrowsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest { Email = "unknown@example.com", Password = "password" };

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _sut.LoginAsync(request, "127.0.0.1", "TestBrowser");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ThrowsUnauthorized()
    {
        // Arrange
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            Nickname = "TestUser"
        };
        var request = new LoginRequest { Email = "test@example.com", Password = "wrong_password" };

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        // Act
        var act = () => _sut.LoginAsync(request, "127.0.0.1", "TestBrowser");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_WhenAccountLocked_ThrowsUnauthorized()
    {
        // Arrange
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            Nickname = "TestUser",
            IsLocked = true,
            LockReason = "Too many failed attempts"
        };
        var request = new LoginRequest { Email = "test@example.com", Password = "password" };

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var act = () => _sut.LoginAsync(request, "127.0.0.1", "TestBrowser");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Account locked: Too many failed attempts");
    }

    #endregion

    #region ChildLoginAsync Tests

    [Fact]
    public async Task ChildLoginAsync_WhenCredentialsValid_ReturnsLoginResponse()
    {
        // Arrange
        var childId = new UserId(Guid.NewGuid());
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family { Id = familyId, Name = "Familie Müller", FamilyCode = "ABC123" };
        var child = new User
        {
            Id = childId,
            Nickname = "Max",
            PinHash = "hashed_pin",
            Role = UserRole.Child,
            FamilyId = familyId,
            MfaEnabled = false,
            Passkeys = new List<Passkey>(),
            BiometricTokens = new List<BiometricToken>()
        };
        var request = new ChildLoginRequest { FamilyCode = "ABC123", Nickname = "Max", Pin = "1234" };

        _familyRepoMock.Setup(r => r.GetByFamilyCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);
        _userRepoMock.Setup(r => r.GetByNicknameInFamilyAsync(familyId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(child);
        _passwordHasherMock.Setup(p => p.VerifyPin(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access_token");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
            .Returns("refresh_token");
        _tokenServiceMock.Setup(t => t.HashToken(It.IsAny<string>()))
            .Returns("hashed_refresh_token");

        // Act
        var result = await _sut.ChildLoginAsync(request, "127.0.0.1", "TestBrowser");

        // Assert
        result.RequiresMfa.Should().BeFalse();
        result.LoginResponse.Should().NotBeNull();
        result.LoginResponse!.User.Nickname.Should().Be("Max");
        result.LoginResponse.User.Role.Should().Be(UserRole.Child);
    }

    [Fact]
    public async Task ChildLoginAsync_WhenFamilyNotFound_ThrowsUnauthorized()
    {
        // Arrange
        var request = new ChildLoginRequest { FamilyCode = "INVALID", Nickname = "Max", Pin = "1234" };

        _familyRepoMock.Setup(r => r.GetByFamilyCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Family?)null);

        // Act
        var act = () => _sut.ChildLoginAsync(request, "127.0.0.1", "TestBrowser");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task ChildLoginAsync_WhenChildNotFound_ThrowsUnauthorized()
    {
        // Arrange
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family { Id = familyId, FamilyCode = "ABC123" };
        var request = new ChildLoginRequest { FamilyCode = "ABC123", Nickname = "Unknown", Pin = "1234" };

        _familyRepoMock.Setup(r => r.GetByFamilyCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);
        _userRepoMock.Setup(r => r.GetByNicknameInFamilyAsync(familyId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _sut.ChildLoginAsync(request, "127.0.0.1", "TestBrowser");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task ChildLoginAsync_WhenPinInvalid_ThrowsUnauthorized()
    {
        // Arrange
        var familyId = new FamilyId(Guid.NewGuid());
        var family = new Family { Id = familyId, FamilyCode = "ABC123" };
        var child = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Nickname = "Max",
            PinHash = "hashed_pin",
            Role = UserRole.Child,
            FamilyId = familyId
        };
        var request = new ChildLoginRequest { FamilyCode = "ABC123", Nickname = "Max", Pin = "wrong" };

        _familyRepoMock.Setup(r => r.GetByFamilyCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(family);
        _userRepoMock.Setup(r => r.GetByNicknameInFamilyAsync(familyId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(child);
        _passwordHasherMock.Setup(p => p.VerifyPin(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        // Act
        var act = () => _sut.ChildLoginAsync(request, "127.0.0.1", "TestBrowser");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact]
    public async Task RefreshTokenAsync_WhenValidToken_ReturnsNewTokens()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var sessionId = new SessionId(Guid.NewGuid());
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Nickname = "TestUser",
            Role = UserRole.Parent
        };
        var session = new Session
        {
            Id = sessionId,
            UserId = userId,
            RefreshTokenHash = "hashed_token",
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        var request = new RefreshTokenRequest { RefreshToken = "valid_refresh_token" };

        _tokenServiceMock.Setup(t => t.HashToken(It.IsAny<string>()))
            .Returns("hashed_token");
        _sessionRepoMock.Setup(r => r.GetByRefreshTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.GenerateAccessToken(It.IsAny<User>()))
            .Returns("new_access_token");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
            .Returns("new_refresh_token");

        // Act
        var result = await _sut.RefreshTokenAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new_access_token");
        result.RefreshToken.Should().Be("new_refresh_token");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenSessionExpired_ThrowsUnauthorized()
    {
        // Arrange
        var session = new Session
        {
            Id = new SessionId(Guid.NewGuid()),
            UserId = new UserId(Guid.NewGuid()),
            RefreshTokenHash = "hashed_token",
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddDays(-1) // Expired
        };
        var request = new RefreshTokenRequest { RefreshToken = "expired_token" };

        _tokenServiceMock.Setup(t => t.HashToken(It.IsAny<string>()))
            .Returns("hashed_token");
        _sessionRepoMock.Setup(r => r.GetByRefreshTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        // Act
        var act = () => _sut.RefreshTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid or expired refresh token");
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenSessionRevoked_ThrowsUnauthorized()
    {
        // Arrange
        var session = new Session
        {
            Id = new SessionId(Guid.NewGuid()),
            UserId = new UserId(Guid.NewGuid()),
            RefreshTokenHash = "hashed_token",
            IsRevoked = true,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        var request = new RefreshTokenRequest { RefreshToken = "revoked_token" };

        _tokenServiceMock.Setup(t => t.HashToken(It.IsAny<string>()))
            .Returns("hashed_token");
        _sessionRepoMock.Setup(r => r.GetByRefreshTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        // Act
        var act = () => _sut.RefreshTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid or expired refresh token");
    }

    #endregion

    #region LogoutAsync Tests

    [Fact]
    public async Task LogoutAsync_WhenValidSession_RevokesSession()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var session = new Session
        {
            Id = new SessionId(Guid.NewGuid()),
            UserId = userId,
            RefreshTokenHash = "hashed_token",
            IsRevoked = false
        };
        var refreshToken = "valid_token";

        _tokenServiceMock.Setup(t => t.HashToken(It.IsAny<string>()))
            .Returns("hashed_token");
        _sessionRepoMock.Setup(r => r.GetByRefreshTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        // Act
        await _sut.LogoutAsync(userId, refreshToken);

        // Assert
        session.IsRevoked.Should().BeTrue();
        session.RevokedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_WhenSessionNotFound_DoesNotThrow()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var refreshToken = "invalid_token";

        _tokenServiceMock.Setup(t => t.HashToken(It.IsAny<string>()))
            .Returns("hashed_token");
        _sessionRepoMock.Setup(r => r.GetByRefreshTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Session?)null);

        // Act
        var act = () => _sut.LogoutAsync(userId, refreshToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region LogoutAllAsync Tests

    [Fact]
    public async Task LogoutAllAsync_RevokesAllSessions()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        // Act
        await _sut.LogoutAllAsync(userId);

        // Assert
        _sessionRepoMock.Verify(r => r.RevokeAllAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetCurrentUserAsync Tests

    [Fact]
    public async Task GetCurrentUserAsync_WhenUserExists_ReturnsUserDto()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Nickname = "TestUser",
            Role = UserRole.Parent,
            MfaEnabled = true
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.GetCurrentUserAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId.Value);
        result.Email.Should().Be("test@example.com");
        result.Nickname.Should().Be("TestUser");
        result.Role.Should().Be(UserRole.Parent);
        result.MfaEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenUserNotFound_ReturnsNull()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.GetCurrentUserAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}

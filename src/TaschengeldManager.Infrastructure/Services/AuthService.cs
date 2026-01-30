using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaschengeldManager.Core.Configuration;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;
using TaschengeldManager.Infrastructure.Utilities;

namespace TaschengeldManager.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;
    private readonly AuthSettings _authSettings;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger,
        IOptions<AuthSettings> authSettings)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
        _authSettings = authSettings.Value;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();

        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Email = request.Email,
            NormalizedEmail = normalizedEmail,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Nickname = request.Nickname,
            Role = UserRole.Parent,
            MfaEnabled = false
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var mfaSetupToken = _tokenService.GenerateMfaToken(user.Id);

        _logger.LogInformation("User {UserId} registered successfully", user.Id.Value);

        return new RegisterResponse
        {
            UserId = user.Id.Value,
            MfaSetupToken = mfaSetupToken,
            MfaSetupRequired = true
        };
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        var user = await ValidateUserForLoginAsync(request.Email, ipAddress, userAgent, cancellationToken);

        await ValidatePasswordAsync(user, request.Password, ipAddress, userAgent, cancellationToken);

        return await CompleteLoginAsync(user, ipAddress, userAgent, cancellationToken);
    }

    /// <summary>
    /// Validates user exists and is not locked.
    /// </summary>
    private async Task<User> ValidateUserForLoginAsync(
        string email,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);

        if (user == null)
        {
            await LogLoginAttemptAsync(null, email, false, "User not found", ipAddress, userAgent, cancellationToken);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (user.IsLocked)
        {
            await LogLoginAttemptAsync(user.Id, email, false, "Account locked", ipAddress, userAgent, cancellationToken);
            throw new UnauthorizedAccessException($"Account locked: {user.LockReason}");
        }

        CheckTemporaryLockout(user);
        return user;
    }

    /// <summary>
    /// Validates password and handles failed attempts.
    /// </summary>
    private async Task ValidatePasswordAsync(
        User user,
        string password,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        if (user.PasswordHash == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            await LogLoginAttemptAsync(user.Id, user.Email ?? user.Nickname, false, "Invalid password", ipAddress, userAgent, cancellationToken);
            await RecordFailedAttemptAsync(user, cancellationToken);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        ResetFailedAttempts(user);
    }

    /// <summary>
    /// Completes login - either returns MFA required or creates session.
    /// </summary>
    private async Task<LoginResult> CompleteLoginAsync(
        User user,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        if (user.MfaEnabled)
        {
            var mfaToken = _tokenService.GenerateMfaToken(user.Id);
            var availableMethods = GetAvailableMfaMethods(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return LoginResult.MfaRequired(new MfaRequiredResponse
            {
                MfaToken = mfaToken,
                AvailableMethods = availableMethods,
                UserId = user.Id.Value
            });
        }

        var response = await CreateLoginResponseAsync(user, ipAddress, userAgent, cancellationToken);
        return LoginResult.Success(response);
    }

    public async Task<LoginResult> ChildLoginAsync(ChildLoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        var child = await ValidateChildForLoginAsync(request, ipAddress, userAgent, cancellationToken);

        await ValidatePinAsync(child, request.Pin, ipAddress, userAgent, cancellationToken);

        return await CompleteLoginAsync(child, ipAddress, userAgent, cancellationToken);
    }

    /// <summary>
    /// Validates child exists in family and is not locked.
    /// </summary>
    private async Task<User> ValidateChildForLoginAsync(
        ChildLoginRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        var loginIdentifier = $"{request.FamilyCode}/{request.Nickname}";

        var family = await _unitOfWork.Families.GetByFamilyCodeAsync(request.FamilyCode, cancellationToken);
        if (family == null)
        {
            await LogLoginAttemptAsync(null, loginIdentifier, false, "Family not found", ipAddress, userAgent, cancellationToken);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var child = await _unitOfWork.Users.GetByNicknameInFamilyAsync(family.Id, request.Nickname, cancellationToken);
        if (child == null)
        {
            await LogLoginAttemptAsync(null, loginIdentifier, false, "Child not found", ipAddress, userAgent, cancellationToken);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (child.IsLocked)
        {
            await LogLoginAttemptAsync(child.Id, request.Nickname, false, "Account locked", ipAddress, userAgent, cancellationToken);
            throw new UnauthorizedAccessException($"Account locked: {child.LockReason}");
        }

        CheckTemporaryLockout(child);
        return child;
    }

    /// <summary>
    /// Validates PIN and handles failed attempts.
    /// </summary>
    private async Task ValidatePinAsync(
        User child,
        string pin,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        if (child.PinHash == null || !_passwordHasher.VerifyPin(pin, child.PinHash))
        {
            await LogLoginAttemptAsync(child.Id, child.Nickname, false, "Invalid PIN", ipAddress, userAgent, cancellationToken);
            await RecordFailedAttemptAsync(child, cancellationToken);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        ResetFailedAttempts(child);
    }

    public async Task<LoginResponse> VerifyTotpAsync(VerifyTotpRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        var userId = _tokenService.ValidateMfaToken(request.MfaToken);
        if (userId == null)
        {
            throw new UnauthorizedAccessException("Invalid or expired MFA token");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Verify TOTP code
        if (!TotpHelper.VerifyCode(user.TotpSecret, request.Code))
        {
            // Try backup codes
            var backupCodes = await _unitOfWork.TotpBackupCodes.GetUnusedByUserAsync(user.Id, cancellationToken);
            var matchedBackupCode = backupCodes.FirstOrDefault(bc => _tokenService.VerifyToken(request.Code, bc.CodeHash));

            if (matchedBackupCode != null)
            {
                matchedBackupCode.IsUsed = true;
                matchedBackupCode.UsedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                await LogLoginAttemptAsync(user.Id, user.Email ?? user.Nickname, false, "Invalid TOTP code", ipAddress, userAgent, cancellationToken);
                throw new UnauthorizedAccessException("Invalid TOTP code");
            }
        }

        await LogLoginAttemptAsync(user.Id, user.Email ?? user.Nickname, true, null, ipAddress, userAgent, cancellationToken, "TOTP");

        return await CreateLoginResponseAsync(user, ipAddress, userAgent, cancellationToken);
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenService.HashToken(request.RefreshToken);
        var session = await _unitOfWork.Sessions.GetByRefreshTokenHashAsync(tokenHash, cancellationToken);

        if (session == null || session.IsRevoked || session.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(session.UserId, cancellationToken);
        if (user == null || user.IsLocked)
        {
            throw new UnauthorizedAccessException("User not found or locked");
        }

        // Update session
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        session.RefreshTokenHash = _tokenService.HashToken(newRefreshToken);
        session.LastActivityAt = DateTime.UtcNow;
        session.ExpiresAt = DateTime.UtcNow.AddDays(7);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = MapToUserDto(user)
        };
    }

    public async Task LogoutAsync(UserId userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenService.HashToken(refreshToken);
        var session = await _unitOfWork.Sessions.GetByRefreshTokenHashAsync(tokenHash, cancellationToken);

        if (session != null && session.UserId == userId)
        {
            session.IsRevoked = true;
            session.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("User {UserId} logged out", userId.Value);
    }

    public async Task LogoutAllAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Sessions.RevokeAllAsync(userId, cancellationToken);

        _logger.LogInformation("User {UserId} logged out from all devices", userId.Value);
    }

    public async Task<UserDto?> GetCurrentUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        return user == null ? null : MapToUserDto(user);
    }

    public Task<UserId?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        // This is typically handled by the JWT middleware
        // Keeping for potential use cases
        return Task.FromResult<UserId?>(null);
    }

    private async Task<LoginResponse> CreateLoginResponseAsync(User user, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var session = new Session
        {
            Id = new SessionId(Guid.NewGuid()),
            UserId = user.Id,
            RefreshTokenHash = _tokenService.HashToken(refreshToken),
            DeviceInfo = userAgent,
            IpAddress = ipAddress,
            LastActivityAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _unitOfWork.Sessions.AddAsync(session, cancellationToken);

        user.LastLoginAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} logged in successfully", user.Id.Value);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = MapToUserDto(user)
        };
    }

    private async Task LogLoginAttemptAsync(UserId? userId, string identifier, bool success, string? failureReason, string? ipAddress, string? userAgent, CancellationToken cancellationToken, string? mfaMethod = null)
    {
        var attempt = new LoginAttempt
        {
            Id = new LoginAttemptId(Guid.NewGuid()),
            UserId = userId,
            Identifier = identifier,
            Success = success,
            FailureReason = failureReason,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            MfaMethod = mfaMethod
        };

        await _unitOfWork.LoginAttempts.AddAsync(attempt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private IReadOnlyList<MfaMethod> GetAvailableMfaMethods(User user)
    {
        var methods = new List<MfaMethod>();

        if (!string.IsNullOrEmpty(user.TotpSecret))
        {
            methods.Add(MfaMethod.Totp);
        }

        if (user.Passkeys.Count > 0)
        {
            methods.Add(MfaMethod.Passkey);
        }

        if (user.BiometricTokens.Any(bt => bt.IsValid && bt.ExpiresAt > DateTime.UtcNow))
        {
            methods.Add(MfaMethod.Biometric);
        }

        if (user.Role == UserRole.Child)
        {
            methods.Add(MfaMethod.ParentApproval);
        }

        return methods;
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id.Value,
            Email = user.Email,
            Nickname = user.Nickname,
            Role = user.Role,
            MfaEnabled = user.MfaEnabled,
            FamilyId = user.FamilyId?.Value,
            SecurityTutorialCompleted = user.SecurityTutorialCompleted
        };
    }

    /// <summary>
    /// Checks if user is temporarily locked out and throws if so.
    /// </summary>
    private static void CheckTemporaryLockout(User user)
    {
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remainingMinutes = (int)Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
            throw new UnauthorizedAccessException($"Account temporarily locked. Try again in {remainingMinutes} minutes.");
        }
    }

    /// <summary>
    /// Records a failed login attempt and applies lockout if threshold is reached.
    /// </summary>
    private async Task RecordFailedAttemptAsync(User user, CancellationToken cancellationToken)
    {
        user.FailedLoginAttempts++;

        if (user.FailedLoginAttempts >= _authSettings.MaxFailedLoginAttempts)
        {
            // Calculate lockout duration based on how many times user has been locked out
            var lockoutCount = user.FailedLoginAttempts / _authSettings.MaxFailedLoginAttempts;
            var durationIndex = Math.Min(lockoutCount - 1, _authSettings.LockoutDurationsMinutes.Length - 1);
            var durationMinutes = _authSettings.LockoutDurationsMinutes[durationIndex];

            user.LockoutEnd = DateTime.UtcNow.AddMinutes(durationMinutes);

            _logger.LogWarning(
                "User {UserId} locked out for {Duration} minutes after {Attempts} failed attempts",
                user.Id.Value, durationMinutes, user.FailedLoginAttempts);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Resets failed login attempts on successful login.
    /// </summary>
    private static void ResetFailedAttempts(User user)
    {
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
    }
}

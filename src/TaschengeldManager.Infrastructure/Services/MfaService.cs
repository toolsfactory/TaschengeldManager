using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;
using TaschengeldManager.Infrastructure.Utilities;

namespace TaschengeldManager.Infrastructure.Services;

public class MfaService : IMfaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<MfaService> _logger;
    private const int BiometricTokenExpirationDays = 14;

    public MfaService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<MfaService> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<SetupTotpResponse> SetupTotpAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Generate secret
        var secret = TotpHelper.GenerateSecret();
        var setupToken = _tokenService.GenerateSecureToken();

        // Store temporarily (we'll save permanently after verification)
        // Using a special prefix to indicate it's pending verification
        user.TotpSecret = $"PENDING:{setupToken}:{secret}";
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate QR code data URL
        var issuer = "TaschengeldManager";
        var label = Uri.EscapeDataString(user.Email ?? user.Nickname);
        var otpauthUrl = $"otpauth://totp/{issuer}:{label}?secret={secret}&issuer={issuer}";
        var qrCodeDataUrl = GenerateQrCodeDataUrl(otpauthUrl);

        _logger.LogInformation("TOTP setup initiated for user {UserId}", userId.Value);

        return new SetupTotpResponse
        {
            Secret = secret,
            QrCodeDataUrl = qrCodeDataUrl,
            SetupToken = setupToken
        };
    }

    public async Task<bool> VerifyAndActivateTotpAsync(UserId userId, string setupToken, string code, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(user.TotpSecret) || !user.TotpSecret.StartsWith("PENDING:"))
        {
            return false;
        }

        var parts = user.TotpSecret.Split(':');
        if (parts.Length != 3 || parts[1] != setupToken)
        {
            return false;
        }

        var secret = parts[2];

        // Verify the code
        if (!TotpHelper.VerifyCode(secret, code))
        {
            return false;
        }

        // Activate TOTP
        user.TotpSecret = secret;
        user.MfaEnabled = true;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("TOTP activated for user {UserId}", userId.Value);

        return true;
    }

    public async Task<bool> VerifyTotpAsync(UserId userId, string code, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null || string.IsNullOrEmpty(user.TotpSecret) || user.TotpSecret.StartsWith("PENDING:"))
        {
            return false;
        }

        return TotpHelper.VerifyCode(user.TotpSecret, code);
    }

    public async Task<BackupCodesResponse> GenerateBackupCodesAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Delete existing backup codes
        await _unitOfWork.TotpBackupCodes.DeleteAllByUserAsync(userId, cancellationToken);

        // Generate new backup codes
        var codes = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            var code = GenerateBackupCode();
            codes.Add(code);

            var backupCode = new TotpBackupCode
            {
                Id = new TotpBackupCodeId(Guid.NewGuid()),
                UserId = userId,
                CodeHash = _tokenService.HashToken(code),
                IsUsed = false
            };

            await _unitOfWork.TotpBackupCodes.AddAsync(backupCode, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Backup codes generated for user {UserId}", userId.Value);

        return new BackupCodesResponse
        {
            BackupCodes = codes
        };
    }

    public async Task<bool> VerifyBackupCodeAsync(UserId userId, string code, CancellationToken cancellationToken = default)
    {
        var backupCodes = await _unitOfWork.TotpBackupCodes.GetUnusedByUserAsync(userId, cancellationToken);

        foreach (var backupCode in backupCodes)
        {
            if (_tokenService.VerifyToken(code, backupCode.CodeHash))
            {
                backupCode.IsUsed = true;
                backupCode.UsedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Backup code used for user {UserId}", userId.Value);
                return true;
            }
        }

        return false;
    }

    public async Task<EnableBiometricResponse> EnableBiometricAsync(UserId userId, EnableBiometricRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Invalidate existing token for this device
        await _unitOfWork.BiometricTokens.InvalidateByDeviceAsync(request.DeviceId, cancellationToken);

        // Generate new token
        var token = _tokenService.GenerateSecureToken(64);
        var expiresAt = DateTime.UtcNow.AddDays(BiometricTokenExpirationDays);

        var biometricToken = new BiometricToken
        {
            Id = new BiometricTokenId(Guid.NewGuid()),
            UserId = userId,
            DeviceId = request.DeviceId,
            DeviceName = request.DeviceName,
            Platform = request.Platform,
            BiometryType = request.BiometryType,
            TokenHash = _tokenService.HashToken(token),
            ExpiresAt = expiresAt,
            IsValid = true
        };

        await _unitOfWork.BiometricTokens.AddAsync(biometricToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Biometric enabled for user {UserId} on device {DeviceId}", userId.Value, request.DeviceId);

        return new EnableBiometricResponse
        {
            BiometricToken = token,
            ExpiresAt = expiresAt
        };
    }

    public async Task<LoginResponse?> BiometricLoginAsync(BiometricLoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        var biometricToken = await _unitOfWork.BiometricTokens.GetValidByDeviceAsync(request.DeviceId, cancellationToken);

        if (biometricToken == null || !_tokenService.VerifyToken(request.BiometricToken, biometricToken.TokenHash))
        {
            _logger.LogWarning("Invalid biometric login attempt for device {DeviceId}", request.DeviceId);
            return null;
        }

        var user = biometricToken.User;
        if (user == null || user.IsLocked)
        {
            return null;
        }

        // Update last used
        biometricToken.LastUsedAt = DateTime.UtcNow;

        // Create session
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
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsTrustedDevice = true
        };

        await _unitOfWork.Sessions.AddAsync(session, cancellationToken);

        user.LastLoginAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Biometric login successful for user {UserId}", user.Id.Value);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserDto
            {
                Id = user.Id.Value,
                Email = user.Email,
                Nickname = user.Nickname,
                Role = user.Role,
                MfaEnabled = user.MfaEnabled,
                FamilyId = user.FamilyId?.Value,
                SecurityTutorialCompleted = user.SecurityTutorialCompleted
            }
        };
    }

    public async Task DisableBiometricAsync(UserId userId, string deviceId, CancellationToken cancellationToken = default)
    {
        var token = await _unitOfWork.BiometricTokens.GetByDeviceAndUserAsync(deviceId, userId, cancellationToken);

        if (token != null)
        {
            token.IsValid = false;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Biometric disabled for user {UserId} on device {DeviceId}", userId.Value, deviceId);
    }

    public async Task<EnableBiometricResponse?> GetBiometricStatusAsync(UserId userId, string deviceId, CancellationToken cancellationToken = default)
    {
        var token = await _unitOfWork.BiometricTokens.GetByDeviceAndUserAsync(deviceId, userId, cancellationToken);

        if (token == null || !token.IsValid || token.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        return new EnableBiometricResponse
        {
            BiometricToken = "***", // Don't expose the actual token
            ExpiresAt = token.ExpiresAt
        };
    }

    private static string GenerateBackupCode()
    {
        var bytes = RandomNumberGenerator.GetBytes(5);
        var number = BitConverter.ToUInt32(bytes, 0) % 100000000;
        var code = number.ToString("D8");
        return $"{code[..4]}-{code[4..]}";
    }

    private static string GenerateQrCodeDataUrl(string content)
    {
        // For a production app, you'd use a QR code library like QRCoder
        // For now, return a placeholder that the frontend can use with a QR library
        return $"data:text/plain;charset=utf-8,{Uri.EscapeDataString(content)}";
    }
}

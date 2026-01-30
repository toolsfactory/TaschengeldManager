using System.Security.Claims;
using TaschengeldManager.Api.Extensions;
using TaschengeldManager.Api.Filters;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Api.Endpoints;

/// <summary>
/// Authentication endpoints.
/// </summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth")
            ;

        // Public endpoints with rate limiting
        group.MapPost("/register", Register)
            .WithSummary("Register a new parent user")
            .WithValidation<RegisterRequest>()
            .RequireRateLimiting("auth")
            .Produces<RegisterResponse>(200)
            .Produces(400);

        group.MapPost("/login", Login)
            .WithSummary("Login with email and password")
            .RequireRateLimiting("auth")
            .Produces<LoginResponse>(200)
            .Produces(401);

        group.MapPost("/login/child", ChildLogin)
            .WithSummary("Login as a child with family code, nickname, and PIN")
            .RequireRateLimiting("auth")
            .Produces<LoginResponse>(200)
            .Produces(401);

        group.MapPost("/mfa/verify", VerifyTotp)
            .WithSummary("Verify TOTP code and complete login")
            .RequireRateLimiting("auth")
            .Produces<LoginResponse>(200)
            .Produces(401);

        group.MapPost("/login/biometric", BiometricLogin)
            .WithSummary("Login with biometric token")
            .RequireRateLimiting("auth")
            .Produces<LoginResponse>(200)
            .Produces(401);

        group.MapPost("/refresh", RefreshToken)
            .WithSummary("Refresh access token")
            .Produces<LoginResponse>(200)
            .Produces(401);

        // Authorized endpoints
        group.MapPost("/mfa/totp/setup", SetupTotp)
            .WithSummary("Setup TOTP for the authenticated user")
            .RequireAuthorization()
            .Produces<SetupTotpResponse>(200);

        group.MapPost("/mfa/totp/activate", ActivateTotp)
            .WithSummary("Verify and activate TOTP setup")
            .RequireAuthorization()
            .Produces(200)
            .Produces(400);

        group.MapPost("/mfa/backup-codes", GenerateBackupCodes)
            .WithSummary("Generate backup codes for the authenticated user")
            .RequireAuthorization()
            .Produces<BackupCodesResponse>(200);

        group.MapPost("/mfa/biometric/enable", EnableBiometric)
            .WithSummary("Enable biometric authentication")
            .RequireAuthorization()
            .Produces<EnableBiometricResponse>(200);

        group.MapDelete("/mfa/biometric/{deviceId}", DisableBiometric)
            .WithSummary("Disable biometric authentication for a device")
            .RequireAuthorization()
            .Produces(204);

        group.MapPost("/logout", Logout)
            .WithSummary("Logout and invalidate the current session")
            .RequireAuthorization()
            .Produces(204);

        group.MapPost("/logout/all", LogoutAll)
            .WithSummary("Logout from all devices")
            .RequireAuthorization()
            .Produces(204);

        group.MapGet("/me", GetCurrentUser)
            .WithSummary("Get current user information")
            .RequireAuthorization()
            .Produces<UserDto>(200)
            .Produces(404);

        return app;
    }

    private static async Task<IResult> Register(
        RegisterRequest request,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.RegisterAsync(request, cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IAuthService authService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            var result = await authService.LoginAsync(request, ipAddress, userAgent, cancellationToken);
            return MapLoginResult(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> ChildLogin(
        ChildLoginRequest request,
        IAuthService authService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            var result = await authService.ChildLoginAsync(request, ipAddress, userAgent, cancellationToken);
            return MapLoginResult(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    /// <summary>
    /// Maps LoginResult to appropriate HTTP response.
    /// </summary>
    private static IResult MapLoginResult(LoginResult result)
    {
        if (result.RequiresMfa)
        {
            // Return MFA required response with 200 (not an error, just needs additional step)
            return Results.Ok(result.MfaResponse);
        }

        return Results.Ok(result.LoginResponse);
    }

    private static async Task<IResult> VerifyTotp(
        VerifyTotpRequest request,
        IAuthService authService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            var result = await authService.VerifyTotpAsync(request, ipAddress, userAgent, cancellationToken);
            return Results.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> SetupTotp(
        ClaimsPrincipal user,
        IMfaService mfaService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var result = await mfaService.SetupTotpAsync(userId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ActivateTotp(
        ActivateTotpRequest request,
        ClaimsPrincipal user,
        IMfaService mfaService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var success = await mfaService.VerifyAndActivateTotpAsync(userId, request.SetupToken, request.Code, cancellationToken);

        if (!success)
        {
            return Results.BadRequest(new { error = "Invalid code or setup token" });
        }

        return Results.Ok(new { message = "TOTP activated successfully" });
    }

    private static async Task<IResult> GenerateBackupCodes(
        ClaimsPrincipal user,
        IMfaService mfaService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var result = await mfaService.GenerateBackupCodesAsync(userId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> EnableBiometric(
        EnableBiometricRequest request,
        ClaimsPrincipal user,
        IMfaService mfaService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var result = await mfaService.EnableBiometricAsync(userId, request, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> BiometricLogin(
        BiometricLoginRequest request,
        IMfaService mfaService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        var result = await mfaService.BiometricLoginAsync(request, ipAddress, userAgent, cancellationToken);

        if (result == null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> DisableBiometric(
        string deviceId,
        ClaimsPrincipal user,
        IMfaService mfaService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        await mfaService.DisableBiometricAsync(userId, deviceId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> RefreshToken(
        RefreshTokenRequest request,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.RefreshTokenAsync(request, cancellationToken);
            return Results.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> Logout(
        LogoutRequest request,
        ClaimsPrincipal user,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        await authService.LogoutAsync(userId, request.RefreshToken, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> LogoutAll(
        ClaimsPrincipal user,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        await authService.LogoutAllAsync(userId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> GetCurrentUser(
        ClaimsPrincipal user,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var userDto = await authService.GetCurrentUserAsync(userId, cancellationToken);

        if (userDto == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(userDto);
    }
}

/// <summary>
/// Request to activate TOTP.
/// </summary>
public record ActivateTotpRequest
{
    public required string SetupToken { get; init; }
    public required string Code { get; init; }
}

/// <summary>
/// Request to logout.
/// </summary>
public record LogoutRequest
{
    public required string RefreshToken { get; init; }
}

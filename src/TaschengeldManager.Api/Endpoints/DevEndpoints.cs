using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Infrastructure.Data;
using TaschengeldManager.Infrastructure.Services;

namespace TaschengeldManager.Api.Endpoints;

/// <summary>
/// Development-only endpoints for seeding and managing test data.
/// These endpoints are only available in Development and Testing environments.
/// </summary>
public static class DevEndpoints
{
    public static IEndpointRouteBuilder MapDevEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dev")
            .WithTags("Dev")
            ;

        group.MapGet("/status", GetStatus)
            .WithSummary("Check if dev endpoints are available")
            .Produces(200)
            .Produces(404);

        group.MapPost("/seed", SeedDatabase)
            .WithSummary("Seed the database with test data")
            .Produces<SeedResult>(200)
            .Produces(404);

        group.MapPost("/reset", ResetDatabase)
            .WithSummary("Reset the database and reseed with test data")
            .Produces<SeedResult>(200)
            .Produces(404);

        group.MapPost("/migrate", ApplyMigrations)
            .WithSummary("Apply pending migrations")
            .Produces(200)
            .Produces(404);

        group.MapGet("/stats", GetDatabaseStats)
            .WithSummary("Get database statistics")
            .Produces(200)
            .Produces(404);

        group.MapGet("/credentials", GetTestCredentials)
            .WithSummary("Get test credentials for manual testing")
            .Produces<TestCredentials>(200)
            .Produces(404);

        group.MapDelete("/sessions", ClearSessions)
            .WithSummary("Clear all sessions")
            .Produces(200)
            .Produces(404);

        return app;
    }

    private static bool IsDevOrTestEnvironment(IWebHostEnvironment environment) =>
        environment.IsDevelopment() || environment.IsEnvironment("Testing");

    private static IResult GetStatus(IWebHostEnvironment environment)
    {
        if (!IsDevOrTestEnvironment(environment))
        {
            return Results.NotFound();
        }

        return Results.Ok(new
        {
            environment = environment.EnvironmentName,
            devEndpointsEnabled = true,
            timestamp = DateTime.UtcNow
        });
    }

    private static async Task<IResult> SeedDatabase(
        DevSeederService seederService,
        IWebHostEnvironment environment,
        ILogger<DevSeederService> logger,
        CancellationToken cancellationToken)
    {
        if (!IsDevOrTestEnvironment(environment))
        {
            return Results.NotFound();
        }

        logger.LogInformation("Seeding database with test data");
        var result = await seederService.SeedAsync(cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ResetDatabase(
        DevSeederService seederService,
        IWebHostEnvironment environment,
        ILogger<DevSeederService> logger,
        CancellationToken cancellationToken)
    {
        if (!IsDevOrTestEnvironment(environment))
        {
            return Results.NotFound();
        }

        logger.LogWarning("Resetting database and reseeding with test data");
        var result = await seederService.ResetAndSeedAsync(cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ApplyMigrations(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<ApplicationDbContext> logger,
        CancellationToken cancellationToken)
    {
        if (!IsDevOrTestEnvironment(environment))
        {
            return Results.NotFound();
        }

        logger.LogInformation("Applying pending migrations");

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
        var pendingList = pendingMigrations.ToList();

        if (pendingList.Count == 0)
        {
            return Results.Ok(new { message = "No pending migrations", appliedMigrations = Array.Empty<string>() });
        }

        await context.Database.MigrateAsync(cancellationToken);

        return Results.Ok(new
        {
            message = $"Applied {pendingList.Count} migrations",
            appliedMigrations = pendingList
        });
    }

    private static async Task<IResult> GetDatabaseStats(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        CancellationToken cancellationToken)
    {
        if (!IsDevOrTestEnvironment(environment))
        {
            return Results.NotFound();
        }

        var stats = new
        {
            users = await context.Users.CountAsync(cancellationToken),
            families = await context.Families.CountAsync(cancellationToken),
            accounts = await context.Accounts.CountAsync(cancellationToken),
            transactions = await context.Transactions.CountAsync(cancellationToken),
            sessions = await context.Sessions.CountAsync(cancellationToken),
            invitations = await context.FamilyInvitations.CountAsync(cancellationToken),
            passkeys = await context.Passkeys.CountAsync(cancellationToken),
            biometricTokens = await context.BiometricTokens.CountAsync(cancellationToken)
        };

        return Results.Ok(stats);
    }

    private static IResult GetTestCredentials(IWebHostEnvironment environment)
    {
        if (!IsDevOrTestEnvironment(environment))
        {
            return Results.NotFound();
        }

        return Results.Ok(new TestCredentials
        {
            Parent1 = new CredentialInfo { Email = "max.mueller@example.com", Password = "Test1234!" },
            Parent2 = new CredentialInfo { Email = "anna.mueller@example.com", Password = "Test1234!" },
            Parent3 = new CredentialInfo { Email = "peter.schmidt@example.com", Password = "Test1234!" },
            Relative = new CredentialInfo { Email = "oma.mueller@example.com", Password = "Test1234!" },
            Child1 = new ChildCredentialInfo { FamilyCode = "MUEL01", Nickname = "Tim", Pin = "1234" },
            Child2 = new ChildCredentialInfo { FamilyCode = "MUEL01", Nickname = "Lisa", Pin = "5678" }
        });
    }

    private static async Task<IResult> ClearSessions(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        CancellationToken cancellationToken)
    {
        if (!IsDevOrTestEnvironment(environment))
        {
            return Results.NotFound();
        }

        var count = await context.Sessions.CountAsync(cancellationToken);
        context.Sessions.RemoveRange(context.Sessions);
        await context.SaveChangesAsync(cancellationToken);

        return Results.Ok(new { message = $"Cleared {count} sessions" });
    }
}

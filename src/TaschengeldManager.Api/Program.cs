using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TaschengeldManager.Api.Endpoints;
using TaschengeldManager.Api.Middleware;
using TaschengeldManager.Api.Validators;
using StackExchange.Redis;
using TaschengeldManager.Core.Interfaces.Services;
using TaschengeldManager.Infrastructure;
using TaschengeldManager.Infrastructure.Data;
using TaschengeldManager.Infrastructure.Services;

namespace TaschengeldManager.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Infrastructure services (DbContext, Repositories, etc.)
        builder.Services.AddInfrastructure(builder.Configuration);

        // Add FluentValidation validators
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        // Add global exception handler
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // Add Aspire services only when not in Testing environment
        if (!builder.Environment.IsEnvironment("Testing"))
        {
            // Add Aspire service defaults
            builder.AddServiceDefaults();

            // Add Aspire PostgreSQL
            builder.AddNpgsqlDbContext<ApplicationDbContext>("taschengelddb");

            // Add Aspire Redis/Valkey
            builder.AddRedisClient("cache");

            // Replace NullCacheService with RedisCacheService when Redis is available
            builder.Services.AddSingleton<ICacheService>(sp =>
            {
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                var logger = sp.GetRequiredService<ILogger<RedisCacheService>>();
                return new RedisCacheService(redis, logger);
            });
        }

        // Configure JWT Authentication
        var jwtKey = GetJwtKey(builder.Configuration, builder.Environment);
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TaschengeldManager";
        var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TaschengeldManager";

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();

        // Add OpenAPI in Development and Testing environments
        if (IsDevOrTestEnvironment(builder.Environment))
        {
            builder.Services.AddOpenApi();
        }

        // Add CORS for web client
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                // Restrict to only required HTTP methods (REST API methods)
                var allowedMethods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH" };

                if (IsDevOrTestEnvironment(builder.Environment))
                {
                    // Development/Testing: Allow localhost on common ports
                    policy.SetIsOriginAllowed(origin =>
                    {
                        var uri = new Uri(origin);
                        return uri.Host == "localhost" || uri.Host == "127.0.0.1";
                    })
                    .WithMethods(allowedMethods)
                    .AllowAnyHeader()
                    .AllowCredentials();
                }
                else
                {
                    // Production: Only allow configured origins
                    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                    if (allowedOrigins == null || allowedOrigins.Length == 0)
                    {
                        throw new InvalidOperationException(
                            "CORS origins must be configured in production. Set 'Cors:AllowedOrigins' in configuration.");
                    }

                    policy.WithOrigins(allowedOrigins)
                          .WithMethods(allowedMethods)
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
            });
        });

        // Add Rate Limiting (only in production)
        if (!IsDevOrTestEnvironment(builder.Environment))
        {
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Strict policy for authentication endpoints (5 requests per minute per IP)
                options.AddPolicy("auth", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                // Standard policy for general API calls (100 requests per minute per IP)
                options.AddPolicy("standard", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));
            });
        }
        else
        {
            // In dev/test: add no-op rate limiter policies so RequireRateLimiting doesn't fail
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("auth", _ => RateLimitPartition.GetNoLimiter<string>(""));
                options.AddPolicy("standard", _ => RateLimitPartition.GetNoLimiter<string>(""));
            });
        }

        var app = builder.Build();

        // Apply migrations only in Development (not Testing, as it uses InMemory DB)
        if (app.Environment.IsDevelopment())
        {
            await ApplyMigrationsAsync(app);
        }

        // Map Aspire endpoints (only when not testing)
        if (!app.Environment.IsEnvironment("Testing"))
        {
            app.MapDefaultEndpoints();
        }

        // Configure the HTTP request pipeline.
        if (IsDevOrTestEnvironment(app.Environment))
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("TaschengeldManager API");
                options.WithTheme(ScalarTheme.BluePlanet);
            });
        }

        // Global exception handler - must be early in the pipeline
        app.UseExceptionHandler();

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        // Map Minimal API endpoints
        app.MapAuthEndpoints();
        app.MapAccountEndpoints();
        app.MapFamilyEndpoints();
        app.MapMoneyRequestEndpoints();
        app.MapRecurringPaymentEndpoints();
        app.MapSessionEndpoints();
        app.MapStatisticsEndpoints();
        app.MapDevEndpoints();

        await app.RunAsync();
    }

    /// <summary>
    /// Checks if we're in Development or Testing environment.
    /// </summary>
    private static bool IsDevOrTestEnvironment(IHostEnvironment environment) =>
        environment.IsDevelopment() || environment.IsEnvironment("Testing");

    /// <summary>
    /// Gets the JWT signing key with environment-appropriate security.
    /// </summary>
    private static string GetJwtKey(IConfiguration configuration, IHostEnvironment environment)
    {
        var configuredKey = configuration["Jwt:Key"];

        if (!string.IsNullOrEmpty(configuredKey))
        {
            // Validate key length (minimum 32 characters for HS256)
            if (configuredKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT key must be at least 32 characters long for security.");
            }
            return configuredKey;
        }

        // No key configured
        if (environment.IsDevelopment() || environment.IsEnvironment("Testing"))
        {
            // Development/Testing: Use a well-marked development key
            Console.WriteLine("WARNING: Using development JWT key. Do not use in production!");
            return "DEVELOPMENT-ONLY-JWT-KEY-DO-NOT-USE-IN-PRODUCTION-MIN-32-CHARS";
        }

        // Production: Require explicit configuration
        throw new InvalidOperationException(
            "JWT key must be configured in production. Set 'Jwt:Key' in configuration or environment variables.");
    }

    private static async Task ApplyMigrationsAsync(WebApplication app)
    {
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();

        const int maxRetries = 10;
        const int initialDelayMs = 1000;

        logger.LogInformation("Waiting for database to be ready...");

        for (var retry = 0; retry < maxRetries; retry++)
        {
            // Create a fresh scope for each attempt to get a clean DbContext
            using var scope = app.Services.CreateScope();

            try
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Test connection first
                var canConnect = await context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    throw new InvalidOperationException("Database connection test failed");
                }

                logger.LogInformation("Database connection established. Applying migrations...");

                // Get pending migrations for logging
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                var pendingList = pendingMigrations.ToList();

                if (pendingList.Count > 0)
                {
                    logger.LogInformation("Found {Count} pending migrations: {Migrations}",
                        pendingList.Count,
                        string.Join(", ", pendingList));
                }
                else
                {
                    logger.LogInformation("No pending migrations found");
                }

                // Apply migrations
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations completed successfully");

                // Auto-seed if SEED_ON_STARTUP=true environment variable is set
                var seedOnStartup = Environment.GetEnvironmentVariable("SEED_ON_STARTUP");
                if (string.Equals(seedOnStartup, "true", StringComparison.OrdinalIgnoreCase))
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<DevSeederService>();
                    var result = await seeder.SeedAsync();
                    if (result.Success)
                    {
                        logger.LogInformation("Database seeded successfully");
                    }
                    else
                    {
                        logger.LogInformation("Database seeding skipped: {Message}", result.Message);
                    }
                }

                // Success - exit the retry loop
                return;
            }
            catch (Exception ex) when (retry < maxRetries - 1)
            {
                // Calculate delay with exponential backoff (1s, 2s, 4s, 8s, ...)
                var delayMs = initialDelayMs * (int)Math.Pow(2, retry);
                logger.LogWarning(
                    "Database not ready (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}ms... Error: {Error}",
                    retry + 1,
                    maxRetries,
                    delayMs,
                    ex.Message);

                await Task.Delay(delayMs);
            }
            catch (Exception ex)
            {
                // Final attempt failed
                logger.LogError(ex, "Failed to connect to database after {MaxRetries} attempts", maxRetries);
                throw;
            }
        }
    }
}

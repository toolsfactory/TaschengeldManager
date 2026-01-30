using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Api.Tests;

/// <summary>
/// Custom WebApplicationFactory that replaces PostgreSQL with InMemory database
/// and removes Aspire-specific services for testing.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestJwtKey = "TestSecretKeyForIntegrationTestsOnly123456789!";
    private const string TestIssuer = "TaschengeldManager.Tests";
    private const string TestAudience = "TaschengeldManager.Tests";

    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Override configuration BEFORE services are configured
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testSettings = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = TestJwtKey,
                ["Jwt:Issuer"] = TestIssuer,
                ["Jwt:Audience"] = TestAudience,
                ["Jwt:AccessTokenExpirationMinutes"] = "15",
                ["Jwt:RefreshTokenExpirationDays"] = "7"
            };
            config.AddInMemoryCollection(testSettings);
        });

        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            // Remove hosted services (background jobs) for testing
            services.RemoveAll<IHostedService>();

            // Add InMemory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // Reconfigure JWT authentication with test values
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = TestIssuer,
                    ValidateAudience = true,
                    ValidAudience = TestAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Build a service provider to ensure database is created
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Remove Swagger-related assemblies from application parts
        builder.ConfigureServices(services =>
        {
            // Remove any Swagger-related services that might have been added
            var swaggerServices = services
                .Where(d => d.ServiceType?.FullName?.Contains("Swagger") == true ||
                           d.ImplementationType?.FullName?.Contains("Swagger") == true)
                .ToList();

            foreach (var service in swaggerServices)
            {
                services.Remove(service);
            }
        });

        return base.CreateHost(builder);
    }

    /// <summary>
    /// Creates a new scope with the database context.
    /// </summary>
    public IServiceScope CreateScope()
    {
        return Services.CreateScope();
    }

    /// <summary>
    /// Gets the database context for seeding test data.
    /// </summary>
    public ApplicationDbContext GetDbContext(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}

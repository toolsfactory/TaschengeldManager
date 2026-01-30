using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaschengeldManager.Core.Configuration;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;
using TaschengeldManager.Infrastructure.Data;
using TaschengeldManager.Infrastructure.Repositories;
using TaschengeldManager.Infrastructure.Services;

namespace TaschengeldManager.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Add infrastructure services (without DbContext - use Aspire for that).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.Configure<AuthSettings>(configuration.GetSection(AuthSettings.SectionName));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Core Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        // Cache Service (NullCacheService as default, replaced by RedisCacheService when Redis is available)
        services.AddSingleton<ICacheService, NullCacheService>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMfaService, MfaService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IFamilyService, FamilyService>();
        services.AddScoped<IFamilyInvitationService, FamilyInvitationService>();
        services.AddScoped<IChildManagementService, ChildManagementService>();
        services.AddScoped<IFamilyMemberService, FamilyMemberService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRecurringPaymentService, RecurringPaymentService>();
        services.AddScoped<IMoneyRequestService, MoneyRequestService>();
        services.AddScoped<IInterestService, InterestService>();
        services.AddScoped<IStatisticsService, StatisticsService>();

        // Background Services
        services.AddHostedService<RecurringPaymentBackgroundService>();
        services.AddHostedService<InterestBackgroundService>();

        // Dev Services (registered but only used in dev environment)
        services.AddScoped<DevSeederService>();

        return services;
    }

    /// <summary>
    /// Add infrastructure services with DbContext (for non-Aspire scenarios or tests).
    /// </summary>
    public static IServiceCollection AddInfrastructureWithDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("taschengelddb")));

        return services.AddInfrastructure(configuration);
    }
}

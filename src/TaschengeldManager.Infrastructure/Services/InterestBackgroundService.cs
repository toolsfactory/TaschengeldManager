using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Background service that calculates and credits due interest daily.
/// </summary>
public class InterestBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InterestBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6); // Check every 6 hours

    public InterestBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<InterestBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("InterestBackgroundService started");

        // Wait a bit before first execution to let the app start up
        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDueInterestAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing interest calculations");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("InterestBackgroundService stopped");
    }

    private async Task ProcessDueInterestAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var interestService = scope.ServiceProvider.GetRequiredService<IInterestService>();

        var processedCount = await interestService.ProcessDueInterestAsync(cancellationToken);

        if (processedCount > 0)
        {
            _logger.LogInformation("Processed interest for {Count} accounts", processedCount);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Background service that executes due recurring payments daily.
/// </summary>
public class RecurringPaymentBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecurringPaymentBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

    public RecurringPaymentBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RecurringPaymentBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RecurringPaymentBackgroundService started");

        // Wait a bit before first execution to let the app start up
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDuePaymentsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing recurring payments");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("RecurringPaymentBackgroundService stopped");
    }

    private async Task ProcessDuePaymentsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var recurringPaymentService = scope.ServiceProvider.GetRequiredService<IRecurringPaymentService>();

        var executedCount = await recurringPaymentService.ExecuteDuePaymentsAsync(cancellationToken);

        if (executedCount > 0)
        {
            _logger.LogInformation("Processed {Count} recurring payments", executedCount);
        }
    }
}

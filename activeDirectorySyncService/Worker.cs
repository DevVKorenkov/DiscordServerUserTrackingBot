using Microsoft.Extensions.Options;
using DsBot.ActiveDirectorySync.Services.Abstractions;
using DsBot.ConfigureSyncUserLib.Settings;

namespace DsBot.ActiveDirectorySync;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IUserService _userService;
    private readonly ServiceSettings _serviceSettings;

    public Worker(
        ILogger<Worker> logger,
        IUserService userService,
        IOptions<ServiceSettings> serviceSettings)
    {
        _logger = logger;
        _userService = userService;
        _serviceSettings = serviceSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Worker has been run at: {DateTime.Now}");

        while (!stoppingToken.IsCancellationRequested)
        {
            await _userService.UserSync(stoppingToken);

            await Task.Delay(TimeSpan.FromDays(_serviceSettings.WorkerRestartDelayDays), stoppingToken);
        }

        _logger.LogInformation($"Worker has been stopped at: {DateTime.Now}");
    }
}
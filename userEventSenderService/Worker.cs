using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings;
using DiscordServerUserTrackingBot.UserEventSenderService.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace DiscordServerUserTrackingBot.UserEventSenderService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IEventService _eventService;
    private readonly EventDelaySettings _serviceSettings;

    public Worker(
        ILogger<Worker> logger,
        IEventService eventService,
        IOptions<EventDelaySettings> serviceSettings)
    {
        _logger = logger;
        _eventService = eventService;
        _serviceSettings = serviceSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Worker was run at: {DateTime.Now}");

        while (!stoppingToken.IsCancellationRequested)
        {
            await _eventService.SendAllEvents(stoppingToken);

            _logger.LogInformation("Events were sent");

            await Task.Delay(TimeSpan.FromSeconds(_serviceSettings.WorkerRestartDelaySeconds));
        }

        _logger.LogInformation($"Worker has been stopped at: {DateTime.Now}");
    }
}

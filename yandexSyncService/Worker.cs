using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings;
using DiscordServerUserTrackingBot.YandexSyncService.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordServerUserTrackingBot.YandexSyncService
{
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
                await _userService.SetEmailsCacheAsync();

                await _userService.UserSync(stoppingToken);

                await Task.Delay(TimeSpan.FromDays(_serviceSettings.WorkerRestartDelayDays), stoppingToken);
            }

            _logger.LogInformation($"Worker has been stopped at: {DateTime.Now}");
        }
    }
}

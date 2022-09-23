using DiscordServerUserTrackingBot.SilentUsersRemovingService.Settings;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.SilentUsers.Abstractions;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.Delayed;

public class DelayedWorker : BackgroundService
{
    private readonly DelayedWorkerSettings _delayedWorkerSettings;
    private readonly ISilentUsersRemover _silentUsersRemover;

    public DelayedWorker(
        IOptions<DelayedWorkerSettings> delayedWorkerSettings,
        ISilentUsersRemover silentUsersRemover)
    {
        _delayedWorkerSettings = delayedWorkerSettings.Value;
        _silentUsersRemover = silentUsersRemover;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jobId = BackgroundJob.Schedule(
            () => _silentUsersRemover.ExecuteAsync(),
            TimeSpan.FromDays(_delayedWorkerSettings.DaysDelay));
    }
}

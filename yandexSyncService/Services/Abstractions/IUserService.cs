using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models;

namespace DiscordServerUserTrackingBot.YandexSyncService.Services.Abstractions;

public interface IUserService
{
    Task UserSync(CancellationToken cancellationToken);
    Task UserProcessing(UserCreds userCreds);
    Task SetEmailsCacheAsync();
}

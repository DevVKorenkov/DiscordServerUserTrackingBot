using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Infrastructure.Enums;
using DiscordServerUserTrackingBot.YandexSyncService.Models;

namespace DiscordServerUserTrackingBot.YandexSyncService.Infrastructure.Abstractions;

public interface IYandexRepository
{
    Task<UserStatus> GetUserStatusByEmail(string email);
    Task SetYandexEmailsCacheAsync();
    Task<List<YandexUser>> GetAllUsersFromYandex();
}


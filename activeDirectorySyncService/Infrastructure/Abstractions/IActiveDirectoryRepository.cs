using DsBot.ConfigureSyncUserLib.Infrastructure.Enums;

namespace DsBot.ActiveDirectorySync.Infrastructure.Abstractions;

public interface IActiveDirectoryRepository
{
    UserStatus GetUserStatus(string email);
}
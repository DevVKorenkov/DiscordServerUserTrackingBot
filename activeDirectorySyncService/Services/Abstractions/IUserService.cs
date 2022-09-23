using DsBot.ConfigureSyncUserLib.Models;

namespace DsBot.ActiveDirectorySync.Services.Abstractions
{
    public interface IUserService
    {
        Task UserSync(CancellationToken cancellationToken);
        Task CheckUser(UserCreds userCreds, CancellationToken cancellationToken);
    }
}

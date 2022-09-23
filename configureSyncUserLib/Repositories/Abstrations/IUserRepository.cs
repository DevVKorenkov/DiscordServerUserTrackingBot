using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;

public interface IUserRepository
{
    IAsyncEnumerable<User> IterateUsersAsync(CancellationToken stoppingToken);
    Task<User> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task ReplaceUserData(User user);
    Task UpdateUserAsync(
        User user, 
        Expression<Func<User, object>> userUpdateFn, 
        object value,
        params Func<IEventRepository, Task>[] eventEmiters);

    IAsyncEnumerable<User> IterateUsersAsync(
        Expression<Func<User, bool>> filter,
        CancellationToken cancellationToken);
}


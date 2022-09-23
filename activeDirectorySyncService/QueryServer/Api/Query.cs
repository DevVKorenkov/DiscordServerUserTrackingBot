using DsBot.ConfigureSyncUserLib.Repositories.Abstrations;

namespace DsBot.ActiveDirectorySync.QueryServer.Api;

public class Query
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<Query> _logger;

    public Query(IUserRepository userRepository, ILogger<Query> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    public List<ActiveUsers> GetActiveUsers()
    {
        return GetActiveUsersInternal().Result;
    }
    private async Task<List<ActiveUsers>> GetActiveUsersInternal()
    {
        try
        { 
            var activeUsers = new List<ActiveUsers>();
            await foreach(var user in _userRepository.IterateUsersAsync(CancellationToken.None))
            {
                activeUsers.Add(new ActiveUsers() {DiscordCliendId = user.DiscordId});
            }
            return activeUsers;
        }
        catch(Exception e)
        {
            _logger.LogError(e,"Active users loading error");
            throw;
        }
    }
}

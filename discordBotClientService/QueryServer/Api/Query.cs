using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord.Abstractions;
using Microsoft.Extensions.Logging;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.QueryServer.Api;

public class Query
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<Query> _logger;
    private readonly IBotClient _botClient;

    public Query(IUserRepository userRepository, ILogger<Query> logger, IBotClient botClient)
    {
        _userRepository = userRepository;
        _logger = logger;
        _botClient = botClient;
    }
    public List<ActiveUsers> GetActiveUsers()
    {
        return GetActiveUsersInternal().Result;
    }
    private async Task<List<ActiveUsers>> GetActiveUsersInternal()
    {
        try
        {
            var disCordUsers = await _botClient.GetAllUsersFromGuild();
            var activeUsers = new List<ActiveUsers>();
            foreach (var user in disCordUsers)
            {
                activeUsers.Add(new ActiveUsers { DiscordCliendId = user.Id });
            }
            return activeUsers;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Active users loading error");
            throw;
        }
    }
}


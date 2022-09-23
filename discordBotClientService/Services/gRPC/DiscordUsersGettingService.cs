//using DiscordServerUserTrackingBot.DiscordBotClientService.Discord.Abstractions;
//using Grpc.Core;

//namespace DiscordServerUserTrackingBot.DiscordBotClientService.Services.gRPC;

//public class DiscordUsersGettingService : GetUsers.GetUsersBase
//{
//    private readonly ILogger<DiscordUsersGettingService> _logger;
//    private readonly IBotClient _botClient;

//    public DiscordUsersGettingService(
//        ILogger<DiscordUsersGettingService> logger,
//        IBotClient botClient)
//    {
//        _logger = logger;
//        _botClient = botClient;
//    }

//    public async override Task<DiscordUserIdsResponce> GetAllServerUsers(DiscordUserIdsRequest request,
//        ServerCallContext context)
//    {
//        var discordUserIdsResponce = new DiscordUserIdsResponce();

//        var userIDsFromGuild = await _botClient.GetAllUsersFromGuild();

//        foreach (var u in userIDsFromGuild)
//        {
//            discordUserIdsResponce.DiscordUserId.Add(u.Id);
//        }

//        return Task.FromResult(discordUserIdsResponce);
//    }
//}
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord.Abstractions;
using DiscordServerUserTrackingBot.DiscordBotClientService.QueryServer.Api;
using Microsoft.AspNetCore.Mvc;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.Controllers;

[Route("DiscordUsers")]
public class DiscordUsersController : Controller
{
    private readonly IBotClient _botClient;
    private readonly ILogger<DiscordUsersController> _logger;

    public DiscordUsersController(IBotClient botClient)
    {
        _botClient = botClient;
    }

    [HttpGet]
    [Route("GetUsers")]
    public async Task<string[]> GetActiveUsers()
    {
        try
        {
            var disCordUsers = await _botClient.GetAllUsersFromGuild();

            return disCordUsers.Select(u => u.Id.ToString()).ToArray();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Active users loading error");
            throw;
        }
    }
}
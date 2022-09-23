using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Messages;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord.Abstractions;
using MassTransit;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.Consumers;

class AddRolesToNewDiscordUserConsumer : IConsumer<AddRolesToNewDiscordUserEvent>
{
    private readonly ILogger<AddRolesToNewDiscordUserConsumer> _logger;
    private readonly IBotClient _botClient;

    public AddRolesToNewDiscordUserConsumer(
        IBotClient botClient,
        ILogger<AddRolesToNewDiscordUserConsumer> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AddRolesToNewDiscordUserEvent> context)
    {
        try
        {
            await _botClient.GrantUserRoles(context.Message.DiscordClientId);
            _logger.LogInformation($"Message for user {context.Message.DiscordClientId} has been processed.");
        }
        catch (UserNotFoundException userNotFound)
        {
            _logger.LogWarning($"Discord user id {userNotFound.UserId} is not found");
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Error during discord command calling for user {context.Message.DiscordClientId}: {e.Message}");
        }
    }
}

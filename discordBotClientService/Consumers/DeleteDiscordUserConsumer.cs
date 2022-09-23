using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Messages;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord.Abstractions;
using MassTransit;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.Consumers;

class DeleteDiscordUserConsumer : IConsumer<DeleteDiscordUserEvent>
{
    private readonly ILogger<DeleteDiscordUserConsumer> _logger;
    private readonly IBotClient _botClient;

    public DeleteDiscordUserConsumer(
        IBotClient botClient,
        ILogger<DeleteDiscordUserConsumer> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteDiscordUserEvent> context)
    {
        try
        {
            await _botClient.RemoveUserFromGuild(context.Message.DiscordClientId);
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

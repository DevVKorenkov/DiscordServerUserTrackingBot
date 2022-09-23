using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Messages;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord.Abstractions;
using MassTransit;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.Consumers;

class DiscordUserWasArchivedConsumer : IConsumer<DiscordUserWasArchivedEvent>
{
    private readonly ILogger<DiscordUserWasArchivedConsumer> _logger;
    private readonly IBotClient _botClient;

    public DiscordUserWasArchivedConsumer(
        IBotClient botClient,
        ILogger<DiscordUserWasArchivedConsumer> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DiscordUserWasArchivedEvent> context)
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

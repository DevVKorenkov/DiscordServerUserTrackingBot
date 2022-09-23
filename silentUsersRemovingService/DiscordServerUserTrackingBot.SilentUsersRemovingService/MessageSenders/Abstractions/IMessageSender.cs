namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.MessageSenders.Abstractions;

public interface IMessageSender
{
    Task<bool> TrySendAsync<T>(T message, CancellationToken stoppingToken);
}

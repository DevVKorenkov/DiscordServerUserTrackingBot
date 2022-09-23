namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.MessageSenders;

public interface IMessageSender
{
    Task<bool> TrySendAsync<T>(T message, CancellationToken stoppingToken);
}

namespace DiscordServerUserTrackingBot.DiscordBotClientService.MessageSender.Abstractions
{
    public interface IMessageSender
    {
        Task<bool> TrySendAsync<T>(T message, CancellationToken cancellation);
    }
}

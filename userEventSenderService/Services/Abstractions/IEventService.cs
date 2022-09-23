namespace DiscordServerUserTrackingBot.UserEventSenderService.Services.Abstractions;

public interface IEventService
{
    Task SendAllEvents(CancellationToken cancellationToken);
}

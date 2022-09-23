using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Messages;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models;

namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;

public interface IEventRepository
{
    Task<long> Count(CancellationToken stoppingToken);
    Task PutEventToQueueAsync(UserEvent userEvent);
    Task RemoveEventFromQueueAsync(UserEvent userEvent);
    IAsyncEnumerable<UserEvent> IterateEventsAsync(CancellationToken cancellationToken);
}

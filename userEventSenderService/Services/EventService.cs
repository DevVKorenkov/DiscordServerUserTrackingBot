using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Infrastructure.Enums;
using Microsoft.Extensions.Logging;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Messages;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.MessageSenders;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models;
using DiscordServerUserTrackingBot.UserEventSenderService.Services.Abstractions;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;
using MassTransit;

namespace DiscordServerUserTrackingBot.UserEventSenderService.Services;

public class EventService : IEventService
{
    private readonly ILogger<EventService> _logger;
    private readonly IEventRepository _eventRepository;
    private readonly IBus _bus;

    public EventService(
        ILogger<EventService> logger,
        IEventRepository eventRepository,
        IBus bus)
    {
        _logger = logger;
        _eventRepository = eventRepository;
        _bus = bus;
    }

    public async Task SendAllEvents(CancellationToken cancellationToken)
    {
        if (await IsCollectionEmpty(cancellationToken))
        {
            return;
        }

        await foreach (var userEvent in _eventRepository.IterateEventsAsync(cancellationToken))
        {
            await SendUserEventFromQueue(userEvent);
        }
    }

    private async Task<bool> IsCollectionEmpty(CancellationToken cancellationToken)
        => await _eventRepository.Count(cancellationToken) <= 0;

    private async Task SendUserEventFromQueue(
        UserEvent userEvent)
    {
        if (!ulong.TryParse(userEvent.DiscordClientId, out var discordId))
        {
            _logger.LogWarning("DiscordId is invalid");
            await Task.CompletedTask;
        }

        if (userEvent.EventType == EventType.UserWasRegistered)
        {
            var addRolesEvent = new AddRolesToNewDiscordUserEvent(discordId);
            await _bus.Publish(addRolesEvent);
        }
        else
        {
            var archivedRolesEvent = new DiscordUserWasArchivedEvent(discordId);
            await _bus.Publish(archivedRolesEvent);
        }

        await _eventRepository.RemoveEventFromQueueAsync(userEvent);

        _logger.LogInformation($"User event {userEvent.DiscordClientId} was deleted from DB at: {DateTimeOffset.Now}");
    }


}

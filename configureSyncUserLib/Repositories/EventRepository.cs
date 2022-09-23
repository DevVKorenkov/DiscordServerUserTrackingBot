using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
using MongoDB.Bson;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models;

namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IMongoCollection<UserEvent> _mongoEventsCollection;

    public EventRepository(
        IOptions<EventDbSettings> settings,
        IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(settings.Value.DataBaseName);
        _mongoEventsCollection =
            database.GetCollection<UserEvent>(settings.Value.CollectionName);
    }

    public async Task PutEventToQueueAsync(UserEvent userEvent)
    {
        await _mongoEventsCollection.InsertOneAsync(userEvent, null);
    }

    public async Task RemoveEventFromQueueAsync(
        UserEvent userEvent)
    {
        await _mongoEventsCollection.DeleteOneAsync(
            uEvent => uEvent.DiscordClientId == userEvent.DiscordClientId);
    }

    public async Task<long> Count(CancellationToken stoppingToken)
    {
        return await _mongoEventsCollection.CountDocumentsAsync(
            Builders<UserEvent>.Filter.Where(d => true),
            null,
            stoppingToken);
    }

    public async IAsyncEnumerable<UserEvent> IterateEventsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var cursor = await _mongoEventsCollection.FindAsync(new BsonDocument(), null, cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var userEvent in cursor.Current)
            {
                yield return userEvent;
            };
        }
    }
}

using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Infrastructure.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models;
public record UserEvent(string DiscordClientId, EventType EventType)
{
    [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
    private ObjectId? _id { get; set; }
}

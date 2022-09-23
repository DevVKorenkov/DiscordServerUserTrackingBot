using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.UsersStorage;

public record RequestedUser(string DiscordId)
{
    [BsonIgnoreIfDefault]
    [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
    private ObjectId? _id { get; set; }
}

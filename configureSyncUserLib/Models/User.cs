using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Infrastructure.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models
{
    public record User(string DiscordId, string Email, UserStatus UserStatus)
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        private ObjectId? _id { get; set; }

        public string DiscordId { get; set; } = DiscordId;
        public string Email { get; set; } = Email;
        public UserStatus UserStatus { get; set; } = UserStatus;
    }
}

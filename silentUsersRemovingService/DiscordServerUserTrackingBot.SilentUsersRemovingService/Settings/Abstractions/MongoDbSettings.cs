namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.Settings.Abstractions;

public abstract class MongoDbSettings
{
    public string DataBaseName { get; set; }
    
    public string CollectionName { get; set; }
}

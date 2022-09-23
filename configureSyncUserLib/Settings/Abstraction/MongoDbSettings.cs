namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings.Abstraction;

public abstract class MongoDbSettings
{
    public string DataBaseName { get; set; }
    
    public string CollectionName { get; set; }
}

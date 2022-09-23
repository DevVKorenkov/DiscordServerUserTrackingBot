namespace DiscordServerUserTrackingBot.DiscordBotClientService.Settings;

public class RabbitMqSettings
{
    public string Url { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public int RetryCount { get; set; }
    public int RetryIntervalMs { get; set; }
    public int TimeoutMs { get; set; }
}

namespace DiscordServerUserTrackingBot.DiscordBotClientService.Consumers;

public class WrongDiscordUserIdStringException : Exception
{
    public string WrongString { get; }
    public WrongDiscordUserIdStringException(string wrongString)
    {
        WrongString = wrongString;
    }
}

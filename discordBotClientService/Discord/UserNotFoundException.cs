namespace DiscordServerUserTrackingBot.DiscordBotClientService.Discord;

public class UserNotFoundException : Exception
{
    public ulong UserId { get; }

    public UserNotFoundException(ulong userId)
    {
        UserId = userId;
    }
}

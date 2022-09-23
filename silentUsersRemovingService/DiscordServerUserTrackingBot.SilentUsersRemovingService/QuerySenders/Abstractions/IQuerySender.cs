namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.QuerySenders.Abstractions;

public interface IQuerySender
{
    public Task<TResponseType> SendQuery<TResponseType>(string query);
}

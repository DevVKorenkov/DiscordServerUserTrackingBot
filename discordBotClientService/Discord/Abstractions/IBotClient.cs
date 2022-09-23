using Discord;
using Discord.WebSocket;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.Discord.Abstractions
{
    public interface IBotClient
    {
        Task GrantUserRoles(ulong userId);
        Task RemoveUserFromGuild(ulong userId);
        //Task RemoveUserRoles(ulong userId);
        Task SendMessageAsync(ulong userId, string messageText);
        Task<IReadOnlyCollection<IGuildUser>> GetAllUsersFromGuild();
    }
}

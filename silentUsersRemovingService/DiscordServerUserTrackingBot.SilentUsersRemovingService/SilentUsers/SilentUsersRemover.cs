
using System.Net.Http.Json;
using System.Text;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Messages;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;
//using DiscordServerUserTrackingBot.SilentUsersRemovingService.MessageSenders.Abstractions;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.QuerySenders.Abstractions;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.SilentUsers.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.SilentUsers;

public class SilentUsersRemover: ISilentUsersRemover
{
    private readonly IUserRepository _requestedUserRepository;
    private readonly IBus _bus;
    //private readonly IQuerySender _querySender;
    private readonly ILogger<SilentUsersRemover> _logger;
    private readonly HttpClient _httpClient;

    public SilentUsersRemover(
        IUserRepository requestedUserRepository,
        IBus bus, 
        //IQuerySender querySender,
        ILogger<SilentUsersRemover> logger,
        IHttpClientFactory httpFactory)
    {
        _requestedUserRepository = requestedUserRepository;
        _bus = bus;
        //_querySender = querySender;
        _logger = logger;
        _httpClient = httpFactory.CreateClient("UsersRequest");
    }

    public async Task ExecuteAsync()
    {
        try
        {
            var allDiscordUsers = await GetAuthenticatedUsers();

            _logger.LogInformation("All Users from server {allDiscordUsers}", allDiscordUsers.Aggregate(
                new StringBuilder(), (builder, text) =>
                {
                    builder.Append($"{text} ");
                    return builder;
                },
                builder => builder.ToString()));

            var alreadyAuthorizedUsers = await GetAuthenticateRequestedUsers();

            _logger.LogInformation("Authorized users {alreadyAuthorizedUsers}", alreadyAuthorizedUsers.Aggregate(
                new StringBuilder(), (builder, text) =>
                {
                    builder.Append($"{text} ");
                    return builder;
                },
                builder => builder.ToString()));

            var nonAuthenticatedUsers = allDiscordUsers.Except(alreadyAuthorizedUsers).Distinct();

            foreach (var nonAuthenticatedDiscordId in nonAuthenticatedUsers)
            {
                if (ulong.TryParse(nonAuthenticatedDiscordId, out var nonAuthenticatedUserId))
                {
                    await _bus.Publish(new DeleteDiscordUserEvent(nonAuthenticatedUserId));

                    _logger.LogInformation("A Delete Event was sent. User id {nonAuthenticatedUserId}", nonAuthenticatedUserId);
                }
                else
                {
                    _logger.LogWarning("DiscordId is invalid");
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "an exception during non-authenticated users processing: ");
        }
    }

    private async Task<List<string>> GetAuthenticateRequestedUsers()
    {
        var authenticateRequestedUsers = new List<string>();
        await foreach (var user in _requestedUserRepository.IterateUsersAsync(CancellationToken.None))
        {
            authenticateRequestedUsers.Add(user.DiscordId);
        }

        return authenticateRequestedUsers;
    }

    private async Task<string[]> GetAuthenticatedUsers()
    {
       return await _httpClient.GetFromJsonAsync<string[]>(_httpClient.BaseAddress);

        //var queryResult =
        //    await _querySender.SendQuery<ActiveUsersResponse>(
        //    @"
        //    {
        //        activeUsers {
        //            discordCliendId
        //        }
        //    }");
        //var authenticatedUsers = queryResult.ActiveUsers.Select(x => x.DiscordCliendId.ToString());
        //return authenticatedUsers;
    }
}

using DiscordServerUserTrackingBot.SilentUsersRemovingService.QuerySenders.Abstractions;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.Settings;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Options;

namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.QuerySenders;

public class GraphQLQuerySender : IQuerySender
{
    private readonly GraphQLClientSettings _graphQlClientSettings;

    public GraphQLQuerySender(IOptions<GraphQLClientSettings> graphQLClientSettings)
    {
        _graphQlClientSettings = graphQLClientSettings.Value;
    }

    public async Task<TResponseType> SendQuery<TResponseType>(string query)
    {
        var graphQLClient = 
            new GraphQLHttpClient(
                _graphQlClientSettings.ServerUrl, 
                new NewtonsoftJsonSerializer());

        var heroRequest = 
            new GraphQLRequest 
            {
                Query = query
            };

        var response = await graphQLClient.SendQueryAsync<TResponseType>(heroRequest);
        return response.Data;
    }
}

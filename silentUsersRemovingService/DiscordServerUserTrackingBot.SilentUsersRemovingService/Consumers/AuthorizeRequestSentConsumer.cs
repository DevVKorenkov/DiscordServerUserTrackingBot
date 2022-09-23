//using DiscordServerUserTrackingBot.DiscordBot.Messages;
//using DiscordServerUserTrackingBot.SilentUsersRemovingService.UsersStorage;
//using DiscordServerUserTrackingBot.SilentUsersRemovingService.UsersStorage.Abstractions;
//using MassTransit;
//using Microsoft.Extensions.Logging;

//namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.Consumers;

//class AuthorizeRequestSentConsumer : IConsumer<AuthorizeRequestSentEvent>
//{
//    private readonly ILogger<AuthorizeRequestSentConsumer> _logger;
//    private readonly IRequestedUserRepository _requestedUserRepository;

//    public AuthorizeRequestSentConsumer(ILogger<AuthorizeRequestSentConsumer> logger, IRequestedUserRepository requestedUserRepository)
//    {
//        _logger = logger;
//        _requestedUserRepository = requestedUserRepository;
//    }

//    public async Task Consume(ConsumeContext<AuthorizeRequestSentEvent> context)
//    {
//        var clientId = context.Message.DiscordClientId;
//        await _requestedUserRepository.AddUserAsync(new RequestedUser(clientId));
//        await Task.FromResult(clientId);
//    }
//}

using Discord;
using Discord.WebSocket;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord.Abstractions;
using DiscordServerUserTrackingBot.DiscordBotClientService.Settings;
using MassTransit;
using Microsoft.Extensions.Options;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.Discord;

/// <summary>
/// Discord bot logic
/// </summary>
public class BotClientManager : IHostedService, IBotClient
{
    private const string roleForKickName = "roleForKick";

    private readonly DiscordBotSettings _botOptions;
    private readonly ILogger<BotClientManager> _logger;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private SocketGuild? _serverGuild;
    private TaskCompletionSource<bool> _readinessWaiter = new TaskCompletionSource<bool>();

    public BotClientManager(
        ILogger<BotClientManager> logger,
        IOptions<DiscordBotSettings> botOptions,
        IConfiguration configuration)
    {
        _logger = logger;
        _botOptions = botOptions.Value;
        _configuration = configuration;

        var socketConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All
        };

        _client = new DiscordSocketClient(socketConfig);
        _client.UserJoined += UserJoined;
        _client.Ready += async () =>
        {
            _readinessWaiter.TrySetResult(true);

            _serverGuild = _client.Guilds.First();

            var bot = _serverGuild?.Users.FirstOrDefault(b => b.IsBot);

            var roleForKick = _serverGuild?.Roles.FirstOrDefault(r => r.Name == roleForKickName) ?? await CreateRoleForKickAsync();

            if (bot != null && !bot.Roles.Contains(roleForKick))
            {
                await bot.AddRoleAsync(roleForKick);
            }

            _logger.LogInformation("Bot is ready!");
        };
        _client.Disconnected += async (_) =>
        {
            try
            {
                _logger.LogWarning("Bot is disconnected and it is trying to connect to the server");

                int increment = 1;

                do
                {
                    await Task.Delay(_botOptions.BotDisconnectedDelay * increment);
                    await _client.LoginAsync(TokenType.Bot, _botOptions.BotToken);
                    await _client.StartAsync();

                    increment++;

                    if (increment < _botOptions.ConnectionAttemptsCount)
                    {
                        throw new Exception("Connection attempts ended");
                    }

                } while (_client.ConnectionState != ConnectionState.Connected && _client.Status != UserStatus.Online);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Bot was disconnected by {errorMessage}", e.Message);
            }
        };
    }

    /// <summary>
    /// Initializes service
    /// </summary>
    public async Task StartAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _client.LoginAsync(TokenType.Bot, _botOptions.BotToken);
            await _client.StartAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "bot client start error");
        }

        var isHaveToSendEvents = _configuration.GetValue<bool>("send");

        if (isHaveToSendEvents)
        {
            var discordUsers = await GetAllUsersFromGuild();

            foreach (var user in discordUsers)
            {
                var userId = user.Id;

                await SendMessageAsync(user, $"{_botOptions.ReRegistrationMessage}\n{_botOptions.AuthenticationUrl}{userId}");
            }
        }
    }

    /// <summary>
    /// Stops service
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
        await _client.LogoutAsync();
    }

    public async Task<IReadOnlyCollection<IGuildUser>> GetAllUsersFromGuild()
    {
        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(_botOptions.ReadinessWaitTimeoutMs));

        var isTaskComplete = await Task.WhenAny(_readinessWaiter.Task, timeoutTask) != timeoutTask;

        if (isTaskComplete)
        {
            return _serverGuild.Users;
        }
        else
        {
            throw new TimeoutException("discord bot is not ready too long");
        }
    }

    /// <summary>
    /// Greets user and sends authentication url.
    /// </summary>
    /// <param name="user">New user.</param>
    private async Task UserJoined(IGuildUser user)
    {
        if (user.IsBot)
            return;

        var userId = user.Id;
        _logger.LogInformation($"User {user.Username} with id {userId} connected");
        var messageText = $"{_botOptions.WelcomeMessage}\n{_botOptions.AuthenticationUrl}{userId}";
        await SendMessageAsync(user, messageText);
    }

    private static async Task SendMessageAsync(IGuildUser user, string messageText)
    {
        var isOwner = user.Guild.OwnerId == user.Id;

        if (!user.IsBot && !isOwner)
        {
            await user.SendMessageAsync(messageText);
        }
    }

    public async Task SendMessageAsync(ulong userId, string messageText)
    {
        var user = await GetGuildUserById(userId);

        await SendMessageAsync(user, messageText);
    }
    /// <summary>
    /// Grants roles to server user.
    /// </summary>
    /// <param name="userId">Id of changing user.</param>
    public async Task GrantUserRoles(ulong userId)
    {
        var user = await GetGuildUserById(userId);

        await user.AddRolesAsync(_botOptions.Roles);
    }

    /// <summary>
    /// Removes user from server.
    /// </summary>
    /// <param name="userId">Id of removing user.</param>
    public async Task RemoveUserFromGuild(ulong userId)
    {
        var user = await GetGuildUserById(userId);

        var isOwner = user.Guild.OwnerId == user.Id;

        if (user.IsBot)
        {
            _logger.LogInformation("This user is bot and won't be kicked");
        }

        if (isOwner)
        {
            _logger.LogInformation("This user is owner and won't be kicked");
        }

        if (!user.IsBot && !isOwner)
        {
            var roleForKick = await CreateRoleForKickAsync();

            await user.AddRoleAsync(roleForKick);

            await user.RemoveRolesAsync(user.RoleIds.Where(r => r != roleForKick.Id));
            await user.KickAsync();

            _logger.LogInformation("User {user} was successfully kicked", user.Id);
        }
    }

    private async Task<IGuildUser> GetGuildUserById(ulong userId)
    {
        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(_botOptions.ReadinessWaitTimeoutMs));

        var isTaskComplete = await Task.WhenAny(_readinessWaiter.Task, timeoutTask) != timeoutTask;

        IGuildUser clientGuId;

        if (isTaskComplete)
        {
            clientGuId = _serverGuild.GetUser(userId);
        }
        else
        {
            throw new TimeoutException("discord bot is not ready too long");
        }

        return clientGuId ?? throw new UserNotFoundException(userId);
    }

    private async Task<IRole> CreateRoleForKickAsync()
    {
        IRole? roleForKick = _serverGuild.Roles.FirstOrDefault(r => r.Name == roleForKickName);

        return roleForKick ?? await _serverGuild.CreateRoleAsync(roleForKickName);
    }
}

using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Infrastructure.Enums;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;
using DiscordServerUserTrackingBot.YandexSyncService.Infrastructure.Abstractions;
using DiscordServerUserTrackingBot.YandexSyncService.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace DiscordServerUserTrackingBot.YandexSyncService.Services;

internal class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IYandexRepository _yandexRepository;

    public UserService(
        ILogger<UserService> logger,
        IUserRepository userRepository,
        IYandexRepository yandexRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _yandexRepository = yandexRepository;
    }

    public async Task UserSync(CancellationToken cancellationToken)
    {
        #region posible option
        //List<User> mongoActiveUsers = await _userRepository
        //    .IterateUsersAsync(u => u.UserStatus == UserStatus.Active, cancellationToken).ToListAsync();

        //var yandexUsers = await _yandexRepository.GetAllUsersFromYandex();

        //var inactiveUsers = mongoActiveUsers.FindAll(u => yandexUsers.Exists(y => y.Email == u.Email));

        //foreach (var user in inactiveUsers)
        //{
        //    var userEvent = GetEventForWithEventType(user.DiscordId, EventType.UserWasArchived);

        //    await _userRepository.UpdateUserAsync(
        //        user,
        //        u => u.UserStatus,
        //        UserStatus.Inactive,
        //        userEvent);
        //}
        #endregion

        await foreach (var user in _userRepository.IterateUsersAsync(u => u.UserStatus == UserStatus.Active, cancellationToken))
        {
            var yandexUserStatus = await _yandexRepository.GetUserStatusByEmail(user.Email);

            if (yandexUserStatus != UserStatus.Inactive)
            {
                continue;
            }

            var userEvent = GetEventForWithEventType(user.DiscordId, EventType.UserWasArchived);

            await _userRepository.UpdateUserAsync(
                user,
                u => u.UserStatus,
                UserStatus.Inactive,
                userEvent);
        }

        _logger.LogInformation($"Users status was synchronized at {DateTime.Now}");
    }

    public async Task UserProcessing(UserCreds userCreds)
    {
        var isUserExists = await IsThereUserInYandex(userCreds);

        if (!isUserExists)
        {
            return;
        }

        var user = await _userRepository.GetUserByEmailAsync(userCreds.Email);

        if (user == null)
        {
            await CreateNewUser(userCreds);
        }
        else if (user.DiscordId != userCreds.DiscordId)
        {
            var registerEvent = GetEventForWithEventType(userCreds.DiscordId, EventType.UserWasRegistered);

            var archiveEvent = GetEventForWithEventType(user.DiscordId, EventType.UserWasArchived);

            await _userRepository.UpdateUserAsync(
                user,
                u => u.DiscordId,
                userCreds.DiscordId,
                registerEvent,
                archiveEvent);
        }
    }

    //private Func<IEventRepository, Task> GetEventForWithEventType(string discordId, EventType eventType) 
    //    => eventRepository => eventRepository.PutEventToQueueAsync(new UserEvent(discordId, eventType));

    private Func<IEventRepository, Task> GetEventForWithEventType(string discordId, EventType eventType)
    {
        return eventRepository => eventRepository.PutEventToQueueAsync(new UserEvent(discordId, eventType));
    }

    public async Task SetEmailsCacheAsync()
    {
        await _yandexRepository.SetYandexEmailsCacheAsync();
    }

    private async Task CreateNewUser(UserCreds userCreds)
    {
        var user = new User(userCreds.DiscordId, userCreds.Email, UserStatus.Active);

        await _userRepository.AddUserAsync(user);
    }

    private async Task<bool> IsThereUserInYandex(UserCreds userCreds)
    {
        var userStatus = await _yandexRepository.GetUserStatusByEmail(userCreds.Email);

        return userStatus == UserStatus.Active;
    }
}


using DsBot.ActiveDirectorySync.Services.Abstractions;
using DsBot.ActiveDirectorySync.Infrastructure.Abstractions;
using DsBot.ConfigureSyncUserLib.Infrastructure.Enums;
using DsBot.ConfigureSyncUserLib.Models;
using DsBot.ConfigureSyncUserLib.Repositories.Abstrations;

namespace DsBot.ActiveDirectorySync.Services
{
    internal class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IActiveDirectoryRepository _activeDirectoryRepository;

        public UserService(
            ILogger<UserService> logger,
            IUserRepository userRepository,
            IActiveDirectoryRepository activeDirectoryRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _activeDirectoryRepository = activeDirectoryRepository;
        }

        public async Task UserSync(CancellationToken cancellationToken)
        {
            await foreach (var user in _userRepository.IterateUsersAsync(cancellationToken))
            {
                user.UserStatus = _activeDirectoryRepository.GetUserStatus(user.Email);

                if (user.UserStatus != UserStatus.Inactive)
                {
                    continue;
                }

                await UpdateUserStatus(user, EventType.UserWasArchived, cancellationToken);
            }

            _logger.LogInformation($"Users status was synchronized at {DateTime.Now}");
        }

        public async Task CheckUser(UserCreds userCreds, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(userCreds.Email, cancellationToken);

            if (user == null)
            {
                user = new User(userCreds.DiscordId, userCreds.Email, UserStatus.Active);
            }
            else
            {
                await UpdateUserStatus(user, EventType.UserWasArchived, cancellationToken);

                user.DiscordId = userCreds.DiscordId;
            }

            await UpdateUserStatus(user, EventType.UserWasActivated, cancellationToken);
        }

        private async Task UpdateUserStatus(User user, EventType eventType, CancellationToken cancellationToken)
        {
            await _userRepository.UpdateUserStatusAsync(
            user,
            new UserEvent(user.DiscordId, eventType),
            cancellationToken);
        }
    }
}

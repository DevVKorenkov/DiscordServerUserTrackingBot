using System.Linq.Expressions;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Infrastructure.Enums;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings;
using Microsoft.Extensions.Logging;

namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _mongoUsersCollection;
    private readonly IEventRepository _eventRepository;
    private readonly IMongoClient _mongoClient;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        IOptions<UserDbSettings> settings,
        IEventRepository eventRepository,
        IMongoClient mongoClient,
        ILogger<UserRepository> logger)
    {
        _logger = logger;

        _mongoClient = mongoClient;
        var database = _mongoClient.GetDatabase(settings.Value.DataBaseName);
        _mongoUsersCollection
            = database.GetCollection<User>(settings.Value.CollectionName);

        _eventRepository = eventRepository;
    }

    public async Task<User> GetUserByEmailAsync(string email) =>
        await _mongoUsersCollection.Find(u => u.Email == email).SingleOrDefaultAsync();

    public async Task AddUserAsync(User user)
    {
        try
        {
            using (var session = await _mongoClient.StartSessionAsync(options: null))
            {
                session.StartTransaction();

                try
                {
                    await _mongoUsersCollection.InsertOneAsync(user, new InsertOneOptions());

                    await _eventRepository.PutEventToQueueAsync(new UserEvent(user.DiscordId, EventType.UserWasRegistered));

                    await session.CommitTransactionAsync();

                    _logger.LogInformation($"User event {user.Email} was added into DB at: {DateTimeOffset.Now}");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(
                        $"User event {user.Email} wasn't added by {ex.Message} at: {DateTimeOffset.Now}");

                    await session.AbortTransactionAsync();
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning($"User adding was failed. {e.Message} at {DateTime.Now}");
        }
    }

    public async Task UpdateUserAsync(
        User user, 
        Expression<Func<User, object>> userUpdateFn, 
        object value,
        params Func<IEventRepository, Task>[] eventEmiters)
    {
        using (var session = await _mongoClient.StartSessionAsync(options: null))
        {
            session.StartTransaction();

            try
            {
                await UpdateUserPropAsync(user, userUpdateFn, value);

                if (eventEmiters.Length > 0)
                {
                    foreach (var eventEmiter in eventEmiters)
                    {
                        await eventEmiter(_eventRepository);
                    }
                }

                await session.CommitTransactionAsync();

                _logger.LogInformation($"User event {user.Email} was added into DB at: {DateTimeOffset.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    $"User event {user.Email} wasn't added by {ex.Message} at: {DateTimeOffset.Now}");

                await session.AbortTransactionAsync();
            }
        }
    }

    private async Task UpdateUserPropAsync(User user, Expression<Func<User, object>> field, object value)
    {
        var updateDefinition = Builders<User>.Update.Set(field, value);
        await this._mongoUsersCollection.UpdateOneAsync<User>((Expression<Func<User, bool>>)(x => x.Email == user.Email), updateDefinition);
    }

    public async IAsyncEnumerable<User> IterateUsersAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var cursor = await _mongoUsersCollection.FindAsync(_ => true, null, cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var user in cursor.Current)
            {
                yield return user;
            }
        }
    }

    public async IAsyncEnumerable<User> IterateUsersAsync(
        Expression<Func<User, bool>> filter,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var cursor = await _mongoUsersCollection.FindAsync(filter, null, cancellationToken);
        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var user in cursor.Current)
            {
                yield return user;
            }
        }
    }

    public async Task ReplaceUserData(User user)
    {
        await _mongoUsersCollection.ReplaceOneAsync(
                    u => u.Email == user.Email,
                    user,
                    new ReplaceOptions
                    {
                        IsUpsert = true
                    });

        _logger.LogInformation($"User status {user.Email} was updated in DB at: {DateTimeOffset.Now}");
    }
}
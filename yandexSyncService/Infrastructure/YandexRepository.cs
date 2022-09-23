using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Infrastructure.Enums;
using DiscordServerUserTrackingBot.YandexSyncService.Infrastructure.Abstractions;
using DiscordServerUserTrackingBot.YandexSyncService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DiscordServerUserTrackingBot.YandexSyncService.Infrastructure;

internal class YandexRepository : IYandexRepository
{
    private readonly ILogger<YandexRepository> _logger;
    private readonly HttpClient _yandexHttpClient;

    private IMemoryCache _memoryCache;

    private const string CacheKey = "yandexUsers";
    private const string UsersIdPropertyName = "id";
    private const string UsersJsonPropertyName = "users";
    private const string EmailJsonPropertyName = "email";

    public YandexRepository(
        ILogger<YandexRepository> logger,
        IHttpClientFactory httpFactory,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _yandexHttpClient = httpFactory.CreateClient("SyncUsers");
        _memoryCache = memoryCache;
    }

    public async Task<UserStatus> GetUserStatusByEmail(string email)
    {
        IEnumerable<YandexUser> yandexUsers = Enumerable.Empty<YandexUser>();

        if (!_memoryCache.TryGetValue(CacheKey, out yandexUsers))
        {
            var jsonContent = await GetYandexResponse();
            yandexUsers = GetUsersFromYandexResponse(jsonContent);
            SetMemoryCache(yandexUsers);
        }

        var userStatus 
            = yandexUsers.SingleOrDefault(u => u.Email == email) == null
            ? UserStatus.Inactive
            : UserStatus.Active;

        return userStatus;
    }

    public async Task<List<YandexUser>> GetAllUsersFromYandex()
    {
        var yandexUsers = new List<YandexUser>();

        if (!_memoryCache.TryGetValue(CacheKey, out yandexUsers))
        {
            var jsonContent = await GetYandexResponse();
            yandexUsers = GetUsersFromYandexResponse(jsonContent).ToList();
            SetMemoryCache(yandexUsers);
        }

        return yandexUsers;
    }

    public async Task SetYandexEmailsCacheAsync()
    {
        try
        {
            var jsonContent = await GetYandexResponse();

            var yandexUsers = GetUsersFromYandexResponse(jsonContent);

            SetMemoryCache(yandexUsers);
        }
        catch (Exception e)
        {
            _logger.LogError($"Cache hasn't set. {e.Message}");
        }
    }

    private void SetMemoryCache(object cacheObj)
    {
        _memoryCache.Set(CacheKey, cacheObj);

        _logger.LogInformation("Cache has been set.");
    }

    private async Task<string> GetYandexResponse()
    {
        var response = await _yandexHttpClient.GetAsync(_yandexHttpClient.BaseAddress);
        return await response.Content.ReadAsStringAsync();
    }

    private IEnumerable<YandexUser> GetUsersFromYandexResponse(string jsonString)
    {
        ICollection<YandexUser> emails = new List<YandexUser>();

        using (var jObj = JsonDocument.Parse(jsonString))
        {
            var users = jObj.RootElement.GetProperty(UsersJsonPropertyName).EnumerateArray();

            foreach (var element in users)
            {
                var id = element.GetProperty(UsersIdPropertyName).GetString();
                var email = element.GetProperty(EmailJsonPropertyName).GetString();

                emails.Add(new YandexUser
                {
                    Id = id,
                    Email = email,
                });
            }
        }

        return emails;
    }
}




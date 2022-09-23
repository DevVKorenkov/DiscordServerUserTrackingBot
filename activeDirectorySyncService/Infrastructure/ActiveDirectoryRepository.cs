using System.Collections;
using DsBot.ActiveDirectorySync.Extentions;
using DsBot.ActiveDirectorySync.Infrastructure.Abstractions;
using System.DirectoryServices;
using DsBot.ConfigureSyncUserLib.Infrastructure.Enums;

namespace DsBot.ActiveDirectorySync.Infrastructure;

public class ActiveDirectoryRepository : IActiveDirectoryRepository
{
    private readonly DirectoryEntry _directoryEntry;
    private readonly ILogger<ActiveDirectoryRepository> _logger;

    public ActiveDirectoryRepository(ILogger<ActiveDirectoryRepository> logger)
    {
        _logger = logger;

        var rootDirectory = new DirectoryEntry("LDAP://RootDSE");
        _directoryEntry = new DirectoryEntry("LDAP://" + rootDirectory.Properties["defaultNamingContext"][0]);
    }

    public UserStatus GetUserStatus(string email)
    {
        var searchResult = GetUserByEmail(email);

        var groupsCount = ((ICollection)searchResult?.GetDirectoryEntry().Properties["memberOf"].Value)?.Count;

        return !groupsCount.HasValue ? UserStatus.NotFound 
            : groupsCount > 0 ? UserStatus.Active : UserStatus.Inactive;
    }

    private SearchResult GetUserByEmail(string email)
    {
        return SearchThroughDirectory(searcher =>
        {
            searcher.PropertiesToLoad.Add("memberOf");
            searcher.Filter = $"(&(objectClass=User)(mail={email.Trim()}))";
            return searcher.FindOne();
        });
    }

    private SearchResult SearchThroughDirectory(Func<DirectorySearcher, SearchResult> searchFn)
    {
        using var search = new DirectorySearcher(_directoryEntry);

        return searchFn(search);
    }
}
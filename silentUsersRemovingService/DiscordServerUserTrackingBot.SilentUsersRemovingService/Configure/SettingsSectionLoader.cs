using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.Configure;

public static class SettingsSectionLoader
{
    public static T LoadSettingsSection<T>(this IServiceCollection serviceCollection, HostBuilderContext hostBuilderContext) where T : class
    {
        serviceCollection.SetSettingsFromSection<T>(hostBuilderContext);
        var settings =
            serviceCollection.BuildServiceProvider().GetService<IOptions<T>>().Value;
        return settings;
    }

    public static void SetSettingsFromSection<T>(this IServiceCollection serviceCollection,
        HostBuilderContext hostBuilderContext) where T : class
    {
        serviceCollection.Configure<T>(
            hostBuilderContext.Configuration.GetSection(typeof(T).Name));
    }
}

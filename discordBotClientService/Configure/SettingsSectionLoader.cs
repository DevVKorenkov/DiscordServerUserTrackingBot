using Microsoft.Extensions.Options;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.Configure;

public static class SettingsSectionLoader
{
    static public T LoadSettingsSection<T>(this IServiceCollection serviceCollection, HostBuilderContext hostBuilderContext) where T : class
    {
        serviceCollection.SetSettingsFromSection<T>(hostBuilderContext);

        var settings =
            serviceCollection.BuildServiceProvider().GetService<IOptions<T>>().Value;
        return settings;
    }

    static public void SetSettingsFromSection<T>(this IServiceCollection serviceCollection,
        HostBuilderContext hostBuilderContext) where T : class
    {
        serviceCollection.Configure<T>(
            hostBuilderContext.Configuration.GetSection(typeof(T).Name));
    }
}

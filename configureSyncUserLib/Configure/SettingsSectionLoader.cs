using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.Configure;

public static class SettingsSectionLoader
{
    public static T LoadSettingsSection<T>(this IServiceCollection serviceCollection, HostBuilderContext hostBuilderContext) where T : class
    {
        serviceCollection.SetSettingsFromSection<T>(hostBuilderContext);

        var settings = GetSettings<T>(serviceCollection);

        return settings;
    }

    public static T LoadSettingsSection<T>(this IServiceCollection serviceCollection, IConfiguration configuration) where T : class
    {
        serviceCollection.SetSettingsFromSection<T>(configuration);

        var settings = GetSettings<T>(serviceCollection);

        return settings;
    }

    public static void SetSettingsFromSection<T>(this IServiceCollection serviceCollection,
        HostBuilderContext hostBuilderContext) where T : class
    {
        serviceCollection.Configure<T>(
            hostBuilderContext.Configuration.GetSection(typeof(T).Name));
    }

    public static void SetSettingsFromSection<T>(this IServiceCollection serviceCollection,
        IConfiguration configuration) where T : class
    {
        serviceCollection.Configure<T>(
            configuration.GetSection(typeof(T).Name));
    }

    private static T GetSettings<T>(IServiceCollection serviceCollection) where T : class
    { 
        return serviceCollection.BuildServiceProvider().GetService<IOptions<T>>().Value;
    }
}

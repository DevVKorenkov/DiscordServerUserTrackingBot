using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordServerUserTrackingBot.ConfigureSyncUserLib.Configure;

public static class RabbitMqConfigurator
{
    static public void ConfigureRabbitMq(this IServiceCollection serviceCollection, RabbitMqSettings settings)
    {
        serviceCollection.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(settings.Url), h =>
                {
                    h.Username(settings.User);
                    h.Password(settings.Password);
                });
                cfg.UseMessageRetry(r => r.Incremental(10, TimeSpan.FromMinutes(5), TimeSpan.FromHours(6)));
                cfg.ConfigureEndpoints(context);
            });
        });
        serviceCollection.AddOptions<MassTransitHostOptions>()
            .Configure(options => { options.WaitUntilStarted = true; });
    }
}
